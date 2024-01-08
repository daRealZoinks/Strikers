using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class CharacterMovementController : NetworkBehaviour
{
    [field: Header("Movement")]
    [field: SerializeField]
    public float MaxSpeed { get; private set; } = 13f;
    [field: SerializeField]
    public float Acceleration { get; private set; } = 64f;
    [field: SerializeField]
    public float Deceleration { get; private set; } = 128f;
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
    [field: SerializeField]
    public float GravityScale { get; private set; } = 1.5f;

    public event Action OnJump;
    public delegate void OnLandedDelegate(float fallSpeed);
    public event OnLandedDelegate OnLanded;

    public Rigidbody Rigidbody { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool MovementEnabled { get; set; }
    public Vector2 MovementInput { get; set; }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
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
        Move(MovementInput);

        ApplyAditionalGravity();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.contacts.Any(contact => Vector3.Dot(contact.normal, Vector3.up) > 0.5f))
        {
            IsGrounded = true;

            OnLanded?.Invoke(Rigidbody.velocity.y);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        IsGrounded = false;
    }

    private void ApplyAditionalGravity()
    {
        if (!IsGrounded) Rigidbody.AddForce(Physics.gravity * (GravityScale - 1), ForceMode.Acceleration);
    }

    private void Move(Vector2 movementInput)
    {
        var horizontalVelocity = new Vector3()
        {
            x = Rigidbody.velocity.x,
            z = Rigidbody.velocity.z
        };
        var horizontalClampedVelocity = horizontalVelocity.normalized * Mathf.Clamp01(horizontalVelocity.magnitude / MaxSpeed);

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
        if (!IsGrounded) return;

        Rigidbody.velocity = new Vector3()
        {
            x = Rigidbody.velocity.x,
            z = Rigidbody.velocity.z
        };

        var jumpForce = Vector3.up * Mathf.Sqrt(-2 * Physics.gravity.y * JumpHeight);

        Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);

        OnJump?.Invoke();
    }
}
