using UnityEngine;

public abstract class HitScanWeapon : Weapon
{
    [SerializeField] protected float range;

    public float Range
    {
        get => range;
        set => range = value;
    }
}