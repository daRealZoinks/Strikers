using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class CharacterMovementController : NetworkBehaviour
{
    #region Movement

    [field: Header("Movement")]
    [field: SerializeField]
    public float MaxSpeed { get; private set; } = 13f;

    [field: SerializeField] public float Acceleration { get; private set; } = 64f;
    [field: SerializeField] public float Deceleration { get; private set; } = 128f;

    #endregion

    #region Jump

    [field: Space]
    [field: Header("Jump")]
    [field: SerializeField]
    public float JumpHeight { get; private set; } = 3f;

    [field: Range(0, 1f)]
    [field: SerializeField]
    public float AirControl { get; private set; } = 0.1f; // 0 - 1

    [field: Range(0, 1f)]
    [field: SerializeField]
    public float AirBreak { get; private set; } = 0f; // 0 - 1

    [field: SerializeField] public float GravityScale { get; private set; } = 1.5f;

    #endregion

    #region Camera

    [field: Space]
    [field: Header("Camera")]
    [field: SerializeField]
    private CinemachineVirtualCamera VirtualCamera { get; set; }

    [field: SerializeField] private float Sensitivity { get; set; } = 0.1f;
    [field: SerializeField] private float MinimumAngle { get; set; } = -90f;
    [field: SerializeField] private float MaximumAngle { get; set; } = 90f;

    #endregion

    #region Wall Run

    [field: Space]
    [field: Header("Wall Run Settings")]
    [field: Range(0.1f, 2f)]
    [field: SerializeField]
    public float WallCheckDistance { get; private set; } = 0.75f;

    [field: SerializeField] public float WallJumpHeight { get; private set; } = 4f;
    [field: SerializeField] public float WallRunInitialImpulse { get; private set; } = 7f;

    [field: Header("Wall Jump Settings")]
    [field: SerializeField]
    public float WallJumpSideForce { get; private set; } = 6f;

    [field: SerializeField] public float WallJumpForwardForce { get; private set; } = 5f;

    #endregion

    public event Action OnJump;

    public delegate void OnLandedDelegate(float fallSpeed);

    public event OnLandedDelegate OnLanded;
    public Rigidbody Rigidbody { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool MovementEnabled { get; set; }
    public bool CanJump { get; set; }
    public bool IsWallRight { get; private set; }
    public bool IsWallLeft { get; private set; }
    public bool IsWallRunning => IsWallRight || IsWallLeft;
    public bool CanWallJump { get; set; }

    public Vector2 MovementInput { get; set; }
    public bool JumpInput { get; set; }
    public Vector2 LookInput { get; set; }

    private float _xRotation;
    private RaycastHit _rightHitInfo;
    private RaycastHit _leftHitInfo;

    private const int BufferLength = 1024;
    private InputSnapshot[] inputSnapshots = new InputSnapshot[BufferLength];
    private StateSnapshot[] stateSnapshots = new StateSnapshot[BufferLength];
    private Queue<InputSnapshot> inputSnapshotQueue = new();

    private NetworkVariable<StateSnapshot> serverStateSnapshot = new();
    private StateSnapshot previousStateSnapshot;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.NetworkTickSystem.Tick += OnNetworkTick;
        serverStateSnapshot.OnValueChanged += OnStateSnapshotChanged;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.NetworkTickSystem.Tick -= OnNetworkTick;
        serverStateSnapshot.OnValueChanged -= OnStateSnapshotChanged;
    }

    private void OnNetworkTick()
    {
        var inputSnapshot = new InputSnapshot
        {
            Tick = NetworkManager.NetworkTickSystem.LocalTime.Tick,
            MovementInput = MovementInput,
            LookInput = LookInput,
            JumpInput = JumpInput
        };

        inputSnapshots[NetworkManager.NetworkTickSystem.LocalTime.Tick % BufferLength] = inputSnapshot;

        if (!IsServer)
        {
            stateSnapshots[NetworkManager.NetworkTickSystem.LocalTime.Tick % BufferLength] =
                PerformMovement(inputSnapshot);

            PerformMovementServerRpc(inputSnapshot);
        }
        else
        {
            if (IsOwnedByServer)
            {
                PerformMovement(inputSnapshot);
            }
        }
    }

    private void OnStateSnapshotChanged(StateSnapshot previousValue, StateSnapshot newValue)
    {
        if (IsServer) return;

        if (stateSnapshots[newValue.Tick % BufferLength].Position != newValue.Position)
        {
            Rigidbody.position = newValue.Position;
            Rigidbody.rotation = newValue.Rotation;
            Rigidbody.velocity = newValue.Velocity;
        }
    }

    [ServerRpc]
    private void PerformMovementServerRpc(InputSnapshot inputSnapshot)
    {
        var stateSnapshot = PerformMovement(inputSnapshot);

        previousStateSnapshot = serverStateSnapshot.Value;
        serverStateSnapshot.Value = stateSnapshot;
    }

    private StateSnapshot PerformMovement(InputSnapshot inputSnapshot)
    {
        MovementEnabled = !IsWallRunning;

        Move(inputSnapshot.MovementInput);

        UpdateCameraRotation(inputSnapshot.LookInput);

        ApplyAdditionalGravity();

        CheckForWallRun();

        if (inputSnapshot.JumpInput)
        {
            if (CanJump)
            {
                Jump();
                CanJump = false;
            }

            if (CanWallJump)
            {
                WallJump();
                CanWallJump = false;
            }
        }

        return new StateSnapshot
        {
            Tick = inputSnapshot.Tick,
            Position = Rigidbody.position,
            Rotation = Rigidbody.rotation,
            Velocity = Rigidbody.velocity
        };
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.contacts.Any(contact => Vector3.Dot(contact.normal, Vector3.up) > 0.5f)) return;

        IsGrounded = true;

        if (!JumpInput)
        {
            CanJump = true;
        }

        OnLanded?.Invoke(Rigidbody.velocity.y);
    }

    private void OnCollisionExit()
    {
        IsGrounded = false;
    }

    private void ApplyAdditionalGravity()
    {
        if (!IsGrounded) Rigidbody.AddForce(Physics.gravity * (GravityScale - 1), ForceMode.Acceleration);
    }

    private void Move(Vector2 movementInput)
    {
        if (!MovementEnabled) return;
        var horizontalVelocity = new Vector3()
        {
            x = Rigidbody.velocity.x,
            z = Rigidbody.velocity.z
        };
        var horizontalClampedVelocity =
            horizontalVelocity.normalized * Mathf.Clamp01(horizontalVelocity.magnitude / MaxSpeed);

        var moveDirection = (movementInput.x * transform.right + movementInput.y * transform.forward).normalized;

        Vector3 finalForce;

        if (moveDirection != Vector3.zero)
        {
            var accelerationForce = moveDirection - horizontalClampedVelocity;
            accelerationForce *= Acceleration * (IsGrounded ? 1 : AirControl);
            finalForce = accelerationForce;
        }
        else
        {
            var decelerationForce = -horizontalClampedVelocity;
            decelerationForce *= Deceleration * (IsGrounded ? 1 : AirBreak);
            finalForce = decelerationForce;
        }

        Rigidbody.AddForce(finalForce, ForceMode.Acceleration);
    }

    private void UpdateCameraRotation(Vector2 lookInput)
    {
        _xRotation -= lookInput.y * Sensitivity;
        _xRotation = Mathf.Clamp(_xRotation, MinimumAngle, MaximumAngle);

        var localRotation = VirtualCamera.transform.localRotation;

        localRotation = Quaternion.Euler(_xRotation, localRotation.eulerAngles.y, localRotation.eulerAngles.z);
        VirtualCamera.transform.localRotation = localRotation;

        transform.Rotate(lookInput.x * Sensitivity * Vector3.up);
    }

    private void Jump()
    {
        Rigidbody.velocity = new Vector3
        {
            x = Rigidbody.velocity.x,
            z = Rigidbody.velocity.z
        };

        var jumpForce = Vector3.up * Mathf.Sqrt(-2 * Physics.gravity.y * JumpHeight);

        Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);

        OnJump?.Invoke();
    }

    private void CheckForWallRun()
    {
        if (IsGrounded)
        {
            IsWallRight = false;
            IsWallLeft = false;
            return;
        }

        var rightRay = new Ray(transform.position, transform.right);
        var leftRay = new Ray(transform.position, -transform.right);

        var wasWallRight = IsWallRight;
        var wasWallLeft = IsWallLeft;

        IsWallRight = Physics.Raycast(rightRay, out _rightHitInfo, WallCheckDistance) &&
                      (IsWallRight || MovementInput.x > 0);
        IsWallLeft = Physics.Raycast(leftRay, out _leftHitInfo, WallCheckDistance) &&
                     (IsWallLeft || MovementInput.x < 0);

        var boostForce = Vector3.zero;

        if (IsWallRight && !wasWallRight)
            boostForce = -Vector3.Cross(_rightHitInfo.normal, transform.up) * WallRunInitialImpulse;

        if (IsWallLeft && !wasWallLeft)
            boostForce = Vector3.Cross(_leftHitInfo.normal, transform.up) * WallRunInitialImpulse;

        Rigidbody.AddForce(boostForce, ForceMode.VelocityChange);

        if (IsWallRight) Rigidbody.AddForce(-_rightHitInfo.normal, ForceMode.Acceleration);
        if (IsWallLeft) Rigidbody.AddForce(-_leftHitInfo.normal, ForceMode.Acceleration);

        if (!JumpInput)
        {
            CanWallJump = IsWallRunning;
        }
    }

    private void WallJump()
    {
        if (!IsWallRunning) return;

        var sideForce = Vector3.zero;
        if (IsWallRight) sideForce = _rightHitInfo.normal * WallJumpSideForce;
        if (IsWallLeft) sideForce = _leftHitInfo.normal * WallJumpSideForce;

        var jumpForce = Vector3.up * Mathf.Sqrt(WallJumpHeight * -2 * Physics.gravity.y);

        var forwardForce = transform.forward * WallJumpForwardForce;

        var finalForce = jumpForce + sideForce + forwardForce;

        Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, 0f, Rigidbody.velocity.z);

        if (IsWallRight)
        {
            var velocity = Vector3.Project(Rigidbody.velocity, _rightHitInfo.normal);

            Rigidbody.velocity -= velocity;
        }

        if (IsWallLeft)
        {
            var velocity = Vector3.Project(Rigidbody.velocity, _leftHitInfo.normal);

            Rigidbody.velocity -= velocity;
        }

        IsWallRight = false;
        IsWallLeft = false;

        Rigidbody.AddForce(finalForce, ForceMode.VelocityChange);
    }

    private struct InputSnapshot : INetworkSerializable
    {
        public int Tick;
        public Vector2 MovementInput;
        public Vector2 LookInput;
        public bool JumpInput;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref MovementInput);
            serializer.SerializeValue(ref LookInput);
            serializer.SerializeValue(ref JumpInput);
        }
    }

    private struct StateSnapshot : INetworkSerializable
    {
        public int Tick;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref Rotation);
            serializer.SerializeValue(ref Velocity);
        }
    }
}