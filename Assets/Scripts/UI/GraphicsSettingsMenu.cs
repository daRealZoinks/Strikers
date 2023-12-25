using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphicsSettingsMenu : MonoBehaviour
{
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var qualityDropdown = root.Q<DropdownField>("qualityDropdown");
        var backButton = root.Q<VisualElement>("GraphicsMenu").Q<Button>("backButton");

        InitializeQualityDropdown(qualityDropdown);

        qualityDropdown.RegisterValueChangedCallback(OnQualityChanged);
        backButton.clicked += OnBackClicked;
    }

    private static void InitializeQualityDropdown(DropdownField qualityDropdown)
    {
        var qualityOptions = new List<string>(QualitySettings.names);
        qualityDropdown.choices = qualityOptions;
    }

    private static void OnQualityChanged(ChangeEvent<string> evt)
    {
        var qualityIndex = QualitySettings.names.ToList().IndexOf(evt.newValue);

        QualitySettings.SetQualityLevel(qualityIndex);
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