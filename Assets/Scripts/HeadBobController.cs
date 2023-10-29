using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [field: SerializeField]
    public bool HeadBobEnabled { get; set; } = true;

    [field: SerializeField]
    public CharacterMovementController CharacterMovementController { get; set; }

    [SerializeField]
    private float amplitude = 0.08f;

    [SerializeField]
    private float frequency = 18.5f;

    private Vector3 _originalPosition;

    private void Start()
    {
        _originalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (!HeadBobEnabled) return;

        if (CharacterMovementController.IsGrounded && CharacterMovementController.Rigidbody.velocity.magnitude > 0.1f)
        {
            var speed = Mathf.Clamp01(CharacterMovementController.Rigidbody.velocity.magnitude / CharacterMovementController.MaxSpeed);

            var endPosition = _originalPosition + speed * GetHeadBobPosition();

            transform.localPosition = endPosition;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _originalPosition, Time.deltaTime);
        }
    }

    private Vector3 GetHeadBobPosition()
    {
        var position = new Vector3(
            Mathf.Cos(Time.time * frequency / 2) * amplitude * 2,
            Mathf.Sin(Time.time * frequency) * amplitude,
            0);

        return position;
    }
}
