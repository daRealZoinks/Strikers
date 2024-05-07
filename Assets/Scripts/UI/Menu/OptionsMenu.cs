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
        var root = GetComponent<UIDocument>().rootVisualElement;

        var graphicsMenu = root.Q<VisualElement>("GraphicsMenu");
        var optionsMenu = root.Q<VisualElement>("OptionsMenu");

        optionsMenu.style.display = DisplayStyle.None;
        graphicsMenu.style.display = DisplayStyle.Flex;
    }

    private void OnDisplaySettingsClicked()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var displayMenu = root.Q<VisualElement>("DisplayMenu");
        var optionsMenu = root.Q<VisualElement>("OptionsMenu");

        optionsMenu.style.display = DisplayStyle.None;
        displayMenu.style.display = DisplayStyle.Flex;
    }

    private void OnAudioSettingsClicked()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var audioMenu = root.Q<VisualElement>("AudioMenu");
        var optionsMenu = root.Q<VisualElement>("OptionsMenu");

        optionsMenu.style.display = DisplayStyle.None;
        audioMenu.style.display = DisplayStyle.Flex;
    }

    private void OnBackClicked()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var mainMenu = root.Q<VisualElement>("MainMenu");
        var optionsMenu = root.Q<VisualElement>("OptionsMenu");

        optionsMenu.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.Flex;
    }
}