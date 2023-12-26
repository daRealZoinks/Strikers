using UnityEngine;
using UnityEngine.UIElements;

public class InGameMenu : MonoBehaviour
{
    private void Awake()
    {
        var connectScript = GetComponent<ConnectMenu>();
        connectScript.OnGameCreated += OnGameCreated;
        connectScript.OnGameJoined += OnGameJoined;
    }

    private void OnGameJoined()
    {
        HideConnectMenu();
    }

    private void OnGameCreated()
    {
        HideConnectMenu();
    }

    private void HideConnectMenu()
    {
        // Get the root VisualElement
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Get the main menu and options menu
        var connectMenu = root.Q<VisualElement>("Connect");

        // Hide the options menu and show the main menu
        connectMenu.style.display = DisplayStyle.None;
    }
}