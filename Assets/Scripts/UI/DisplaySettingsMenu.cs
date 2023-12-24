using UnityEngine;
using UnityEngine.UIElements;

public class DisplaySettingsMenu : MonoBehaviour
{
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var resolutionDropdown = root.Q<DropdownField>("resolutionDropdown");
        var fullscreenDropdown = root.Q<DropdownField>("fullscreenDropdown");
        var backButton = root.Q<VisualElement>("DisplayMenu").Q<Button>("backButton");

        resolutionDropdown.RegisterValueChangedCallback(OnResolutionChanged);
        fullscreenDropdown.RegisterValueChangedCallback(OnFullscreenChanged);
        backButton.clicked += OnBackClicked;
    }

    private void OnResolutionChanged(ChangeEvent<string> evt)
    {
        // Implement your logic to change the resolution
        Debug.Log("Resolution changed to " + evt.newValue);
    }

    private void OnFullscreenChanged(ChangeEvent<string> evt)
    {
        // Implement your logic to change the fullscreen setting
        Debug.Log("Fullscreen changed to " + evt.newValue);
    }

    private void OnBackClicked()
    {
        // Get the root VisualElement
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Get the main menu and options menu
        var displayMenu = root.Q<VisualElement>("DisplayMenu");
        var optionsMenu = root.Q<VisualElement>("OptionsMenu");

        // Hide the options menu and show the main menu
        optionsMenu.style.display = DisplayStyle.Flex;
        displayMenu.style.display = DisplayStyle.None;
    }
}