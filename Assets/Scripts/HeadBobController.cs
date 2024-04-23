using UnityEngine;
using UnityEngine.Events;

public class HeadBobController : MonoBehaviour
{
    [field: SerializeField] public bool HeadBobEnabled { get; set; } = true;

    [field: SerializeField] public CharacterMovementController CharacterMovementController { get; set; }

    [SerializeField] private float amplitude = 0.08f;

    [SerializeField] private float frequency = 18.5f;

    [SerializeField] private UnityEvent onStepTaken;
    private bool _isStepTaken;

    private Vector3 _originalPosition;
    private Vector3 _endPosition;

    private void Start()
    {
        _originalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (CharacterMovementController.IsGrounded || CharacterMovementController.IsWallRunning)
        {
            var speed = Mathf.Clamp01(CharacterMovementController.Rigidbody.velocity.magnitude /
                                      CharacterMovementController.MaxSpeed);

            var position = _originalPosition + speed * GetHeadBobPosition();

            if (_endPosition.y < position.y && !_isStepTaken &&
                CharacterMovementController.MovementInput.magnitude > 0 &&
                speed > 0.7f)
            {
                onStepTaken?.Invoke();
                _isStepTaken = true;
            }
            else if (_endPosition.y > position.y)
            {
                _isStepTaken = false;
            }

            _endPosition = position;
        }
        else
        {
            _endPosition = Vector3.Lerp(transform.localPosition, _originalPosition, Time.deltaTime);
        }

        if (!HeadBobEnabled) return;
        transform.localPosition = _endPosition;
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