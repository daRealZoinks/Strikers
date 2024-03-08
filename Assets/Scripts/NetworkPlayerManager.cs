using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkPlayerManager : NetworkBehaviour
{
    [SerializeField] private UnityEvent doOnSpawnIfOwner;
    [SerializeField] private UnityEvent doOnSpawnIfNotOwner;

    private NetworkVariable<Team> _team = new();

    public Team Team
    {
        get => _team.Value;
        private set
        {
            if (!IsServer) return;

            _team.Value = value;

            // set the color & stuff here
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            doOnSpawnIfOwner.Invoke();
        }
        else
        {
            doOnSpawnIfNotOwner.Invoke();
        }

        if (IsServer)
        {
            Team = NetworkManager.Singleton.ConnectedClientsList.Count % 2 == 0 ? Team.Orange : Team.Blue;
        }
    }
}