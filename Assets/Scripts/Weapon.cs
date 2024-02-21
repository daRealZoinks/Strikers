using System;
using Unity.Netcode;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected FirePoint firePoint;
    [field: SerializeField] public int MaxAmmo { get; set; }
    [field: SerializeField] public float FireRate { get; set; }
    [field: SerializeField] public float ReloadTime { get; set; }

    public int CurrentAmmo { get; set; }

    public event Action OnEmptyAmmo;

    public void ExecuteShoot()
    {
        Shoot();

        CurrentAmmo--;
        if (CurrentAmmo <= 0)
        {
            OnEmptyAmmo?.Invoke();
        }
    }

    protected abstract void Shoot();
}