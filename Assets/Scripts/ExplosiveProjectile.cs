﻿using UnityEngine;
using UnityEngine.VFX;

public class ExplosiveProjectile : Projectile
{
    [SerializeField] private float explosionRadius = 10f;
    [SerializeField] private float explosionForce = 1000f;
    [SerializeField] private float upwardsModifier = 1f;

    [SerializeField] private VisualEffect explosionEffect;

    private void OnCollisionEnter()
    {
        var audioSource = Instantiate(hitSound, transform.position, Quaternion.identity);
        Destroy(audioSource.gameObject, hitSound.clip.length);

        Explode();
    }

    private void Explode()
    {
        var detectedColliders = new Collider[100];
        var size = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, detectedColliders);

        for (var i = 0; i < size; i++)
        {
            var hitRigidbody = detectedColliders[i].attachedRigidbody;

            if (hitRigidbody)
            {
                hitRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier,
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