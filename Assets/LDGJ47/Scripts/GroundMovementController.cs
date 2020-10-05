﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class GroundMovementController : MonoBehaviour
{
    // Last direction of movement (by input)
    public float LastDirection { get { return _lastDirection; } }

    // Is there anything beneath this
    public bool IsBlocked { get { return _isBlocked; } }

    // Is the player colliding down
    public bool IsGrounded { get { return _isGrounded; } }

    // Is the controller colliding upw
    public bool IsTouchingCeiling { get { return _isTouchingCeiling; } }

    // Int defines direction of wall, 0 means none
    public int TouchingWallDirection { get { return _touchingWallDirection; } }

    // Rigidbody's current velocity
    public Vector2 Velocity { get { return rigidbody.velocity; } }

    public Vector2 RelativeUp { get { return _relativeUp; } }
    public Vector2 RelativeRight { get { return _relativeRight; } }
    public Vector2 RelativeLeft { get { return _relativeRight * -1f; } }
    public Vector2 RelativeDown { get { return _relativeUp * -1f; } }

    [SerializeField] public GameStateController gameState;

    [SerializeField] public GroundMovementSettings movementSettings;
    [SerializeField] public new Collider2D collider;
    [SerializeField] public new Rigidbody2D rigidbody;

    [SerializeField] private Transform _transform;
    [SerializeField] private RaycastHit2D[] _contacts = new RaycastHit2D[3];
    [SerializeField] private Vector2 _currentVelocity = Vector2.zero;
    [SerializeField] private bool _shouldJump = false;
    [SerializeField] private float _lastDirection = 1f;
    [SerializeField] private bool _isGrounded = false;
    [SerializeField] private int _touchingWallDirection = 0;
    [SerializeField] private bool _isTouchingCeiling = false;
    [SerializeField] private bool _didMoveLastFrame = false;
    [SerializeField] private bool _isBlocked = false;

    [SerializeField] private Vector2 _relativeUp = Vector2.up;
    [SerializeField] private Vector2 _relativeRight = Vector2.right;

    public void Start() {
        if (!_transform) _transform = GetComponent<Transform>();
        if (!rigidbody) rigidbody = GetComponent<Rigidbody2D>();
        if (!collider) collider = GetComponent<BoxCollider2D>();

        gameState = FindObjectOfType<GameStateController>();

        rigidbody.isKinematic = false;
        rigidbody.simulated = true;
        rigidbody.freezeRotation = true;
    }

    public void SetGravityScale(float scale) {
        rigidbody.gravityScale = scale;
    }

    public void FixedUpdate() {
        if (gameState.IsPaused) {
            rigidbody.simulated = false;
            return;
        } else if (rigidbody.simulated == false) {
            rigidbody.simulated = true;
        }

        if (!_didMoveLastFrame) {
            dampenMovement();
        }
        _didMoveLastFrame = false;

        
        rigidbody.velocity = Vector2.ClampMagnitude(rigidbody.velocity, movementSettings.maxVelocity);
    }

    public void SetRelative(Vector2 up, Vector2 right) {
        _relativeUp = up;
        _relativeRight = right;
    }

    public void SetVelocity(Vector2 velocity) {
        rigidbody.velocity = velocity;
    }

    public bool Jump(bool canJumpInAir = false) {
        if (_isTouchingCeiling) return false;

        if (canJumpInAir) _shouldJump = true;
        else if (_isGrounded) _shouldJump = true;
        else if (movementSettings.enableWallJump && _touchingWallDirection != 0) _shouldJump = true;

        if ((_isGrounded || canJumpInAir) && !_isTouchingCeiling) _shouldJump = true;

        return _shouldJump;
    }

    public void Move(float direction) {
        float adjustedDirection = direction * RelativeRight.x;

        if (adjustedDirection > 0) _lastDirection = 1f;
        else if (adjustedDirection < 0) _lastDirection = -1f;

        // prevent wall sticking
        if (!movementSettings.enableWallJump) {
            if (Mathf.Sign(adjustedDirection) == _touchingWallDirection) {
                direction = 0;
            }
        }

        // prevent affecting velocity with move when input is 0
        _didMoveLastFrame = direction > 0f || direction < 0f;
        if (!_didMoveLastFrame) return;

        if (_isGrounded) {
            float desiredDirection = Mathf.Sign(direction);
            Vector2 targetVelocity = new Vector2(direction * movementSettings.groundMoveVelocity, rigidbody.velocity.y);
            SetVelocity(Vector2.SmoothDamp(rigidbody.velocity, targetVelocity, ref _currentVelocity, movementSettings.groundMoveSmoothing));
        } else if (movementSettings.canMoveInAir) {
            float desiredDirection = Mathf.Sign(direction);
            Vector2 targetVelocity = new Vector2(direction * movementSettings.airMoveVelocity, rigidbody.velocity.y);
            SetVelocity(Vector2.SmoothDamp(rigidbody.velocity, targetVelocity, ref _currentVelocity, movementSettings.airMoveSmoothing));
        }
    }

    public void Update() {
        updateTouchingWalls();
        updateTouchingCeiling();
        updateBlocked();

        if (_shouldJump) {
            _shouldJump = false;

            if (_isGrounded || _touchingWallDirection == 0) {
                _isGrounded = false;
                _isBlocked = false;
                rigidbody.AddForce(RelativeUp * movementSettings.jumpForce);
            } else {
                Move(-_touchingWallDirection);
                Vector2 rightForce = RelativeRight * movementSettings.jumpForce * 0.66f * -_touchingWallDirection;
                Vector2 upForce = RelativeUp * movementSettings.jumpForce * 0.66f;
                rigidbody.AddForce(rightForce + upForce);
                _isGrounded = false;
                _isBlocked = false;
                _touchingWallDirection = 0;
            }

        }
    }


    public void AddForce(Vector2 force) {
        rigidbody.AddForce(force, ForceMode2D.Impulse);
    }

    public void ClearVelocity() {
        SetVelocity(Vector2.SmoothDamp(rigidbody.velocity, Vector2.zero, ref _currentVelocity, 0.0001f));
    }

    private void dampenMovement() {
        if (_isGrounded) {
           Vector2 targetVelocity = new Vector2(0, rigidbody.velocity.y);
           if (_currentVelocity.x < 0.01f) _currentVelocity.Set(0, _currentVelocity.y);
           SetVelocity(Vector2.SmoothDamp(rigidbody.velocity, targetVelocity, ref _currentVelocity, movementSettings.groundStopSmoothing));

        } else if (movementSettings.dampenAirMovement) {
           Vector2 targetVelocity = new Vector2(0, rigidbody.velocity.y);
            if (_currentVelocity.x < 0.01f) _currentVelocity.Set(0, _currentVelocity.y);
            SetVelocity(Vector2.SmoothDamp(rigidbody.velocity, targetVelocity, ref _currentVelocity, movementSettings.airStopSmoothing));
        }
    }

    private void updateTouchingWalls() {
        if (_lastDirection < 0) {
            // looking left, test left first
            if (isTouching(RelativeLeft, movementSettings.minWallDistance * 1.1f, movementSettings.wallLayer)) {
                _touchingWallDirection = -1;
            } else if (isTouching(RelativeRight, movementSettings.minWallDistance * 1.1f, movementSettings.wallLayer)) {
                _touchingWallDirection = 1;
            } else {
                _touchingWallDirection = 0;
            }
        } else {
            // looking right, test right first
            if (isTouching(RelativeRight, movementSettings.minWallDistance * 1.1f, movementSettings.wallLayer)) {
                _touchingWallDirection = 1;
            } else if (isTouching(RelativeLeft, movementSettings.minWallDistance * 1.1f, movementSettings.wallLayer)) {
                _touchingWallDirection = -1;
            } else {
                _touchingWallDirection = 0;
            }
        }
    }

    private void updateTouchingCeiling() {
        _isTouchingCeiling = isTouching(RelativeUp, movementSettings.minCeilingDistance * 1.1f, movementSettings.ceilingLayer);
    }

    private void updateBlocked() {
        ContactFilter2D filter = new ContactFilter2D();
        filter.ClearLayerMask();

        int contactCount = collider.Cast(RelativeDown, filter, _contacts, movementSettings.minGroundDistance);
        for (int i = 0; i < contactCount; i++) {
            if (_contacts[i].collider != null && _contacts[i].transform != transform) {
                _isBlocked = true;
                if (_contacts[i].collider.tag == "Ground") {
                    _isGrounded = true;     
                }
                return;
            }
        }
        _isGrounded = false;
        _isBlocked = false;
    }

    private bool isTouching(Vector2 direction, float distance, int mask = 1 << 0) {
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = mask;

        int contactCount = collider.Cast(direction, filter, _contacts, distance);
        for (int i = 0; i < contactCount; i++) {
            if (_contacts[i].collider != null && _contacts[i].collider != collider) {
                return true;
            }
        }
        return false;
    }
}
