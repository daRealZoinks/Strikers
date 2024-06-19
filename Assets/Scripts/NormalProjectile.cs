using UnityEngine;

public class NormalProjectile : Projectile
{
    private void OnCollisionEnter()
    {
        var audioSource = Instantiate(hitSound, transform.position, Quaternion.identity);
        Destroy(audioSource.gameObject, hitSound.clip.length);
        Destroy(gameObject);
    }
}