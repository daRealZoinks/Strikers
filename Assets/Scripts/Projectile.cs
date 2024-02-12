using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter()
    {
        Destroy(gameObject);
    }

    public void Launch(float projectileSpeed)
    {
        _rigidbody.velocity = transform.forward * projectileSpeed;
    }
}