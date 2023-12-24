using UnityEngine;
using UnityEngine.UIElements;

public class GraphicsSettingsMenu : MonoBehaviour
{
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var qualityDropdown = root.Q<DropdownField>("qualityDropdown");
        var backButton = root.Q<VisualElement>("GraphicsMenu").Q<Button>("backButton");

        qualityDropdown.RegisterValueChangedCallback(OnQualityChanged);
        backButton.clicked += OnBackClicked;
    }

    private void OnQualityChanged(ChangeEvent<string> evt)
    {
        // Implement your logic to change the quality
        Debug.Log("Quality changed to " + evt.newValue);
    }

    private void OnBackClicked()
    {
        // Get the root VisualElement
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Get the main menu and options menu
        var graphicsMenu = root.Q<VisualElement>("GraphicsMenu");
        var optionsMenu = root.Q<VisualElement>("OptionsMenu");

        // Hide the options menu and show the main menu
        optionsMenu.style.display = DisplayStyle.Flex;
        graphicsMenu.style.display = DisplayStyle.None;
    }
}