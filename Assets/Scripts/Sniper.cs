using UnityEngine;
using UnityEngine.VFX;

public class Sniper : HitScanWeapon
{
    [field: SerializeField] public float Force { get; set; } = 20f;

    [SerializeField] private HitScanBullet bulletPrefab;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private VisualEffect muzzleFlash;

    protected override void Shoot()
    {
        var weaponFirePointTransform = firePoint.transform;

        muzzleFlash.Play();

        var directionOfFire = weaponFirePointTransform.forward;
        var positionOfFire = weaponFirePointTransform.position;

        var rayCastFromWeapon = new Ray(positionOfFire, directionOfFire);

        var maximumHitPoint = positionOfFire + directionOfFire * Range;

        var bulletTrailInstance = Instantiate(bulletPrefab, positionOfFire, Quaternion.identity);

        if (Physics.Raycast(rayCastFromWeapon, out var hitInfo, Range, layerMask))
        {
            var midpointBetweenFireAndHit = (positionOfFire + hitInfo.point) / 2f;

            bulletTrailInstance.SetPositions(positionOfFire, midpointBetweenFireAndHit, hitInfo.point);
            bulletTrailInstance.PlayImpactSequence(hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

            var hitObjectRigidbody = hitInfo.rigidbody;

            if (!hitObjectRigidbody) return;
            var appliedForce = weaponFirePointTransform.forward * Force;
            hitObjectRigidbody.AddForceAtPosition(appliedForce, hitInfo.point, ForceMode.VelocityChange);
        }
        else
        {
            var midpointBetweenFireAndMaxRange = (positionOfFire + maximumHitPoint) / 2f;

            bulletTrailInstance.SetPositions(positionOfFire, midpointBetweenFireAndMaxRange, maximumHitPoint);
        }
    }
}