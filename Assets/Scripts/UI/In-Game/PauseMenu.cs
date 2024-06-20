using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private string joinHostScreenSceneName;

    private Label _roomCodeLabel;
    private Button _leaveToLobbyButton;

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

        var playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;

        if (playerObject)
        {
            playerObject.GetComponent<PlayerInput>().enabled = !isPaused;
        }
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
    }

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _roomCodeLabel = root.Q<Label>("roomCodeLabel");
        var copyRoomCodeButton = root.Q<Button>("copyRoomCodeButton");
        _leaveToLobbyButton = root.Q<Button>("leaveToLobbyButton");
        var exitButton = root.Q<Button>("exitButton");

        copyRoomCodeButton.clicked += OnCopyRoomCodeButtonClicked;
        _leaveToLobbyButton.clicked += OnLeaveToLobbyButtonClicked;
        exitButton.clicked += OnExitButtonOnClicked;

        if (NetworkManager.Singleton.IsServer)
        {
            _leaveToLobbyButton.style.display = DisplayStyle.Flex;
        }

        _roomCodeLabel.text = RelayExample.Instance.JoinCodeText;

        IsPaused = false;
    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsClient) return;
        if (!NetworkManager.Singleton.ShutdownInProgress) return;

        GetComponent<CursorController>().IsCursorLocked = false;
        LeaveLobby();
    }

    private void OnCopyRoomCodeButtonClicked()
    {
        GUIUtility.systemCopyBuffer = RelayExample.Instance.JoinCodeText;
    }

    private void OnLeaveToLobbyButtonClicked()
    {
        IsPaused = false;
        LeaveGame.Instance.LeaveMatch();
    }

    private void OnExitButtonOnClicked()
    {
        LeaveLobby();
    }

    private void LeaveLobby()
    {
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        SceneManager.LoadScene(joinHostScreenSceneName);
    }

#if UNITY_EDITOR
    [SerializeField] private SceneAsset joinHostScreenSceneAsset;

    private void OnValidate()
    {
        if (joinHostScreenSceneAsset != null)
        {
            joinHostScreenSceneName = joinHostScreenSceneAsset.name;
        }
    }
#endif
}