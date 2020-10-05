using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameStateController gameStateController;
    public GroundMovementController groundMovementController;
    // public GroundMovementController2 groundMovementController;
    public CameraController cameraController;
    public MusicController musicController;
    public Animator animator;

    public Transform spriteTransform;
    public SpriteRenderer spriteRenderer;

    public float flipMaxThresh = 0.1f;
    public float flipMinThresh = -3.1f;
    public bool isFlipped = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!gameStateController) gameStateController = FindObjectOfType<GameStateController>();
        if (!musicController) musicController = FindObjectOfType<MusicController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStateController.IsPaused) return;

        if (Input.GetButtonDown("Jump")) {
            groundMovementController.Jump();
        }

        float horizontal = Input.GetAxis("Horizontal");
        //int horizontal = 0;
        //if (Input.GetKey(KeyCode.D)) {
        //    horizontal = 1;
        //} else if (Input.GetKey(KeyCode.A)) {
        //    horizontal = -1;
        //}

        
        groundMovementController.Move(horizontal * (isFlipped ? -1f : 1f));

        float center = (flipMaxThresh + flipMinThresh) / 2;

        if (isFlipped && transform.position.y > flipMaxThresh) {
            cameraController.targetZRotation = 0f;
            groundMovementController.SetGravityScale(1.0f);
            groundMovementController.SetRelative(Vector2.up, Vector2.right);
            musicController.ReplaceTrack(1, 0, 1f);
            isFlipped = false;
            spriteRenderer.flipY = false;
        } else if (!isFlipped && transform.position.y < flipMinThresh) {
            groundMovementController.SetGravityScale(-1.0f);
            groundMovementController.SetRelative(Vector2.down, Vector2.left);
            isFlipped = true;
            musicController.ReplaceTrack(0, 1, 1f);
            cameraController.targetZRotation = 180f;
            spriteRenderer.flipY = true;
        }

        if (groundMovementController.LastDirection < 0f) {
            spriteRenderer.flipX = !isFlipped;
        } else if (groundMovementController.LastDirection > 0f) {
            spriteRenderer.flipX = isFlipped;
        }

        if (isFlipped) {
            animator.SetBool("falling", groundMovementController.Velocity.y > 0);
        } else {
            animator.SetBool("falling", groundMovementController.Velocity.y < 0);
        }

        animator.SetBool("moving", Mathf.Abs(groundMovementController.Velocity.x) > 0.15f);
        animator.SetBool("grounded", groundMovementController.IsGrounded);
    }

    public void TeleportTo(Vector3 target) {
        transform.position = target;
        // groundMovementController.TeleportTo(target, true);
        groundMovementController.SetVelocity(Vector2.zero);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(-25f, flipMaxThresh), new Vector3(25f, flipMaxThresh));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-25f, flipMinThresh), new Vector3(25f, flipMinThresh));
    }
}
