using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ConnectMenu : MonoBehaviour
{
    private TextField _joinGameTextField;
    private Button _createGameButton;
    private Button _joinGameButton;

    public event Action OnGameCreated;
    public event Action OnGameJoined;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _createGameButton = root.Q<Button>("createGameButton");
        _joinGameButton = root.Q<Button>("joinGameButton");
        _joinGameTextField = root.Q<TextField>("joinGameTextField");

        _createGameButton.clicked += OnCreateGameButtonClicked;
        _joinGameButton.clicked += OnJoinGameButtonClicked;
    }

    private void OnCreateGameButtonClicked()
    {
        var gameCreationTask = RelayExample.Instance.CreateGame();

        _createGameButton.SetEnabled(false);
        _joinGameButton.SetEnabled(false);
        _joinGameTextField.SetEnabled(false);

        gameCreationTask.GetAwaiter().OnCompleted(() =>
        {
            OnGameCreated?.Invoke();
        });
    }

    private void OnJoinGameButtonClicked()
    {
        var gameJoiningTask = RelayExample.Instance.JoinGame(_joinGameTextField.value);

        _createGameButton.SetEnabled(false);
        _joinGameButton.SetEnabled(false);
        _joinGameTextField.SetEnabled(false);

        gameJoiningTask.GetAwaiter().OnCompleted(() =>
        {
            OnGameJoined?.Invoke();
        });
    }
}