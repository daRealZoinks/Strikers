using UnityEngine;

public class WeaponOffset : MonoBehaviour
{
    [SerializeField] private CharacterMovementController characterMovementController;

    [field: SerializeField] public float Amount { get; set; }
    [field: SerializeField] public float MaxAmount { get; set; }
    [field: SerializeField] public float SmoothAmount { get; set; }

    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = transform.localPosition;
    }

    private void Update()
    {
        var playerVelocity = characterMovementController.Rigidbody.velocity;
        var playerDirection = characterMovementController.transform.forward;

        var relativeVelocity = Vector3.Dot(playerVelocity, playerDirection);

        var velocityNormalized = -relativeVelocity * Amount * playerDirection;

        velocityNormalized.x = Mathf.Clamp(velocityNormalized.x, -MaxAmount, MaxAmount);
        velocityNormalized.y = Mathf.Clamp(velocityNormalized.y, -MaxAmount, MaxAmount);
        velocityNormalized.z = Mathf.Clamp(velocityNormalized.z, -MaxAmount, MaxAmount);

        var finalPosition = velocityNormalized + _initialPosition;

        var interpolationRate = Time.deltaTime * SmoothAmount;

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition, interpolationRate);

        if (playerVelocity.magnitude < 0.5f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _initialPosition, interpolationRate);
        }
    }
}