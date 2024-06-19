using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ConnectMenu : MonoBehaviour
{
    private VisualElement _connectMenu;

    private TextField _joinGameTextField;

    public event Action OnBackToMenu;

    [SerializeField] private string menuSceneName;
    [SerializeField] private string lobbySceneName;

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _connectMenu = root.Q<VisualElement>("ConnectMenu");
        var createGameButton = root.Q<Button>("createGameButton");
        var joinGameButton = root.Q<Button>("joinGameButton");
        _joinGameTextField = root.Q<TextField>("joinGameTextField");
        var exitButton = root.Q<Button>("exitButton");

        createGameButton.clicked += OnCreateGameButtonClicked;
        joinGameButton.clicked += OnJoinGameButtonClicked;
        exitButton.clicked += OnExitButtonClicked;
    }

    private void OnCreateGameButtonClicked()
    {
        var gameCreationTask = RelayExample.Instance.CreateGame();

        _connectMenu.SetEnabled(false);

        gameCreationTask.GetAwaiter().OnCompleted(() =>
        {
            if (gameCreationTask.Result)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(lobbySceneName, LoadSceneMode.Single);
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
            if (!gameJoiningTask.Result)
            {
                _connectMenu.SetEnabled(true);
            }
        });
    }

    private void OnExitButtonClicked()
    {
        OnBackToMenu?.Invoke();
        RelayExample.DeAuthenticate();
        SceneManager.LoadScene(menuSceneName, LoadSceneMode.Single);
    }

#if UNITY_EDITOR
    [SerializeField] private SceneAsset menuSceneAsset;
    [SerializeField] private SceneAsset lobbySceneAsset;

    private void OnValidate()
    {
        if (menuSceneAsset != null)
        {
            menuSceneName = menuSceneAsset.name;
        }

        if (lobbySceneAsset != null)
        {
            lobbySceneName = lobbySceneAsset.name;
        }
    }
#endif
}