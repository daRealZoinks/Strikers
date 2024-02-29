using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

namespace UI
{
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
            audioMixer.SetFloat(MasterVolume, evt.newValue);
            PlayerPrefs.SetFloat(MasterVolume, evt.newValue);
        }

        private void OnSFXVolumeChanged(ChangeEvent<float> evt)
        {
            audioMixer.SetFloat(SfxVolume, evt.newValue);
            PlayerPrefs.SetFloat(SfxVolume, evt.newValue);
        }

        private void OnMusicVolumeChanged(ChangeEvent<float> evt)
        {
            audioMixer.SetFloat(MusicVolume, evt.newValue);
            PlayerPrefs.SetFloat(MusicVolume, evt.newValue);
        }

        private void OnBackClicked()
        {
            // Get the root VisualElement
            var root = GetComponent<UIDocument>().rootVisualElement;

            // Get the main menu and options menu
            var audioMenu = root.Q<VisualElement>("AudioMenu");
            var optionsMenu = root.Q<VisualElement>("OptionsMenu");

            // Hide the options menu and show the main menu
            optionsMenu.style.display = DisplayStyle.Flex;
            audioMenu.style.display = DisplayStyle.None;
        }
    }
}