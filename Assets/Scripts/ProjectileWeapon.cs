using UnityEngine;

public abstract class ProjectileWeapon : Weapon
{
    [SerializeField] protected Projectile projectilePrefab;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float range;

    protected override void Shoot(Vector3 position, Quaternion rotation)
    {
        var projectile = Instantiate(projectilePrefab, position, rotation);
        projectile.Launch(projectileSpeed);

        Destroy(projectile.gameObject, range / projectileSpeed);
    }
}