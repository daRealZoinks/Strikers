using UnityEngine;

public abstract class ProjectileWeapon : Weapon
{
    [SerializeField] protected Projectile projectilePrefab;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float range;

    protected override void Shoot()
    {
        var firePointTransform = firePoint.transform;
        var projectile = Instantiate(projectilePrefab, firePointTransform.position, firePointTransform.rotation);
        projectile.Launch(projectileSpeed);

        Destroy(projectile.gameObject, range / projectileSpeed);
    }
}