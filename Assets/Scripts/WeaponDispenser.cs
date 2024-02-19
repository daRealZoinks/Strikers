using Unity.Netcode;
using UnityEngine;

public class WeaponDispenser : NetworkBehaviour
{
    [SerializeField] private Weapon weapon;
    [field: SerializeField] public float RespawnTime { get; set; }

    private bool _isReadyToDispense;
    private float _timeToRespawn;

    private void Start()
    {
        _isReadyToDispense = true;
        _timeToRespawn = RespawnTime;
    }

    private void Update()
    {
        if (!IsServer) return;

        if (_isReadyToDispense) return;

        _timeToRespawn -= Time.deltaTime;

        if (!(_timeToRespawn <= 0)) return;
        ResetDispenser();
        ResetDispenserClientRpc();
    }

    private void ResetDispenser()
    {
        _isReadyToDispense = true;
        weapon.gameObject.SetActive(true);
        _timeToRespawn = RespawnTime;
    }

    [ClientRpc]
    private void ResetDispenserClientRpc()
    {
        _isReadyToDispense = true;
        weapon.gameObject.SetActive(true);
    }

    public void OnTriggerStay(Collider other)
    {
        if (!_isReadyToDispense) return;

        WeaponPicker weaponPicker;

        if (IsServer)
        {
            weaponPicker = other.GetComponent<WeaponPicker>();

            if (!weaponPicker) return;
            var gunManager = weaponPicker.gunManager;
            gunManager.GiveWeapon(weapon);

            weapon.gameObject.SetActive(false);
            _isReadyToDispense = false;
        }
        else
        {
            if (!IsOwner) return;

            weaponPicker = NetworkManager.LocalClient.PlayerObject.gameObject.GetComponent<WeaponPicker>();

            if (!weaponPicker) return;
            var gunManager = weaponPicker.gunManager;
            gunManager.GiveWeapon(weapon);

            DisableWeaponServerRpc();
        }
    }

    [ServerRpc]
    private void DisableWeaponServerRpc()
    {
        weapon.gameObject.SetActive(false);
        _isReadyToDispense = false;

        DisableWeaponClientRpc();
    }

    [ClientRpc]
    private void DisableWeaponClientRpc()
    {
        weapon.gameObject.SetActive(false);
        _isReadyToDispense = false;
    }
}