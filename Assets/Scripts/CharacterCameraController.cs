using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCameraController : MonoBehaviour
{
    [SerializeField]
    private float sensitivity = 0.1f;
    [SerializeField]
    private CharacterMovementController _characterMovementController;
    [SerializeField]
    private float minimumAngle = -90f;
    [SerializeField]
    private float maximumAngle = 90f;

    private float _xRotation;

    public Vector2 LookInput { get; set; }

    private void Update()
    {
        _xRotation -= LookInput.y * sensitivity;
        _xRotation = Mathf.Clamp(_xRotation, minimumAngle, maximumAngle);

        var localRotation = transform.localRotation;

        localRotation = Quaternion.Euler(_xRotation, localRotation.eulerAngles.y, localRotation.eulerAngles.z);
        transform.localRotation = localRotation;

        _characterMovementController.transform.Rotate(LookInput.x * sensitivity * Vector3.up);
    }
}
