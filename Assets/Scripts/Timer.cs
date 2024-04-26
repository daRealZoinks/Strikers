using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Timer : NetworkBehaviour
{
    [SerializeField] private float minutes = 5;
    [SerializeField] private float seconds = 0;

    public static Timer Instance { get; private set; }

    private readonly NetworkVariable<float> _timeRemaining = new();
    public NetworkVariable<bool> TimerIsRunning { get; } = new();

    public UnityEvent onTimerEnd;

    public event Action<int> OnTimeChanged;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;

        _timeRemaining.Value = minutes * 60 + seconds;
        TimerIsRunning.Value = true;
    }

    public void Update()
    {
        if (!IsServer) return;
        if (!TimerIsRunning.Value) return;

        if (_timeRemaining.Value > 1)
        {
            _timeRemaining.Value -= Time.deltaTime;
            OnTimeChanged?.Invoke((int)_timeRemaining.Value);
        }
        else
        {
            onTimerEnd?.Invoke();
            _timeRemaining.Value = 0;
            TimerIsRunning.Value = false;
        }
    }
}
