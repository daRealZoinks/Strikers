using UnityEngine;
using UnityEngine.UIElements;

public class InGameMenu : MonoBehaviour
{
    [SerializeField]
    private ConnectedMenu connectedMenu;

    private void Awake()
    {
        var connectMenu = GetComponent<ConnectMenu>();
        connectMenu.OnGameCreated += OnGameCreated;
        connectMenu.OnGameJoined += OnGameJoined;
    }

    private void OnGameCreated()
    {
        HideConnectMenu();
        ShowConnectedMenu();
    }

    private void OnGameJoined()
    {
        HideConnectMenu();
        ShowConnectedMenu();
    }

    private void HideConnectMenu()
    {
        // Get the root VisualElement
        var uiDocument = GetComponent<UIDocument>();
        uiDocument.enabled = false;
    }

    private void ShowConnectedMenu()
    {
        connectedMenu.gameObject.SetActive(true);
        connectedMenu.Initialize();
        connectedMenu.IsPaused = false;
        connectedMenu.OnGameStarted += OnGameStarted;
    }

    private void OnGameStarted()
    {

    }
}