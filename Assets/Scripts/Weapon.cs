using Unity.Netcode;
using UnityEngine;

public abstract class Weapon : NetworkBehaviour
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

    public void NetworkExecuteShoot()
    {
        if (!IsOwner) return;

        ExecuteShoot(firePoint.transform.position, firePoint.transform.rotation);
        ShootServerRpc(firePoint.transform.position, firePoint.transform.rotation);
    }

    private void ExecuteShoot(Vector3 position, Quaternion rotation)
    {
        Shoot(position, rotation);

        weaponAnimator.SetTrigger("Shoot");

        shootAudioSource.Play();

        CurrentAmmo--;
    }

    [ServerRpc]
    private void ShootServerRpc(Vector3 position, Quaternion rotation)
    {
        ShootClientRpc(position, rotation);
    }

    [ClientRpc]
    private void ShootClientRpc(Vector3 position, Quaternion rotation)
    {
        if (!IsOwner) ExecuteShoot(position, rotation);
    }

    public void PlayReloadSequence()
    {
        weaponAnimator.SetTrigger("Reload");
        reloadAudioSource.Play();
    }

    protected abstract void Shoot(Vector3 position, Quaternion rotation);
}