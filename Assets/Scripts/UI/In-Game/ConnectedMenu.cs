using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ConnectedMenu : MonoBehaviour
{
    [SerializeField] private string joinHostScreenSceneName;

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
        _startMatchButton = root.Q<Button>("startMatchButton");
        var leaveButton = root.Q<Button>("leaveButton");

        copyRoomCodeButton.clicked += OnCopyRoomCodeButtonClicked;
        _startMatchButton.clicked += OnStartMatchButtonClicked;
        leaveButton.clicked += OnLeaveButtonClicked;

        if (NetworkManager.Singleton.IsServer)
        {
            _startMatchButton.style.display = DisplayStyle.Flex;
        }

        _roomCodeLabel.text = RelayExample.Instance.JoinCodeText;

        IsPaused = false;
    }

    private void OnCopyRoomCodeButtonClicked()
    {
        GUIUtility.systemCopyBuffer = RelayExample.Instance.JoinCodeText;
    }

    private void OnStartMatchButtonClicked()
    {
        IsPaused = false;
        StartGame.Instance.StartMatch();
    }

    private void OnLeaveButtonClicked()
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