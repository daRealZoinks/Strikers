using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected Firepoint firepoint;
    [SerializeField] protected int maxAmmo;
    [SerializeField] protected float fireRate;

    private bool _canShoot;
    private int _currentAmmo;

    private void Awake()
    {
        _currentAmmo = maxAmmo;
        _canShoot = true;
    }

    public void ExecuteShoot()
    {
        if (!_canShoot) return;

        Shoot();

        _canShoot = false;
        Invoke(nameof(ResetCanShoot), fireRate);
        _currentAmmo--;

        if (_currentAmmo > 0) return;
        _currentAmmo = 0;
        _canShoot = false;
    }

    protected abstract void Shoot();

    private void ResetCanShoot()
    {
        _canShoot = true;
    }
}