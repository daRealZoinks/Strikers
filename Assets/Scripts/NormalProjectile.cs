public class NormalProjectile : Projectile
{
    private void OnCollisionEnter()
    {
        Destroy(gameObject);
    }
}