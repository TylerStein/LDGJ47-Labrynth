using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroundMovementState
{
    IDLE = 0,
    MOVING = 1,
    JUMPING = 2,
    FALLING = 3
}

public class GroundMovementController2 : MonoBehaviour
{
    public int Facing { get => (int)Mathf.Sign(_moveX); }
    public bool IsMoving { get => Mathf.Abs(_moveX) == 1; }
    public bool Jumping { get => _jumpGraceTimer == 0; }

    [SerializeField] public GroundMovementSettings2 settings;
    [SerializeField] public Rigidbody2D body;
    [SerializeField] public BoxCollider2D verticalCollider;
    [SerializeField] public BoxCollider2D horizontalCollider;
    [SerializeField] public new Transform transform;
    [SerializeField] public Vector2 Acceleration;

    [SerializeField] private Vector2 _up = Vector2.up;
    [SerializeField] private Vector2 _right = Vector2.up;
    [SerializeField] private float _gravityMultiplier = 1.0f;

    [SerializeField] private float _inputX;
    [SerializeField] private float _inputY;

    [SerializeField] private int _moveX = 0;
    [SerializeField] private int _moveY = 0;

    [SerializeField] private bool _onGround = false;

    [SerializeField] private bool _shouldJump = false;

    [SerializeField] private float _speedX = 0f;
    [SerializeField] private float _speedY = 0f;
    [SerializeField] private float _jumpGraceTimer = 0f;
    [SerializeField] private float _jumpPeakTimer = 0f;
    [SerializeField] private float _jumpLaunchGraceTimer = 0f;

    [SerializeField] private bool _hasHorizontalContact = false;

    [SerializeField] private RaycastHit2D[] _contacts = new RaycastHit2D[8];

    private void Start() {
        if (!body) body = GetComponent<Rigidbody2D>();
        if (!transform) transform = GetComponent<Transform>();
        if (!settings) throw new UnityException("GroundMovementController is missing movement settings");

        body.isKinematic = true;
    }

    private void FixedUpdate() {
        UpdateGrounded();
        UpdateState();
        UpdateMovement();
        UpdateCollision();

        transform.position += new Vector3(_speedX, _speedY, 0f) * Time.fixedDeltaTime;
        body.velocity = Vector2.zero;
        body.angularVelocity = 0f;
    }

    public void Move(float move) {
        _inputX = move;
        if (move > 0) _moveX = 1;
        else if (move < 0) _moveX = -1;
        else _moveX = 0;
    }

    public void Jump() {
        if (_onGround) _shouldJump = true;
    }

    public void TeleportTo(Vector3 location, bool killSpeed = false) {
        transform.position = location;
        if (killSpeed) {
            _speedX = 0f;
            _speedY = 0f;
        }
    }

    public void SetRelative(Vector3 up, Vector3 right) {
        _up = up;
        _right = right;
    }

    public void SetGravityScale(float scale) {
        _gravityMultiplier = scale;
    }

    private void UpdateGrounded() {

        if (_jumpLaunchGraceTimer == 0f) {
            ContactFilter2D filter = new ContactFilter2D();
            filter.ClearLayerMask();
            int contactCount = Physics2D.BoxCast(transform.position, verticalCollider.size, 0f, _up * -1f, filter, _contacts, settings.MinGroundDistance * 2);
            for (int i = 0; i < contactCount; i++) {
                if (IsCollidable(_contacts[i].collider)) {
                    _onGround = true;
                    return;
                }
            }
        }

        _onGround = false;
    }

    private void UpdateState() {
        if (_onGround) {
            _jumpGraceTimer = settings.JumpGraceTime;
            _jumpPeakTimer = 0f;
        } else if (_jumpGraceTimer > 0f) {
            _jumpGraceTimer -= Time.fixedDeltaTime;
            if (_jumpGraceTimer < 0f) _jumpGraceTimer = 0f;
        }

        if (_jumpPeakTimer > 0f) {
            if (Mathf.Abs(_speedY) >= settings.MaxJumpSpeed) {
                CancelJump();
            } else {
                _jumpPeakTimer -= Time.fixedDeltaTime;
                if (_jumpPeakTimer < 0f) _jumpPeakTimer = 0f;
            }
        }

        if (_jumpLaunchGraceTimer > 0f) {
            _jumpLaunchGraceTimer -= Time.fixedDeltaTime;
            if (_jumpLaunchGraceTimer < 0f) _jumpLaunchGraceTimer = 0f;
        }
    }

    private void UpdateCollision() {
        ContactFilter2D filter = new ContactFilter2D();
        filter.ClearLayerMask();

        _hasHorizontalContact = false;
        if (Mathf.Abs(_speedX) > 0f) {
            int hContactCount = Physics2D.BoxCast(transform.position, horizontalCollider.size, 0f, _right * _speedX, filter, _contacts, settings.MinWallDistance);
            for (int i = 0; i < hContactCount; i++) {
                if (IsCollidable(_contacts[i].collider)) {
                    float collisionX = _contacts[i].point.x;
                    float playerX = transform.position.x;

                    if (Mathf.Sign(collisionX - playerX) == _moveX) {
                        _hasHorizontalContact = true;
                        _speedX = ResolveXCollisionSpeed(_contacts[i]);
                        break;
                    }
                }
            }
        }
        
        if (Mathf.Abs(_speedY) > 0f) {
            float minDistance = Mathf.Sign(_speedY) == 1 ? settings.MinCeilingDistance : settings.MinGroundDistance;
            int vContactCount = verticalCollider.Cast(_right * _speedY, filter, _contacts, settings.MinWallDistance);
            for (int i = 0; i < vContactCount; i++) {
                if (IsCollidable(_contacts[i].collider)) {
                    float collisionY = _contacts[i].point.y;
                    float playerY = transform.position.y;

                    if (Mathf.Sign(_gravityMultiplier) == Mathf.Sign(collisionY - playerY)) {
                        CancelJump();
                    }

                    if (Mathf.Sign(collisionY - playerY) == Mathf.Sign(_speedY)) {
                        _speedY = ResolveYCollisionSpeed(_contacts[i]);
                        break;
                    }
                }
            }
        }
    }

    private void UpdateMovement() {
        // Ground
        float accel = _onGround ? settings.GroundAcceleration : settings.AirAcceleration;
        float max = _onGround ? settings.MaxGroundSpeed : settings.MaxAirSpeed;

        if (_speedX != 0 && (_moveX == 0 || (Mathf.Sign(_speedX) != Mathf.Sign(_moveX)))) {
            // Brake
            _speedX = Mathf.Lerp(_speedX, max * _moveX, settings.GroundBraking * Time.fixedDeltaTime);
            if (Mathf.Abs(_speedX) < 0.001f) _speedX = 0f;
        } else {
            _speedX = Mathf.Lerp(_speedX, max * _moveX, accel * Time.fixedDeltaTime);
            if (Mathf.Abs(_speedX) < 0.001f) _speedX = 0f;
        }

        _speedY = 0;
        if (_jumpPeakTimer > 0f) {
            _speedY = Mathf.Lerp(_speedY, settings.MaxJumpSpeed * _gravityMultiplier, settings.MaxJumpSpeed * _gravityMultiplier * Time.fixedDeltaTime);
        }

        if (_moveY == -1 || _speedY > settings.MaxFallSpeed) {
            // fast fall
            _speedY = Mathf.Lerp(_speedY, settings.MaxFastFallSpeed * _gravityMultiplier * -1, settings.FastFallAcceleration * _gravityMultiplier * Time.fixedDeltaTime);
        } else {
            // gravity fall
            _speedY = Mathf.Lerp(_speedY, settings.MaxFallSpeed * _gravityMultiplier * -1, settings.GravityAcceleration * _gravityMultiplier * Time.fixedDeltaTime);
        }

        if (_shouldJump) {
            if (_jumpGraceTimer > 0f) {
                ApplyJump();
            }
        }
    }

    private void CancelJump() {
        _jumpPeakTimer = 0f;
        _jumpLaunchGraceTimer = 0f;
    }

    private void ApplyJump() {
        _jumpGraceTimer = 0f;
        _jumpPeakTimer = settings.JumpTime;
        _jumpLaunchGraceTimer = settings.JumpLaunchGraceTime;
        _shouldJump = false;
        _onGround = false;
    }

    private bool IsCollidable(Collider2D other) {
        return other != null
            && other.transform != transform
            && other.tag == settings.CollidableTag;
    }
    
    private float ResolveYCollisionSpeed(RaycastHit2D hit) {
        return 0;
    }

    private float ResolveXCollisionSpeed(RaycastHit2D hit) {
        return 0;
    }
}
