using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DisplaySettingsMenu : MonoBehaviour
{
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var resolutionDropdown = root.Q<DropdownField>("resolutionDropdown");
        var refreshRateDropdown = root.Q<DropdownField>("refreshRateDropdown");
        var fullscreenDropdown = root.Q<DropdownField>("fullscreenDropdown");
        var backButton = root.Q<VisualElement>("DisplayMenu").Q<Button>("backButton");

        InitializeResolutionAndRefreshRateDropdown(resolutionDropdown, refreshRateDropdown);
        InitializeFullscreenDropdown(fullscreenDropdown);

        resolutionDropdown.RegisterValueChangedCallback(OnResolutionChanged);
        refreshRateDropdown.RegisterValueChangedCallback(OnRefreshRateChanged);
        fullscreenDropdown.RegisterValueChangedCallback(OnFullscreenChanged);
        backButton.clicked += OnBackClicked;
    }

    private static void InitializeResolutionAndRefreshRateDropdown(DropdownField resolutionDropdown,
        DropdownField refreshRateDropdown)
    {
        var resolutions = Screen.resolutions;
        Array.Reverse(resolutions);
        var resolutionOptions = new List<string>();
        var refreshRateOptions = new List<string>();

        foreach (var resolution in resolutions)
        {
            // Add resolution to the list
            resolutionOptions.Add(resolution.width + "x" + resolution.height);

            // Add refresh rate to the list
            refreshRateOptions.Add($"{Math.Round(resolution.refreshRateRatio.value, 2)} Hz");
        }

        resolutionDropdown.choices = resolutionOptions;
        resolutionDropdown.index = resolutionOptions.IndexOf(Screen.currentResolution.width + "x" +
                                                             Screen.currentResolution.height);
        refreshRateDropdown.choices = refreshRateOptions;
        refreshRateDropdown.index =
            refreshRateOptions.IndexOf($"{Math.Round(Screen.currentResolution.refreshRateRatio.value, 2)} Hz");
    }

    private static void InitializeFullscreenDropdown(DropdownField fullscreenDropdown)
    {
        var fullscreenOptions = new List<string>
        {
#if UNITY_STANDALONE_WIN
            "Exclusive Fullscreen",
#endif
            "Fullscreen Window",
#if UNITY_STANDALONE_OSX
            "Maximized Window",
#endif
            "Windowed"
        };

        fullscreenDropdown.choices = fullscreenOptions;
        fullscreenDropdown.index = fullscreenOptions.IndexOf(Screen.fullScreenMode switch
        {
            FullScreenMode.ExclusiveFullScreen => "Exclusive Fullscreen",
            FullScreenMode.FullScreenWindow => "Fullscreen Window",
            FullScreenMode.MaximizedWindow => "Maximized Window",
            FullScreenMode.Windowed => "Windowed",
            _ => throw new ArgumentOutOfRangeException()
        });
    }

    private static void OnResolutionChanged(ChangeEvent<string> evt)
    {
        var resolution = evt.newValue.Split('x');
        var width = int.Parse(resolution[0]);
        var height = int.Parse(resolution[1]);
        Screen.SetResolution(width, height, Screen.fullScreenMode, Screen.currentResolution.refreshRateRatio);
    }

    private static void OnRefreshRateChanged(ChangeEvent<string> evt)
    {
        var refreshRate = uint.Parse(evt.newValue.Replace(" Hz", ""));
        var refreshRateRatio = Screen.currentResolution.refreshRateRatio;
        refreshRateRatio.denominator = 1;
        refreshRateRatio.numerator = refreshRate;

        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreenMode,
            refreshRateRatio);
    }

    private static void OnFullscreenChanged(ChangeEvent<string> evt)
    {
        Screen.fullScreenMode = evt.newValue switch
        {
            "Exclusive Fullscreen" => FullScreenMode.ExclusiveFullScreen,
            "Fullscreen Window" => FullScreenMode.FullScreenWindow,
            "Maximized Window" => FullScreenMode.MaximizedWindow,
            "Windowed" => FullScreenMode.Windowed,
            _ => Screen.fullScreenMode
        };
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