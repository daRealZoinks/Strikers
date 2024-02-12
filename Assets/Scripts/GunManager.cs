using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    public List<Weapon> weapons;
    public int currentWeaponIndex;

    public void Shoot()
    {
        weapons[currentWeaponIndex].ExecuteShoot();
    }
}