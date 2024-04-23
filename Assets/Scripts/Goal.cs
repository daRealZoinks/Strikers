using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour
{
    public UnityEvent onGoal;

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Ball")) return;

        if (!NetworkManager.Singleton.IsServer) return;

        onGoal?.Invoke();
    }
}