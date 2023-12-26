using UnityEngine;
using UnityEngine.UIElements;

public class InGameMenu : MonoBehaviour
{
    private ConnectedMenu _connectedMenu;

    private void Awake()
    {
        var connectMenu = GetComponent<ConnectMenu>();
        _connectedMenu = GetComponent<ConnectedMenu>();
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
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Get the main menu and options menu
        var connectMenu = root.Q<VisualElement>("ConnectMenu");

        // Hide the options menu and show the main menu
        connectMenu.style.display = DisplayStyle.None;
    }

    private void ShowConnectedMenu()
    {
        // Get the root VisualElement
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Get the main menu and options menu
        var connectedMenu = root.Q<VisualElement>("ConnectedMenu");

        // Hide the options menu and show the main menu
        connectedMenu.style.display = DisplayStyle.Flex;

        _connectedMenu.Initialize();
        _connectedMenu.OnGameStarted += OnGameStarted;
    }

    private void OnGameStarted()
    {

    }
}