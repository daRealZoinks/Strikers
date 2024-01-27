using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

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
        Simulate();
    }

    private void Simulate()
    {
        InputSystem.Update();
        Physics.Simulate(NetworkManager.Singleton.LocalTime.FixedDeltaTime);
    }
}
