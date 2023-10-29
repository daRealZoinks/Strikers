using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateralHeadTiltController : MonoBehaviour
{
    [field: SerializeField]
    public bool LateralHeadTiltEnabled { get; set; } = true;

    [field: SerializeField]
    public CharacterMovementController CharacterMovementController { get; set; }

    [SerializeField]
    private float angle = 2f;

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

        if (CharacterMovementController.IsGrounded && CharacterMovementController.Rigidbody.velocity.magnitude > 0.1f)
        {
            var directionOfMovement = transform.InverseTransformDirection(CharacterMovementController.Rigidbody.velocity);

            var rotation = Quaternion.Euler(
                _originalRotation.eulerAngles.x,
                _originalRotation.eulerAngles.y,
                -directionOfMovement.normalized.x * angle * Mathf.Clamp01(CharacterMovementController.Rigidbody.velocity.magnitude / CharacterMovementController.MaxSpeed));

            transform.localRotation = Quaternion.Lerp(transform.localRotation, rotation, Time.deltaTime * speed);
        }
        else
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, _originalRotation, Time.deltaTime * speed);
        }
    }
}
