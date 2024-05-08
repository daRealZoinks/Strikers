using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Timer : NetworkBehaviour
{
    [SerializeField] private int minutes = 5;
    [SerializeField] private int seconds = 0;

    public static Timer Instance { get; private set; }

    public UnityEvent onTimerEnd;
    public event Action<int> OnTimeChanged;
    public NetworkVariable<bool> TimerIsRunning { get; } = new();

    private readonly NetworkVariable<float> _timeRemaining = new();
    private int _currentSeconds;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        _timeRemaining.OnValueChanged += (_, newValue) => { OnTimeChanged?.Invoke((int)newValue); };

        if (!IsServer) return;

        _currentSeconds = minutes * 60 + seconds;

        _timeRemaining.Value = _currentSeconds;
        TimerIsRunning.Value = true;
    }

    public void Update()
    {
        if (!IsServer) return;
        if (!TimerIsRunning.Value) return;

        if (_timeRemaining.Value > 0)
        {
            _timeRemaining.Value -= Time.deltaTime;

            if (!(_timeRemaining.Value < _currentSeconds)) return;

            _currentSeconds = (int)_timeRemaining.Value;
            OnTimeChanged?.Invoke(_currentSeconds + 1);
        }
        else
        {
            onTimerEnd?.Invoke();
            _timeRemaining.Value = 0;
            TimerIsRunning.Value = false;
        }
    }
}