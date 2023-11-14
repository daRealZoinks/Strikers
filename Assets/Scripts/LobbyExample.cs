using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;

public class LobbyExample : MonoBehaviour
{
    [SerializeField]
    private UnityTransport _transport;

    public string JoinCodeKey { get; private set; } = "j";

    private Lobby _connectedLobby;
    private string _playerId;

    public void OnGUI()
    {
        if (_connectedLobby != null)
        {
            return;
        }

        if (GUI.Button(new Rect(10, 10, 100, 30), "Create or Join Lobby"))
        {
            CreateOrJoinLobby();
        }
    }

    public async void CreateOrJoinLobby()
    {
        await Authenticate();

        _connectedLobby = await QuickJoinLobby() ?? await CreateLobby();
    }

    private async Task Authenticate()
    {
        var initializationOptions = new InitializationOptions();

        initializationOptions.SetProfile($"{UnityEngine.Random.Range(0, 1000000)}");

        await UnityServices.InitializeAsync(initializationOptions);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        _playerId = AuthenticationService.Instance.PlayerId;
    }

    private async Task<Lobby> QuickJoinLobby()
    {
        try
        {
            var lobby = await Lobbies.Instance.QuickJoinLobbyAsync();

            var joinAllocator = await RelayService.Instance.JoinAllocationAsync(lobby.LobbyCode);

            _transport.SetClientRelayData(joinAllocator);

            NetworkManager.Singleton.StartClient();
            return lobby;
        }
        catch
        {
            return null;
        }
    }

    private async Task<Lobby> CreateLobby()
    {
        try
        {
            const int maxPlayers = 100;

            var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            var createLobbyOptions = new CreateLobbyOptions()
            {
                Data = new Dictionary<string, DataObject>()
                {
                    { JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
                }
            };

            var lobby = await Lobbies.Instance.CreateLobbyAsync("gerghrthrxrtcn", maxPlayers, createLobbyOptions);

            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

            _transport.SetHostRelayData(allocation);

            NetworkManager.Singleton.StartHost();
            return lobby;
        }
        catch
        {
            return null;
        }
    }

    private static IEnumerator HeartbeatLobbyCoroutine(string lobbyId, int waitTimeSeconds)
    {
        var delay = new WaitForSeconds(waitTimeSeconds);

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    private void OnDestroy()
    {
        try
        {
            StopAllCoroutines();

            if (_connectedLobby != null)
            {
                if (_connectedLobby.HostId == _playerId)
                {
                    Lobbies.Instance.DeleteLobbyAsync(_connectedLobby.Id);
                }
                else
                {
                    Lobbies.Instance.RemovePlayerAsync(_connectedLobby.Id, _playerId);
                }
            }
        }
        catch { }
    }
}
