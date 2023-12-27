using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ConnectedMenu : MonoBehaviour
{
    private Label _roomCodeLabel;
    private Button _startMatchButton;

    private bool _isPaused;

    public bool IsPaused
    {
        get => _isPaused;
        set
        {
            _isPaused = value;
            UpdatePauseState(_isPaused);
        }
    }

    private void UpdatePauseState(bool isPaused)
    {
        GetComponent<CursorController>().IsCursorLocked = !isPaused;
        GetComponent<UIDocument>().rootVisualElement.style.display = isPaused ? DisplayStyle.Flex : DisplayStyle.None;
        if (NetworkManager.Singleton.LocalClient.PlayerObject)
        {
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerInput>().enabled = !isPaused;
        }
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
    }

    public event Action OnGameStarted;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _roomCodeLabel = root.Q<Label>("roomCodeLabel");
        _startMatchButton = root.Q<Button>("startMatchButton");

        var copyRoomCodeButton = root.Q<Button>("copyRoomCodeButton");

        _startMatchButton.clicked += OnStartMatchButtonClicked;

        copyRoomCodeButton.clicked += OnCopyRoomCodeButtonClicked;
    }

    public void Initialize()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            _startMatchButton.style.display = DisplayStyle.Flex;
        }

        _roomCodeLabel.text = RelayExample.Instance.JoinCodeText;
    }

    private static void OnCopyRoomCodeButtonClicked()
    {
        GUIUtility.systemCopyBuffer = RelayExample.Instance.JoinCodeText;
    }

    private void OnStartMatchButtonClicked()
    {
        IsPaused = false;
        StartGame.Instance.StartMatch();
        OnGameStarted?.Invoke();
    }
}