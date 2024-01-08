using Unity.Netcode;
using UnityEngine;

public class NetworkPhysicsSimulation : MonoBehaviour
{
    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnServerStarted += OnStarted;
        }
        else
        {
            NetworkManager.Singleton.OnClientStarted += OnStarted;
        }
    }

    private void OnStarted()
    {
        NetworkManager.Singleton.NetworkTickSystem.Tick += OnNetworkTick;
        Physics.simulationMode = SimulationMode.Script;
    }

    private void OnNetworkTick()
    {
        SimulatePhysics();
    }

    private void SimulatePhysics()
    {
        Physics.Simulate(NetworkManager.Singleton.LocalTime.FixedDeltaTime);
    }
}
