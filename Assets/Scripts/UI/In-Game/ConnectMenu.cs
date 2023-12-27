using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ConnectMenu : MonoBehaviour
{
    private VisualElement _connectMenu;

    private TextField _joinGameTextField;

    public event Action OnGameCreated;
    public event Action OnGameJoined;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        _connectMenu = root.Q<VisualElement>("ConnectMenu");
        var createGameButton = root.Q<Button>("createGameButton");
        var joinGameButton = root.Q<Button>("joinGameButton");
        _joinGameTextField = root.Q<TextField>("joinGameTextField");

        createGameButton.clicked += OnCreateGameButtonClicked;
        joinGameButton.clicked += OnJoinGameButtonClicked;
    }

    private void OnCreateGameButtonClicked()
    {
        var gameCreationTask = RelayExample.Instance.CreateGame();

        _connectMenu.SetEnabled(false);

        gameCreationTask.GetAwaiter().OnCompleted(() =>
        {
            if (gameCreationTask.Result)
            {
                OnGameCreated?.Invoke();
            }
            else
            {
                _connectMenu.SetEnabled(true);
            }
        });
    }

    private void OnJoinGameButtonClicked()
    {
        var gameJoiningTask = RelayExample.Instance.JoinGame(_joinGameTextField.value);

        _connectMenu.SetEnabled(false);

        gameJoiningTask.GetAwaiter().OnCompleted(() =>
        {
            if (gameJoiningTask.Result)
            {
                OnGameJoined?.Invoke();
            }
            else
            {
                _connectMenu.SetEnabled(true);
            }
        });
    }
}