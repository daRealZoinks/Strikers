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
    [field: SerializeField] public int MaxPlayers { get; private set; }

    public static RelayExample Instance { get; private set; }

    private UnityTransport _transport;

    public string JoinCodeText { get; private set; }

    private async void Awake()
    {
        await Authenticate();
        _transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
        Instance = this;
    }

    private async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public void Deauthenticate()
    {
        AuthenticationService.Instance.SignOut();
    }

    public async Task<bool> CreateGame()
    {
        try
        {
            var a = await RelayService.Instance.CreateAllocationAsync(MaxPlayers);

            JoinCodeText = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

            _transport.SetHostRelayData(a);

            NetworkManager.Singleton.StartHost();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> JoinGame(string joinCode)
    {
        try
        {
            var a = await RelayService.Instance.JoinAllocationAsync(joinCode);

            JoinCodeText = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

            _transport.SetClientRelayData(a);

            NetworkManager.Singleton.StartClient();

            return true;
        }
        catch
        {
            return false;
        }
    }
}

public static class UnityTransportExtensions
{
    public static void SetHostRelayData(this UnityTransport transport, Allocation allocation)
    {
        transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);
    }

    public static void SetClientRelayData(this UnityTransport transport, JoinAllocation joinAllocation)
    {
        transport.SetClientRelayData(joinAllocation.RelayServer.IpV4, (ushort)joinAllocation.RelayServer.Port,
            joinAllocation.AllocationIdBytes, joinAllocation.Key, joinAllocation.ConnectionData,
            joinAllocation.HostConnectionData);
    }
}