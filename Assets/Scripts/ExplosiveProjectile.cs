using UnityEngine;
using UnityEngine.VFX;

public class ExplosiveProjectile : Projectile
{
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionForce;

    [SerializeField] private VisualEffect explosionEffect;

    private void OnCollisionEnter(Collision other)
    {
        Explode();
    }

    private void Explode()
    {
        var affectedColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var affectedCollider in affectedColliders)
        {
            var hitRigidbody = affectedCollider.attachedRigidbody;
            if (hitRigidbody != null)
            {
                hitRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0,
                    ForceMode.Impulse);
            }
        }

        CreateAndPlayExplosionEffect();
        Destroy(gameObject);
    }

    private void CreateAndPlayExplosionEffect()
    {
        var explosionEffectInstance = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        explosionEffectInstance.Play();
        Destroy(explosionEffectInstance.gameObject, 5f);
    }
}