using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    public List<Weapon> weapons;
    public Weapon currentWeapon;

    // just for testing
    private void OnGUI()
    {
        var ammo = currentWeapon.CurrentAmmo;
        var maxAmmo = currentWeapon.MaxAmmo;
        GUI.Label(new Rect(10, 10, 100, 20), $"Ammo: {ammo}/{maxAmmo}");
    }

    private void Awake()
    {
        ChangeToPistol();

        currentWeapon.Reload();

        foreach (var weapon in weapons)
        {
            weapon.OnEmptyAmmo += ChangeToPistol;
        }
    }

    private void ChangeToPistol()
    {
        ChangeWeapon(weapons[0]);
    }

    public void Shoot()
    {
        currentWeapon.ExecuteShoot();
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
        currentWeapon.Reload();
    }
}