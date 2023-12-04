using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Timer : NetworkBehaviour
{
    public NetworkVariable<float> timeRemaining = new();
    public NetworkVariable<bool> timerIsRunning = new();

    public UnityEvent onTimerEnd;

    // ongui
    private void OnGUI()
    {
        var seconds = (int)timeRemaining.Value % 60;
        var minutes = (int)timeRemaining.Value / 60;
        GUI.Label(new Rect(Screen.width / 2 - 50, 40, 100, 30), $"{minutes:00}:{seconds:00}");

        // when the last 10 seconds are reached, the timer will show on the center of the screen with a big font
        if (timeRemaining.Value <= 10)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 100), $"{timeRemaining.Value}");
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;

        timeRemaining.Value = 60 * 5;
        timerIsRunning.Value = true;
    }

    public void Update()
    {
        if (!IsServer) return;
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
