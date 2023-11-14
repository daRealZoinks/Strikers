using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayExample : MonoBehaviour
{
    [field: SerializeField]
    public int MaxPlayers { get; private set; }

    [SerializeField]
    private UnityTransport _transport;
    private string _joinCodeText;

    private async void Awake()
    {
        await Authenticate();
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Create Game"))
        {
            CreateGame();
        }

        _joinCodeText = GUI.TextField(new Rect(120, 10, 100, 30), _joinCodeText);

        if (GUI.Button(new Rect(10, 50, 100, 30), "Join Game"))
        {
            JoinGame();
        }
    }

    private static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateGame()
    {
        var a = await RelayService.Instance.CreateAllocationAsync(MaxPlayers);

        _joinCodeText = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

        _transport.SetHostRelayData(a);

        NetworkManager.Singleton.StartHost();
    }

    public async void JoinGame()
    {
        var a = await RelayService.Instance.JoinAllocationAsync(_joinCodeText);

        _transport.SetClientRelayData(a);

        NetworkManager.Singleton.StartClient();
    }
}

public static class UnityTransportExtensions
{
    public static void SetHostRelayData(this UnityTransport transport, Allocation allocation)
    {
        transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);
    }

    public static void SetClientRelayData(this UnityTransport transport, JoinAllocation joinAllocation)
    {
        transport.SetClientRelayData(joinAllocation.RelayServer.IpV4, (ushort)joinAllocation.RelayServer.Port, joinAllocation.AllocationIdBytes, joinAllocation.Key, joinAllocation.ConnectionData, joinAllocation.HostConnectionData);
    }
}