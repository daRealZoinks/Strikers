using UnityEngine;

public abstract class ProjectileWeapon : Weapon
{
    [SerializeField] protected Projectile projectilePrefab;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float range;

    protected override void Shoot()
    {
        var firepointTransform = firepoint.transform;
        var projectile = Instantiate(projectilePrefab, firepointTransform.position, firepointTransform.rotation);
        projectile.Launch(projectileSpeed);

        Destroy(projectile.gameObject, range / projectileSpeed);
    }
}