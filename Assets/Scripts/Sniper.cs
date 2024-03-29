﻿using UnityEngine;
using UnityEngine.VFX;

public class Sniper : HitScanWeapon
{
    [field: SerializeField] public float Force { get; set; }

    [SerializeField] private HitScanBulletTrail bulletTrailPrefab;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private VisualEffect muzzleFlash;

    protected override void Shoot()
    {
        var firePointTransform = firePoint.transform;

        muzzleFlash.Play();

        var firePointForward = firePointTransform.forward;
        var firePointPosition = firePointTransform.position;

        var ray = new Ray(firePointPosition, firePointForward);

        var hitPoint = firePointPosition + firePointForward * Range;

        var bulletTrail = Instantiate(bulletTrailPrefab, firePointPosition, Quaternion.identity);

        if (Physics.Raycast(ray, out var hit, Range, layerMask))
        {
            var middlePoint = (firePointPosition + hit.point) / 2f;

            bulletTrail.SetPositions(firePointPosition, middlePoint, hit.point);
            bulletTrail.PlayImpactEffect(hit.point, Quaternion.LookRotation(hit.normal));

            var hitRigidbody = hit.rigidbody;

            if (!hitRigidbody) return;
            var force = firePointTransform.forward * Force;
            hitRigidbody.AddForceAtPosition(force, hit.point, ForceMode.Impulse);
        }
        else
        {
            var middlePoint = (firePointPosition + hit.point) / 2f;

            bulletTrail.SetPositions(firePointPosition, middlePoint, hitPoint);
        }
    }
}