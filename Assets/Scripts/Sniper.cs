using UnityEngine;

public class Sniper : HitScanWeapon
{
    [field: SerializeField] public float Force { get; set; }

    [SerializeField] private HitScanBulletTrail bulletTrailPrefab;

    [SerializeField] private LayerMask layerMask;

    protected override void Shoot()
    {
        var firePointTransform = firePoint.transform;

        var forward = firePointTransform.forward;
        var position = firePointTransform.position;

        var ray = new Ray(position, forward);

        var hitPoint = position + forward * Range;

        var bulletTrail = Instantiate(bulletTrailPrefab, position, Quaternion.identity);

        if (Physics.Raycast(ray, out var hit, Range, layerMask))
        {
            bulletTrail.SetPositions(firePointTransform.position, hit.point);
            bulletTrail.PlayImpactEffect(hit.point, Quaternion.LookRotation(hit.normal));

            var hitRigidbody = hit.rigidbody;

            if (!hitRigidbody) return;
            var force = firePointTransform.forward * Force;
            hitRigidbody.AddForceAtPosition(force, hit.point, ForceMode.Impulse);
        }
        else
        {
            bulletTrail.SetPositions(firePointTransform.position, hitPoint);
        }
    }
}