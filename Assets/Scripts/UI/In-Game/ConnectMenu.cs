using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ConnectMenu : MonoBehaviour
{
    private VisualElement _connectMenu;

    private TextField _joinGameTextField;

    public event Action OnGameCreated;
    public event Action OnGameJoined;

    [SerializeField] private string sceneName;

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

    private void OnExitButtonClicked()
    {
        RelayExample.Instance.Deauthenticate();
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;

    private void OnValidate()
    {
        if (sceneAsset != null) sceneName = sceneAsset.name;
    }
#endif
}