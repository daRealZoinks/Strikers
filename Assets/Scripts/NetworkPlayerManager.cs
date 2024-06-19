using System.Collections.Generic;
using TMPro;
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

    [SerializeField] private TextMeshPro playerNameText;

    private readonly NetworkList<char> _playerName = new(readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        _playerName.OnListChanged += _ =>
        {
            if (!IsOwner)
            {
                playerNameText.text = ReadPlayerName();
            }
        };
    }

    public override void OnNetworkSpawn()
    {
        team.OnValueChanged += OnValueChanged;

        (IsOwner ? doOnSpawnIfOwner : doOnSpawnIfNotOwner).Invoke();

        if (IsServer)
        {
            team.Value = NetworkManager.Singleton.ConnectedClientsList.Count % 2 == 0 ? Team.Orange : Team.Blue;
        }

        InvokeOnTeamChange(team.Value);

        if (IsOwner)
        {
            WritePlayerName();
        }
        else
        {
            playerNameText.text = ReadPlayerName();
        }
    }

    private string ReadPlayerName()
    {
        var playerName = string.Empty;

        foreach (var letter in _playerName)
        {
            playerName += letter;
        }

        return playerName;
    }

    private void WritePlayerName()
    {
        var playerName = PlayerPrefs.GetString("PlayerName", "Player");

        foreach (var letter in playerName)
        {
            _playerName.Add(letter);
        }
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