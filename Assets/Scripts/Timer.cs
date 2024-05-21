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

    private readonly NetworkVariable<int> _timeRemaining = new();
    private float _currentTime;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        _timeRemaining.OnValueChanged += (_, newValue) =>
        {
            OnTimeChanged?.Invoke(newValue);
        };

        if (!IsServer) return;

        _timeRemaining.Value = minutes * 60 + seconds;

        _currentTime = _timeRemaining.Value;
        TimerIsRunning.Value = true;
    }

    public void Update()
    {
        if (!IsServer) return;
        if (!TimerIsRunning.Value) return;

        if (_currentTime > 0)
        {
            _currentTime -= Time.deltaTime;

            if (!(_currentTime < _timeRemaining.Value)) return;

            _timeRemaining.Value = (int)_currentTime;
        }
        else
        {
            onTimerEnd?.Invoke();
            _currentTime = 0;
            TimerIsRunning.Value = false;
        }
    }
}