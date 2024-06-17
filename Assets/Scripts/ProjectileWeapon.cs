using UnityEngine;

public abstract class ProjectileWeapon : Weapon
{
    [SerializeField] protected Projectile projectilePrefab;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float range;

    protected override void Shoot(Vector3 firePosition, Quaternion fireRotation)
    {
        var projectile = Instantiate(projectilePrefab, firePosition, fireRotation);
        projectile.Launch(projectileSpeed);

        Destroy(projectile.gameObject, range / projectileSpeed);
    }
}