using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LateralHeadTiltController : MonoBehaviour
{
    [field: SerializeField]
    public bool LateralHeadTiltEnabled { get; set; } = true;

    [field: SerializeField]
    public CharacterMovementController CharacterMovementController { get; set; }

    [field: SerializeField]
    public CharacterWallRunController CharacterWallRunController { get; set; }

    [SerializeField]
    private float angle = 2f;

    [SerializeField]
    private float wallRunAngle = 5f;

    [SerializeField]
    private float speed = 5f;

    private Quaternion _originalRotation;

    private void Start()
    {
        _originalRotation = transform.localRotation;
    }

    private void Update()
    {
        if (!LateralHeadTiltEnabled) return;

        if (CharacterMovementController.IsGrounded)
        {
            var directionOfMovement = transform.InverseTransformDirection(CharacterMovementController.Rigidbody.velocity);

            var normalizedVelocity = Mathf.Clamp01(CharacterMovementController.Rigidbody.velocity.magnitude / CharacterMovementController.MaxSpeed);

            var rotation = Quaternion.Euler(
                transform.localRotation.eulerAngles.x,
                transform.localRotation.eulerAngles.y,
                -directionOfMovement.normalized.x * angle * normalizedVelocity);

            transform.localRotation = Quaternion.Lerp(transform.localRotation, rotation, Time.deltaTime * speed);
        }
        else if (CharacterWallRunController.IsWallRunning)
        {
            Quaternion rotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, 0);

            if (CharacterWallRunController.IsWallRight)
            {
                rotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, wallRunAngle);
            }

            if (CharacterWallRunController.IsWallLeft)
            {
                rotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, -wallRunAngle);
            }

            transform.localRotation = Quaternion.Lerp(transform.localRotation, rotation, Time.deltaTime * speed);
        }
        else
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, _originalRotation, Time.deltaTime * speed);
        }
    }
}
