using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public NetworkVariable<float> timeRemaining = new();
    public NetworkVariable<bool> timerIsRunning = new();

    public UnityEvent onTimerEnd;

    public void Start()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        timerIsRunning.Value = true;
        timeRemaining.Value = 60 * 5;
    }

    public void Update()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (!timerIsRunning.Value) return;

        if (timeRemaining.Value > 0)
        {
            timeRemaining.Value -= Time.deltaTime;
        }
        else
        {
            onTimerEnd?.Invoke();
            timeRemaining.Value = 0;
            timerIsRunning.Value = false;
        }
    }
}
