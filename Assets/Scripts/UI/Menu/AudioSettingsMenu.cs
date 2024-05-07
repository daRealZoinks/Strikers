using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class AudioSettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    private const string MasterVolume = "masterVolume";
    private const string SfxVolume = "sfxVolume";
    private const string MusicVolume = "musicVolume";

    private void Start()
    {
        InitializeMixerChannels();
    }

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        InitializeUIElements(root);
    }

    private void InitializeMixerChannels()
    {
        if (PlayerPrefs.HasKey(MasterVolume))
        {
            audioMixer.SetFloat(MasterVolume, PlayerPrefs.GetFloat(MasterVolume));
        }

        if (PlayerPrefs.HasKey(MusicVolume))
        {
            audioMixer.SetFloat(MusicVolume, PlayerPrefs.GetFloat(MusicVolume));
        }

        if (PlayerPrefs.HasKey(SfxVolume))
        {
            audioMixer.SetFloat(SfxVolume, PlayerPrefs.GetFloat(SfxVolume));
        }
    }

    private void InitializeUIElements(VisualElement root)
    {
        var masterVolumeSlider = root.Q<Slider>("masterVolumeSlider");
        var sfxVolumeSlider = root.Q<Slider>("sfxVolumeSlider");
        var musicVolumeSlider = root.Q<Slider>("musicVolumeSlider");
        var backButton = root.Q<VisualElement>("AudioMenu").Q<Button>("backButton");

        InitializeMasterVolumeSlider(masterVolumeSlider);
        InitializeSfxVolumeSlider(sfxVolumeSlider);
        InitializeMusicVolumeSlider(musicVolumeSlider);
        backButton.clicked += OnBackClicked;
    }

    private void InitializeMasterVolumeSlider(Slider masterVolumeSlider)
    {
        masterVolumeSlider.RegisterValueChangedCallback(OnMasterVolumeChanged);

        if (!PlayerPrefs.HasKey(MasterVolume)) return;

        masterVolumeSlider.value = PlayerPrefs.GetFloat(MasterVolume);
    }

    private void InitializeSfxVolumeSlider(Slider sfxVolumeSlider)
    {
        sfxVolumeSlider.RegisterValueChangedCallback(OnSFXVolumeChanged);

        if (!PlayerPrefs.HasKey(SfxVolume)) return;

        sfxVolumeSlider.value = PlayerPrefs.GetFloat(SfxVolume);
    }

    private void InitializeMusicVolumeSlider(Slider musicVolumeSlider)
    {
        musicVolumeSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);

        if (!PlayerPrefs.HasKey(MusicVolume)) return;

        musicVolumeSlider.value = PlayerPrefs.GetFloat(MusicVolume);
    }

    private void OnMasterVolumeChanged(ChangeEvent<float> evt)
    {
        var normalizedValue = Mathf.InverseLerp(-80f, 0f, evt.newValue);
        var volume = normalizedValue > 0.01f ? Mathf.Log10(normalizedValue) * 20 : -80f;

        audioMixer.SetFloat(MasterVolume, volume);
        PlayerPrefs.SetFloat(MasterVolume, evt.newValue);
    }

    private void OnSFXVolumeChanged(ChangeEvent<float> evt)
    {
        var normalizedValue = Mathf.InverseLerp(-80f, 0f, evt.newValue);
        var volume = normalizedValue > 0.01f ? Mathf.Log10(normalizedValue) * 20 : -80f;

        audioMixer.SetFloat(SfxVolume, volume);
        PlayerPrefs.SetFloat(SfxVolume, evt.newValue);
    }

    private void OnMusicVolumeChanged(ChangeEvent<float> evt)
    {
        var normalizedValue = Mathf.InverseLerp(-80f, 0f, evt.newValue);
        var volume = normalizedValue > 0.01f ? Mathf.Log10(normalizedValue) * 20 : -80f;

        audioMixer.SetFloat(MusicVolume, volume);
        PlayerPrefs.SetFloat(MusicVolume, evt.newValue);
    }

    private void OnBackClicked()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var audioMenu = root.Q<VisualElement>("AudioMenu");
        var optionsMenu = root.Q<VisualElement>("OptionsMenu");

        optionsMenu.style.display = DisplayStyle.Flex;
        audioMenu.style.display = DisplayStyle.None;
    }
}