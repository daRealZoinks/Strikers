using UnityEngine;

public class NormalProjectile : Projectile
{
    private void OnCollisionEnter()
    {
        AudioSource.PlayClipAtPoint(hitSound.clip, transform.position);
        Destroy(gameObject);
    }
}