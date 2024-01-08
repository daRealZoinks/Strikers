using System;
using Unity.Netcode;
using UnityEngine;

public class CharacterWallRunController : NetworkBehaviour
{
    [field: Range(0.1f, 2f)]
    [field: SerializeField]
    public float WallCheckDistance { get; private set; } = 0.75f;

    [field: Space]
    [field: Header("Wall Run Settings")]
    [field: SerializeField]
    public float WallJumpHeight { get; private set; } = 4f;

    [field: SerializeField]
    public float WallRunInitialImpulse { get; private set; } = 7f;

    [field: Space]
    [field: Header("Wall Jump Settings")]
    [field: SerializeField]
    public float WallJumpSideForce { get; private set; } = 6f;

    [field: SerializeField]
    public float WallJumpForwardForce { get; private set; } = 5f;

    public bool IsWallRight { get; private set; }
    public bool IsWallLeft { get; private set; }
    public bool IsWallRunning => IsWallRight || IsWallLeft;

    public bool CanJump { get; set; }

    private RaycastHit _rightHitInfo;
    private RaycastHit _leftHitInfo;
    private CharacterMovementController _characterMovementController;

    private void Awake()
    {
        _characterMovementController = GetComponent<CharacterMovementController>();
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.NetworkTickSystem.Tick += OnNetworkTick;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;
    }

    private void OnNetworkTick()
    {
        _characterMovementController.MovementEnabled = !IsWallRunning;
        CheckForWallRun();
        if (_characterMovementController.JumpInput && CanJump)
        {
            WallJump();
            CanJump = false;
        }
    }

    private void CheckForWallRun()
    {
        if (_characterMovementController.IsGrounded)
        {
            IsWallRight = false;
            IsWallLeft = false;
            return;
        }

        var rightRay = new Ray(transform.position, transform.right);
        var leftRay = new Ray(transform.position, -transform.right);

        var wasWallRight = IsWallRight;
        var wasWallLeft = IsWallLeft;

        IsWallRight = Physics.Raycast(rightRay, out _rightHitInfo, WallCheckDistance);
        IsWallLeft = Physics.Raycast(leftRay, out _leftHitInfo, WallCheckDistance);

        var boostForce = Vector3.zero;

        if (IsWallRight && !wasWallRight)
            boostForce = -Vector3.Cross(_rightHitInfo.normal, transform.up) * WallRunInitialImpulse;

        if (IsWallLeft && !wasWallLeft)
            boostForce = Vector3.Cross(_leftHitInfo.normal, transform.up) * WallRunInitialImpulse;

        _characterMovementController.Rigidbody.AddForce(boostForce, ForceMode.VelocityChange);

        if (IsWallRight) _characterMovementController.Rigidbody.AddForce(-_rightHitInfo.normal, ForceMode.Acceleration);
        if (IsWallLeft) _characterMovementController.Rigidbody.AddForce(-_leftHitInfo.normal, ForceMode.Acceleration);

        if (!_characterMovementController.JumpInput)
        {
            CanJump = IsWallRunning;
        }
    }

    public void WallJump()
    {
        if (!IsWallRunning) return;

        var sideForce = Vector3.zero;
        if (IsWallRight) sideForce = _rightHitInfo.normal * WallJumpSideForce;
        if (IsWallLeft) sideForce = _leftHitInfo.normal * WallJumpSideForce;

        var jumpForce = Vector3.up * Mathf.Sqrt(WallJumpHeight * -2 * Physics.gravity.y);

        var forwardForce = transform.forward * WallJumpForwardForce;

        var finalForce = jumpForce + sideForce + forwardForce;

        var rigidbodyVelocity = _characterMovementController.Rigidbody.velocity;
        rigidbodyVelocity = new Vector3(rigidbodyVelocity.x, 0f, rigidbodyVelocity.z);
        _characterMovementController.Rigidbody.velocity = rigidbodyVelocity;

        if (IsWallRight)
        {
            var velocity = Vector3.Project(_characterMovementController.Rigidbody.velocity, _rightHitInfo.normal);

            _characterMovementController.Rigidbody.velocity -= velocity;
        }

        if (IsWallLeft)
        {
            var velocity = Vector3.Project(_characterMovementController.Rigidbody.velocity, _leftHitInfo.normal);

            _characterMovementController.Rigidbody.velocity -= velocity;
        }

        IsWallRight = false;
        IsWallLeft = false;

        _characterMovementController.Rigidbody.AddForce(finalForce, ForceMode.VelocityChange);
    }
}
