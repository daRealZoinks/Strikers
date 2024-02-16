using System;
using Unity.Netcode;
using UnityEngine;

public abstract class Weapon : NetworkBehaviour
{
    [SerializeField] protected FirePoint firePoint;
    [field: SerializeField] public int MaxAmmo { get; set; }
    [field: SerializeField] protected float FireRate { get; set; }
    [field: SerializeField] protected float ReloadTime { get; set; }

    public int CurrentAmmo { get; private set; }

    public event Action OnEmptyAmmo;

    private bool _canShoot;
    private bool _isReloading;

    public void Reload()
    {
        if (_isReloading) return;

        CurrentAmmo = 0;
        _isReloading = true;
        Invoke(nameof(ExecuteReloading), ReloadTime);
    }

    public void ExecuteShoot()
    {
        if (!_canShoot || _isReloading) return;

        _canShoot = false;

        if (IsOwner)
        {
            ShootServerRpc();

            Shoot();
        }

        Invoke(nameof(ResetCanShoot), FireRate);
        CurrentAmmo--;

        if (CurrentAmmo > 0) return;

        OnEmptyAmmo?.Invoke();
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        ShootClientRpc();
    }

    [ClientRpc]
    private void ShootClientRpc()
    {
        if (!IsOwner) Shoot();
    }

    protected abstract void Shoot();

    private void ResetCanShoot()
    {
        _canShoot = true;
    }

    private void ExecuteReloading()
    {
        _isReloading = false;
        CurrentAmmo = MaxAmmo;
        ResetCanShoot();
    }
}