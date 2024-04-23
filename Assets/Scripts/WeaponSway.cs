using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [field: SerializeField] public float Amount { get; set; }
    [field: SerializeField] public float MaxAmount { get; set; }
    [field: SerializeField] public float SmoothAmount { get; set; }

    [SerializeField] private CharacterMovementController characterMovementController;

    private Quaternion _initialRotation;

    private void Start()
    {
        _initialRotation = transform.localRotation;
    }

    private void Update()
    {
        var lookInput = characterMovementController.LookInput;

        var rotationX = Quaternion.AngleAxis(-lookInput.y * Amount, Vector3.right);
        var rotationY = Quaternion.AngleAxis(lookInput.x * Amount, Vector3.up);

        var targetRotation = rotationX * rotationY;

        var interpolationRatio = SmoothAmount * Time.deltaTime;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, interpolationRatio);

        if (lookInput == Vector2.zero)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, _initialRotation, interpolationRatio);
        }
    }
}