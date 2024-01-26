using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class CharacterMovementController : NetworkBehaviour
{
    [field: Header("Movement")]
    [field: SerializeField]
    public float MaxSpeed { get; private set; } = 13f;

    [field: SerializeField] public float Acceleration { get; private set; } = 64f;
    [field: SerializeField] public float Deceleration { get; private set; } = 128f;

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

    public event Action OnJump;

    public delegate void OnLandedDelegate(float fallSpeed);

    public event OnLandedDelegate OnLanded;

    public Rigidbody Rigidbody { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool MovementEnabled { get; set; }
    public Vector2 MovementInput { get; set; }
    public bool JumpInput { get; set; }
    public bool CanJump { get; set; }

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
            JumpInput = JumpInput
        };

        inputSnapshots[NetworkManager.NetworkTickSystem.LocalTime.Tick % BufferLength] = inputSnapshot;

        if (!IsServer)
        {
            if (IsOwner)
            {
                stateSnapshots[NetworkManager.NetworkTickSystem.LocalTime.Tick % BufferLength] =
                    PerformMovement(inputSnapshot);

                PerformMovementServerRpc(inputSnapshot);
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
        Move(inputSnapshot.MovementInput);

        ApplyAdditionalGravity();

        if (inputSnapshot.JumpInput)
        {
            Jump();
            CanJump = false;
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
        if (collision.contacts.Any(contact => Vector3.Dot(contact.normal, Vector3.up) > 0.5f))
        {
            IsGrounded = true;

            if (!JumpInput)
            {
                CanJump = true;
            }

            OnLanded?.Invoke(Rigidbody.velocity.y);
        }
    }

    private void OnCollisionExit(Collision collision)
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

    public void Jump()
    {
        if (!CanJump) return;

        Rigidbody.velocity = new Vector3()
        {
            x = Rigidbody.velocity.x,
            z = Rigidbody.velocity.z
        };

        var jumpForce = Vector3.up * Mathf.Sqrt(-2 * Physics.gravity.y * JumpHeight);

        Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);

        OnJump?.Invoke();
    }

    private struct InputSnapshot : INetworkSerializable
    {
        public int Tick;
        public Vector2 MovementInput;
        public bool JumpInput;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref MovementInput);
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