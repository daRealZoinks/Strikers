using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkPlayerManager : NetworkBehaviour
{
    public NetworkVariable<Team> team = new();

    [SerializeField] private UnityEvent doOnSpawnIfOwner;
    [SerializeField] private UnityEvent doOnSpawnIfNotOwner;

    [SerializeField] private UnityEvent doOnIfBlueTeam;
    [SerializeField] private UnityEvent doOnIfOrangeTeam;

    [SerializeField] private Material blueMaterial;
    [SerializeField] private Material orangeMaterial;

    [SerializeField] private List<Renderer> renderers;

    public override void OnNetworkSpawn()
    {
        team.OnValueChanged += OnValueChanged;

        (IsOwner ? doOnSpawnIfOwner : doOnSpawnIfNotOwner).Invoke();

        if (IsServer)
        {
            team.Value = NetworkManager.Singleton.ConnectedClientsList.Count % 2 == 0 ? Team.Orange : Team.Blue;
        }

        InvokeOnTeamChange(team.Value);
    }

    private void OnValueChanged(Team previousValue, Team newValue)
    {
        InvokeOnTeamChange(newValue);
    }

    private void InvokeOnTeamChange(Team value)
    {
        (value == Team.Blue ? doOnIfBlueTeam : doOnIfOrangeTeam).Invoke();
    }

    public void ChangeRenderersMaterialToBlue()
    {
        foreach (var rendererItem in renderers)
        {
            rendererItem.material = blueMaterial;
        }
    }

    public void ChangeRenderersMaterialToOrange()
    {
        foreach (var rendererItem in renderers)
        {
            rendererItem.material = orangeMaterial;
        }
    }
}