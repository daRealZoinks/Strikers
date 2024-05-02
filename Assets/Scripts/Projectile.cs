using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected AudioSource hitSound;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Launch(float projectileSpeed)
    {
        _rigidbody.velocity = transform.forward * projectileSpeed;
    }
}