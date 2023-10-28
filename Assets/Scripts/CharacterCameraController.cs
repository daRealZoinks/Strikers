using UnityEngine;

public class CharacterCameraController : MonoBehaviour
{
    [SerializeField]
    private float sensitivity = 0.1f;
    [SerializeField]
    private Transform playerTransform;
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

        playerTransform.Rotate(LookInput.x * sensitivity * Vector3.up);
    }
}
