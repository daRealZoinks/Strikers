using System;
using System.Collections.Generic;
using Unity.Netcode;

public class GunManager : NetworkBehaviour
{
    public List<Weapon> weapons;
    public Weapon currentWeapon;

    private bool _canShoot;
    private bool _isReloading;

    public Action OnWeaponChanged { get; set; }

    private void Awake()
    {
        ChangeToPistol();

        Reload();
    }

    public void ChangeToPistol()
    {
        ChangeWeapon(weapons[0]);
    }

    public void Shoot()
    {
        if (!_canShoot || _isReloading) return;

        _canShoot = false;

        if (!IsOwner) return;

        ShootServerRpc();
        currentWeapon.ExecuteShoot();
        Invoke(nameof(ResetCanShoot), currentWeapon.FireRate);
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        ShootClientRpc();
    }

    [ClientRpc]
    private void ShootClientRpc()
    {
        if (!IsOwner) currentWeapon.ExecuteShoot();
    }

    public void GiveWeapon(Weapon weapon)
    {
        var weaponInList = weapons.Find(w => w.GetType() == weapon.GetType());

        ChangeWeapon(weaponInList);
    }

    private void ChangeWeapon(Weapon weapon)
    {
        weapons.ForEach(w => w.gameObject.SetActive(false));
        weapon.gameObject.SetActive(true);
        currentWeapon = weapon;
        Reload();

        OnWeaponChanged?.Invoke();
    }

    private void ResetCanShoot()
    {
        _canShoot = true;
        if (currentWeapon.IsEmpty)
        {
            ChangeToPistol();
        }
    }

    private void Reload()
    {
        if (_isReloading) return;

        currentWeapon.CurrentAmmo = 0;
        _isReloading = true;
        Invoke(nameof(ExecuteReloading), currentWeapon.ReloadTime);
        currentWeapon.PlayReloadSequence();
    }

    private void ExecuteReloading()
    {
        _isReloading = false;
        currentWeapon.CurrentAmmo = currentWeapon.MaxAmmo;
        ResetCanShoot();
    }
}