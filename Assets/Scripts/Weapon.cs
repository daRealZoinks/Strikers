using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected FirePoint firePoint;
    [SerializeField] private Animator weaponAnimator;

    [SerializeField] private AudioSource shootAudioSource;
    [SerializeField] private AudioSource reloadAudioSource;

    [field: SerializeField] public int MaxAmmo { get; set; }
    [field: SerializeField] public float FireRate { get; set; }
    [field: SerializeField] public float ReloadTime { get; set; }

    public int CurrentAmmo { get; set; }
    public bool IsEmpty => CurrentAmmo <= 0;

    public void ExecuteShoot()
    {
        Shoot();

        weaponAnimator.SetTrigger("Shoot");

        shootAudioSource.Play();

        CurrentAmmo--;
    }

    public void PlayReloadSequence()
    {
        weaponAnimator.SetTrigger("Reload");
        reloadAudioSource.Play();
    }

    protected abstract void Shoot();
}