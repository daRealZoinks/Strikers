using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkPlayerManager : NetworkBehaviour
{
    [SerializeField]
    private UnityEvent _doOnSpawnIfOwner;
    [SerializeField]
    private UnityEvent _doOnSpawnIfNotOwner;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _doOnSpawnIfOwner.Invoke();
        }
        else
        {
            _doOnSpawnIfNotOwner.Invoke();
        }
    }
}
