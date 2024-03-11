using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected FirePoint firePoint;
    [SerializeField] private Animator weaponAnimator;

    [field: SerializeField] public int MaxAmmo { get; set; }
    [field: SerializeField] public float FireRate { get; set; }
    [field: SerializeField] public float ReloadTime { get; set; }

    public int CurrentAmmo { get; set; }

    public event Action OnEmptyAmmo;

    public void ExecuteShoot()
    {
        Shoot();

        weaponAnimator.SetTrigger("Shoot");

        CurrentAmmo--;
        if (CurrentAmmo <= 0)
        {
            OnEmptyAmmo?.Invoke();
        }
    }

    public void PlayReloadAnimation()
    {
        weaponAnimator.SetTrigger("Reload");
    }

    protected abstract void Shoot();
}