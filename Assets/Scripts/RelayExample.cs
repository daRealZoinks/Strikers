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
        if (NetworkManager.Singleton != GetComponent<NetworkManager>())
        {
            Destroy(gameObject);
        }

        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        _transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
        Instance = this;
    }

    public static void DeAuthenticate()
    {
        AuthenticationService.Instance.SignOut();
    }

    public async Task<bool> CreateGame()
    {
        try
        {
            var numberOfPlayers = MaxPlayers - 1;

            var a = await RelayService.Instance.CreateAllocationAsync(numberOfPlayers);

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

            JoinCodeText = joinCode;

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