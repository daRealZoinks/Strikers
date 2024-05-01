using UnityEngine;

public class BallSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource ballBounceAudioSource;
    [SerializeField] private AudioSource ballRollAudioSource;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        var contacts = other.contacts;

        foreach (var contact in contacts)
        {
            ballBounceAudioSource.transform.position = contact.point;

            var contactDotProduct = Vector3.Dot(_rigidbody.velocity.normalized, contact.normal.normalized);

            ballBounceAudioSource.volume = contactDotProduct * _rigidbody.velocity.magnitude;
            ballBounceAudioSource.Play();
        }
    }

    private void OnCollisionStay(Collision other)
    {
        var contacts = other.contacts;

        foreach (var contact in contacts)
        {
            ballRollAudioSource.transform.position = contact.point;

            ballRollAudioSource.volume = _rigidbody.velocity.magnitude;
            ballRollAudioSource.mute = false;
        }
    }

    private void OnCollisionExit()
    {
        ballRollAudioSource.mute = true;
    }
}