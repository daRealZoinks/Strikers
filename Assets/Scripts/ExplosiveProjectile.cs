using UnityEngine;

public class ExplosiveProjectile : Projectile
{
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionForce;

    private void OnCollisionEnter()
    {
        Explode();
        Destroy(gameObject);
    }

    private void Explode()
    {
        var colliders = new Collider[10];
        var size = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, colliders);

        for (var i = 0; i < size; i++)
        {
            var hitRigidbody = colliders[i].attachedRigidbody;
            if (hitRigidbody != null)
            {
                hitRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }
}