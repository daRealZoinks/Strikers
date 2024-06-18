using UnityEngine;
using UnityEngine.VFX;

public class Sniper : HitScanWeapon
{
    [field: SerializeField] public float Force { get; set; } = 100f;

    [field: Range(0f, 1f)]
    [field: SerializeField]
    public float SpinPercentage { get; set; } = 0.5f;

    [SerializeField] private HitScanBullet bulletPrefab;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private VisualEffect muzzleFlash;

    protected override void Shoot(Vector3 firePosition, Quaternion fireRotation)
    {
        muzzleFlash.Play();

        var directionOfFire = fireRotation * Vector3.forward;

        var rayCastFromWeapon = new Ray(firePosition, directionOfFire);

        var maximumHitPoint = firePosition + directionOfFire * Range;

        var bulletTrailInstance = Instantiate(bulletPrefab, firePosition, Quaternion.identity);

        if (Physics.Raycast(rayCastFromWeapon, out var hitInfo, Range, layerMask))
        {
            var midpointBetweenFireAndHit = (firePosition + hitInfo.point) / 2f;

            bulletTrailInstance.SetPositions(firePosition, midpointBetweenFireAndHit, hitInfo.point);
            bulletTrailInstance.PlayImpactSequence(hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

            var hitObjectRigidbody = hitInfo.rigidbody;

            if (!hitObjectRigidbody) return;

            var appliedForce = Vector3.zero;

            var velocity = hitObjectRigidbody.velocity;
            var dot = Vector3.Dot(velocity.normalized, directionOfFire);

            if (dot < 0)
            {
                appliedForce -= velocity;
            }

            hitObjectRigidbody.velocity = Vector3.zero;

            var force = appliedForce + directionOfFire * (Force * (1 - SpinPercentage));
            hitObjectRigidbody.AddForce(force, ForceMode.Impulse);

            var spinForce = directionOfFire * (Force * SpinPercentage);
            hitObjectRigidbody.AddForceAtPosition(spinForce, hitInfo.point, ForceMode.VelocityChange);
        }
        else
        {
            var midpointBetweenFireAndMaxRange = (firePosition + maximumHitPoint) / 2f;

            bulletTrailInstance.SetPositions(firePosition, midpointBetweenFireAndMaxRange, maximumHitPoint);
        }
    }
}