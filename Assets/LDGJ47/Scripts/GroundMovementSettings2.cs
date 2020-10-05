using UnityEngine;

[CreateAssetMenu(fileName="New Ground Movement Settings 2", menuName = "LDGJ47/MovementSettings", order=0)]
public class GroundMovementSettings2 : ScriptableObject
{
    [Header("Ground Movement")]

    [Tooltip("Maximum ground movement speed")]
    [SerializeField] public float MaxGroundSpeed = 10.0f;

    [Tooltip("Ground acceleration")]
    [SerializeField] public float GroundAcceleration = 10.0f;

    [Tooltip("Ground deceleration")]
    [SerializeField] public float GroundBraking = 5.0f;

    [Header("Air Movement")]

    [Tooltip("Jump Time")]
    [SerializeField] public float JumpTime = 0.75f;

    [Tooltip("Time after jumping to ignore being on the ground")]
    [SerializeField] public float JumpLaunchGraceTime = 0.15f;

    [Tooltip("Maximum vertical air jumping speed")]
    [SerializeField] public float MaxJumpSpeed = 20.0f;

    [Tooltip("Maximum vertical air falling speed")]
    [SerializeField] public float MaxFallSpeed = 10.0f;

    [Tooltip("Maxmimum vertical fast fall speed")]
    [SerializeField] public float MaxFastFallSpeed = 20.0f;

    [Tooltip("Fast fall acceleration")]
    [SerializeField] public float FastFallAcceleration = 20.0f;

    [Tooltip("Base gravity acceleration")]
    [SerializeField] public float GravityAcceleration = 9.8f;

    [Tooltip("Maximum horizontal air speed")]
    [SerializeField] public float MaxAirSpeed = 10.0f;

    [Tooltip("Ground acceleration")]
    [SerializeField] public float AirAcceleration = 0.5f;

    [Header("Accessibility")]

    [Tooltip("Time off ground the player can still jump")]
    [SerializeField] public float JumpGraceTime = 0.15f;

    [Header("Collision")]

    [Tooltip("Tag a collider must have to block the player")]
    [SerializeField] public string CollidableTag = "Collidable";

    [Tooltip("How close should the ground be below to consider touching (meters)")]
    [SerializeField] public float MinGroundDistance = 0.01f;

    [Tooltip("How close should the ceiling be above to consider touching (meters")]
    [SerializeField] public float MinCeilingDistance = 0.01f;

    [Tooltip("How close should a wall be beside the player to consider touching (meters)")]
    [SerializeField] public float MinWallDistance = 0.01f;

    [Header("Contact Layers")]

    [Tooltip("Define Ground layer mask for contacts to count as OnGround")]
    [SerializeField] public LayerMask GroundLayer = 0x0;

    [Tooltip("Define Ceiling layer mask for contacts to count as OnCeiling")]
    [SerializeField] public LayerMask CeilingLayer = 0x0;

    [Tooltip("Define Ceiling layer mask for contacts to count as OnWall")]
    [SerializeField] public LayerMask WallLayer = 0x0;

}
