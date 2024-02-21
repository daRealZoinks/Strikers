using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
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