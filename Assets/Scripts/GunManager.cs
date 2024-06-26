using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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

        var firePosition = currentWeapon.firePoint.transform.position;
        var fireRotation = currentWeapon.firePoint.transform.rotation;

        ShootServerRpc(firePosition, fireRotation);
        currentWeapon.ExecuteShoot(firePosition, fireRotation);

        StartCoroutine(ResetCanShootCoroutine());
    }

    private IEnumerator ResetCanShootCoroutine()
    {
        yield return new WaitForSeconds(currentWeapon.FireRate);

        ResetCanShoot();
    }

    [ServerRpc]
    private void ShootServerRpc(Vector3 firePosition, Quaternion fireRotation)
    {
        ShootClientRpc(firePosition, fireRotation);
    }

    [ClientRpc]
    private void ShootClientRpc(Vector3 firePosition, Quaternion fireRotation)
    {
        if (!IsOwner) currentWeapon.ExecuteShoot(firePosition, fireRotation);
    }

    public void GiveWeapon(Weapon weapon)
    {
        var weaponInList = weapons.Find(w => w.GetType() == weapon.GetType());

        ChangeWeapon(weaponInList);
    }

    private void ChangeWeapon(Weapon weapon)
    {
        StopReloadingAndCoroutines();

        weapons.ForEach(w => w.gameObject.SetActive(false));
        weapon.gameObject.SetActive(true);
        currentWeapon = weapon;

        Reload();

        OnWeaponChanged?.Invoke();
    }

    private void StopReloadingAndCoroutines()
    {
        StopAllCoroutines();
        _isReloading = false;
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

        StartCoroutine(ExecuteReloading());
        currentWeapon.PlayReloadSequence();
    }

    private IEnumerator ExecuteReloading()
    {
        yield return new WaitForSeconds(currentWeapon.ReloadTime);

        _isReloading = false;
        currentWeapon.CurrentAmmo = currentWeapon.MaxAmmo;
        ResetCanShoot();
    }
}