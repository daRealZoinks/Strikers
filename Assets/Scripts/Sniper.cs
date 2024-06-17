using UnityEngine;
using UnityEngine.VFX;

public class Sniper : HitScanWeapon
{
    [field: SerializeField] public float Force { get; set; } = 20f;

    [SerializeField] private HitScanBullet bulletPrefab;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private VisualEffect muzzleFlash;

    protected override void Shoot(Vector3 position, Quaternion rotation)
    {
        muzzleFlash.Play();

        var directionOfFire = rotation * Vector3.forward;

        var rayCastFromWeapon = new Ray(position, directionOfFire);

        var maximumHitPoint = position + directionOfFire * Range;

        var bulletTrailInstance = Instantiate(bulletPrefab, position, rotation);

        if (Physics.Raycast(rayCastFromWeapon, out var hitInfo, Range, layerMask))
        {
            var midpointBetweenFireAndHit = (position + hitInfo.point) / 2f;

            bulletTrailInstance.SetPositions(position, midpointBetweenFireAndHit, hitInfo.point);
            bulletTrailInstance.PlayImpactSequence(hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

            var hitObjectRigidbody = hitInfo.rigidbody;

            if (!hitObjectRigidbody) return;
            var appliedForce = directionOfFire * Force;
            hitObjectRigidbody.AddForceAtPosition(appliedForce, hitInfo.point, ForceMode.VelocityChange);
        }
        else
        {
            var midpointBetweenFireAndMaxRange = (position + maximumHitPoint) / 2f;

            bulletTrailInstance.SetPositions(position, midpointBetweenFireAndMaxRange, maximumHitPoint);
        }
    }
}