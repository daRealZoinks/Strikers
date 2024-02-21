using Unity.Netcode;
using UnityEngine;

public class NetworkGunManager : NetworkBehaviour
{
    private GunManager _gunManager;

    [SerializeField] private NetworkVariable<int> weaponIndex = new(
        writePerm: NetworkVariableWritePermission.Owner,
        readPerm: NetworkVariableReadPermission.Everyone);

    private void Awake()
    {
        _gunManager = GetComponent<GunManager>();
    }

    public override void OnNetworkSpawn()
    {
        weaponIndex.OnValueChanged += OnWeaponIndexOnValueChanged;

        _gunManager.OnWeaponChanged += OnWeaponChanged;

        if (IsOwner)
        {
            weaponIndex.Value = _gunManager.weapons.IndexOf(_gunManager.currentWeapon);
        }
        else
        {
            _gunManager.weapons.ForEach(w => w.gameObject.SetActive(false));
            _gunManager.weapons[weaponIndex.Value].gameObject.SetActive(true);

            _gunManager.currentWeapon = _gunManager.weapons[weaponIndex.Value];
        }
    }

    private void OnWeaponIndexOnValueChanged(int previousValue, int newValue)
    {
        if (IsOwner) return;

        if (newValue < 0 || newValue >= _gunManager.weapons.Count) return;

        _gunManager.weapons.ForEach(w => w.gameObject.SetActive(false));
        _gunManager.weapons[newValue].gameObject.SetActive(true);

        _gunManager.currentWeapon = _gunManager.weapons[newValue];
    }

    public override void OnNetworkDespawn()
    {
        weaponIndex.OnValueChanged -= OnWeaponIndexOnValueChanged;

        _gunManager.OnWeaponChanged -= OnWeaponChanged;
    }

    private void OnWeaponChanged()
    {
        if (!IsOwner) return;

        weaponIndex.Value = _gunManager.weapons.IndexOf(_gunManager.currentWeapon);
    }
}