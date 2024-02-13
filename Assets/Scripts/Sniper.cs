using UnityEngine;

public class Sniper : HitScanWeapon
{
    [field: SerializeField] public float Force { get; set; }

    protected override void Shoot()
    {
        var firePointTransform = firepoint.transform;
        var ray = new Ray(firePointTransform.position, firePointTransform.forward);

        if (!Physics.Raycast(ray, out var hit, range)) return;

        var hitRigidbody = hit.rigidbody;
        if (hitRigidbody != null)
        {
            hitRigidbody.AddForceAtPosition(firePointTransform.forward * Force, hit.point, ForceMode.Impulse);
        }
    }
}