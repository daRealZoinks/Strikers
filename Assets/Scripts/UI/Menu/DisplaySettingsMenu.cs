using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class DisplaySettingsMenu : MonoBehaviour
{
    private readonly SettingsManager _settingsManager = new();
    private Settings _settings;

    private void OnEnable()
    {
#if !UNITY_EDITOR
        _settings = _settingsManager.LoadSettings();
        SetScreenResolution(_settings);
#endif
        var root = GetComponent<UIDocument>().rootVisualElement;
        InitializeUIElements(root);
    }

    private static void SetScreenResolution(Settings settings)
    {
        Screen.SetResolution(settings.Resolution.width, settings.Resolution.height, settings.FullscreenMode,
            settings.Resolution.refreshRateRatio);
    }

    private void InitializeUIElements(VisualElement root)
    {
        var resolutionDropdown = root.Q<DropdownField>("resolutionDropdown");
        var refreshRateDropdown = root.Q<DropdownField>("refreshRateDropdown");
        refreshRateDropdown.SetEnabled(Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen);
        var fullscreenDropdown = root.Q<DropdownField>("fullscreenDropdown");
        var backButton = root.Q<VisualElement>("DisplayMenu").Q<Button>("backButton");

        InitializeResolutionAndRefreshRateDropdown(resolutionDropdown, refreshRateDropdown);
        InitializeFullscreenDropdown(fullscreenDropdown);
        RegisterValueChangedCallbacks(resolutionDropdown, refreshRateDropdown, fullscreenDropdown);
        backButton.clicked += OnBackClicked;
    }

    private void RegisterValueChangedCallbacks(DropdownField resolutionDropdown, DropdownField refreshRateDropdown,
        DropdownField fullscreenDropdown)
    {
        resolutionDropdown.RegisterValueChangedCallback(OnResolutionChanged);
        refreshRateDropdown.RegisterValueChangedCallback(OnRefreshRateChanged);
        fullscreenDropdown.RegisterValueChangedCallback(OnFullscreenChanged);
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
        resolutionDropdown.index = resolutionOptions.IndexOf(Screen.width + "x" +
                                                             Screen.height);
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

    private void OnResolutionChanged(ChangeEvent<string> evt)
    {
        var resolutionString = evt.newValue.Split('x');
        var width = int.Parse(resolutionString[0]);
        var height = int.Parse(resolutionString[1]);
        Screen.SetResolution(width, height, Screen.fullScreenMode, Screen.currentResolution.refreshRateRatio);

        var resolution = new Resolution
        {
            width = width,
            height = height,
            refreshRateRatio = Screen.currentResolution.refreshRateRatio
        };

#if !UNITY_EDITOR
        _settings.Resolution = resolution;
        _settingsManager.SaveSettings(_settings);
#endif
    }

    private void OnRefreshRateChanged(ChangeEvent<string> evt)
    {
        var refreshRate = uint.Parse(evt.newValue.Replace(" Hz", ""));
        var refreshRateRatio = Screen.currentResolution.refreshRateRatio;
        refreshRateRatio.denominator = 1;
        refreshRateRatio.numerator = refreshRate;

        Screen.SetResolution(Screen.width, Screen.height, Screen.fullScreenMode,
            refreshRateRatio);

        var resolution = new Resolution
        {
            width = Screen.width,
            height = Screen.height,
            refreshRateRatio = refreshRateRatio
        };

#if !UNITY_EDITOR
        _settings.Resolution = resolution;
        _settingsManager.SaveSettings(_settings);
#endif
    }

    private void OnFullscreenChanged(ChangeEvent<string> evt)
    {
        var fullScreenMode = evt.newValue switch
        {
            "Exclusive Fullscreen" => FullScreenMode.ExclusiveFullScreen,
            "Fullscreen Window" => FullScreenMode.FullScreenWindow,
            "Maximized Window" => FullScreenMode.MaximizedWindow,
            "Windowed" => FullScreenMode.Windowed,
            _ => Screen.fullScreenMode
        };

        Screen.SetResolution(Screen.width, Screen.height, fullScreenMode,
            Screen.currentResolution.refreshRateRatio);

        var root = GetComponent<UIDocument>().rootVisualElement;
        var refreshRateDropdown = root.Q<DropdownField>("refreshRateDropdown");
        refreshRateDropdown.SetEnabled(fullScreenMode == FullScreenMode.ExclusiveFullScreen);

#if !UNITY_EDITOR
        _settings.FullscreenMode = fullScreenMode;
        _settingsManager.SaveSettings(_settings);
#endif
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

[Serializable]
public class Settings
{
    public Resolution Resolution { get; set; }

    public FullScreenMode FullscreenMode { get; set; }
    // Add other settings as needed
}

public class SettingsManager
{
    private readonly string _settingsFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                                "/My Games/Strikers/";

    private readonly string _settingsFileName = "settings.xml";

    public Settings LoadSettings()
    {
        if (!File.Exists(_settingsFilePath + _settingsFileName))
        {
            var settings = new Settings
            {
                Resolution = Screen.currentResolution,
                FullscreenMode = Screen.fullScreenMode
            };
            SaveSettings(settings);
            return settings;
        }

        var serializer = new XmlSerializer(typeof(Settings));
        using var stream = new FileStream(_settingsFilePath + _settingsFileName, FileMode.Open);
        return (Settings)serializer.Deserialize(stream);
    }

    public void SaveSettings(Settings settings)
    {
        var serializer = new XmlSerializer(typeof(Settings));

        if (!Directory.Exists(_settingsFilePath))
        {
            Directory.CreateDirectory(_settingsFilePath);
        }

        using var stream =
            XmlWriter.Create(_settingsFilePath + _settingsFileName, new XmlWriterSettings { Indent = true });
        serializer.Serialize(stream, settings);
    }
}