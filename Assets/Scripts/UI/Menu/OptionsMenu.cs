using UnityEngine;
using UnityEngine.UIElements;

public class OptionsMenu : MonoBehaviour
{
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var graphicsSettingsButton = root.Q<Button>("graphicsSettingsButton");
        var displaySettingsButton = root.Q<Button>("displaySettingsButton");
        var audioSettingsButton = root.Q<Button>("audioSettingsButton");
        var backButton = root.Q<Button>("backButton");

        graphicsSettingsButton.clicked += OnGraphicsSettingsClicked;
        displaySettingsButton.clicked += OnDisplaySettingsClicked;
        audioSettingsButton.clicked += OnAudioSettingsClicked;
        backButton.clicked += OnBackClicked;
    }

    private void OnGraphicsSettingsClicked()
    {
        // Get the root VisualElement
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Get the main menu and options menu
        var graphicsMenu = root.Q<VisualElement>("GraphicsMenu");
        var optionsMenu = root.Q<VisualElement>("OptionsMenu");

        // Hide the options menu and show the main menu
        optionsMenu.style.display = DisplayStyle.None;
        graphicsMenu.style.display = DisplayStyle.Flex;
    }

    private void OnDisplaySettingsClicked()
    {
        // Get the root VisualElement
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Get the main menu and options menu
        var displayMenu = root.Q<VisualElement>("DisplayMenu");
        var optionsMenu = root.Q<VisualElement>("OptionsMenu");

        // Hide the options menu and show the main menu
        optionsMenu.style.display = DisplayStyle.None;
        displayMenu.style.display = DisplayStyle.Flex;
    }

    private void OnAudioSettingsClicked()
    {
        // Get the root VisualElement
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Get the main menu and options menu
        var audioMenu = root.Q<VisualElement>("AudioMenu");
        var optionsMenu = root.Q<VisualElement>("OptionsMenu");

        // Hide the options menu and show the main menu
        optionsMenu.style.display = DisplayStyle.None;
        audioMenu.style.display = DisplayStyle.Flex;
    }

    private void OnBackClicked()
    {
        // Get the root VisualElement
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Get the main menu and options menu
        var mainMenu = root.Q<VisualElement>("MainMenu");
        var optionsMenu = root.Q<VisualElement>("OptionsMenu");

        // Hide the options menu and show the main menu
        optionsMenu.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.Flex;
    }
}