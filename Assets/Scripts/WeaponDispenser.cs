using UnityEngine;

public class WeaponDispenser : MonoBehaviour
{
    public Weapon weapon;

    public void OnTriggerEnter(Collider other)
    {
        var weaponPicker = other.GetComponent<WeaponPicker>();
        if (!weaponPicker) return;
        var gunManager = weaponPicker.gunManager;
        gunManager.GiveWeapon(weapon);
    }
}