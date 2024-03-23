using System;
using System.Linq;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
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
    public bool IsWallRight { get; private set; }
    public bool IsWallLeft { get; private set; }
    public bool IsWallRunning => IsWallRight || IsWallLeft;

    public Vector2 MovementInput { get; set; }
    public Vector2 LookInput { get; set; }

    private float _xRotation;
    private RaycastHit _rightHitInfo;
    private RaycastHit _leftHitInfo;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        MovementEnabled = !IsWallRunning;

        Move(MovementInput);

        ApplyAdditionalGravity();

        CheckForWallRun();
    }

    private void Update()
    {
        UpdateCameraRotation(LookInput);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.contacts.Any(contact => Vector3.Dot(contact.normal, Vector3.up) > 0.5f))
        {
            IsGrounded = true;

            OnLanded?.Invoke(Rigidbody.velocity.y);
        }
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

        var horizontalVelocity = new Vector3
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

        Rigidbody.MoveRotation(Rigidbody.rotation * Quaternion.Euler(0f, lookInput.x * Sensitivity, 0f));
    }

    public void Jump()
    {
        GroundJump();
        WallJump();
    }

    private void GroundJump()
    {
        if (!IsGrounded) return;

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

        if (Physics.Raycast(rightRay, out _rightHitInfo, WallCheckDistance))
        {
            IsWallRight = !_rightHitInfo.collider.isTrigger;
        }
        else
        {
            IsWallRight = false;
        }

        if (Physics.Raycast(leftRay, out _leftHitInfo, WallCheckDistance))
        {
            IsWallLeft = !_leftHitInfo.collider.isTrigger;
        }
        else
        {
            IsWallLeft = false;
        }

        var boostForce = Vector3.zero;

        if (IsWallRight && !wasWallRight)
            boostForce = -Vector3.Cross(_rightHitInfo.normal, transform.up) * WallRunInitialImpulse;

        if (IsWallLeft && !wasWallLeft)
            boostForce = Vector3.Cross(_leftHitInfo.normal, transform.up) * WallRunInitialImpulse;

        Rigidbody.AddForce(boostForce, ForceMode.VelocityChange);

        if (IsWallRight) Rigidbody.AddForce(-_rightHitInfo.normal, ForceMode.Acceleration);
        if (IsWallLeft) Rigidbody.AddForce(-_leftHitInfo.normal, ForceMode.Acceleration);
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
}