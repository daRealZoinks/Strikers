using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class NetworkGunManager : MonoBehaviour
{
    private GunManager _gunManager;

    private readonly NetworkVariable<List<bool>> _activeWeapons =
        new(new List<bool>(), writePerm: NetworkVariableWritePermission.Owner,
            readPerm: NetworkVariableReadPermission.Everyone);

    private void Awake()
    {
        _gunManager = GetComponent<GunManager>();

        _gunManager.OnWeaponChanged += OnGunManagerOnWeaponChanged;

        _activeWeapons.OnValueChanged += OnActiveWeaponsChanged;
    }

    private void OnActiveWeaponsChanged(List<bool> previousValue, List<bool> newValue)
    {
        for (var i = 0; i < newValue.Count; i++)
        {
            if (newValue[i] != previousValue[i])
            {
                _gunManager.weapons[i].gameObject.SetActive(newValue[i]);
            }
        }
    }

    private void OnGunManagerOnWeaponChanged()
    {
        var activeWeapons = _gunManager.weapons.Select(weapon => weapon.gameObject.activeSelf).ToList();

        _activeWeapons.Value = activeWeapons;
    }
}