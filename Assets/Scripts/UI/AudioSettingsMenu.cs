using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class AudioSettingsMenu : MonoBehaviour
    {
        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            var masterVolumeSlider = root.Q<Slider>("masterVolumeSlider");
            var sfxVolumeSlider = root.Q<Slider>("sfxVolumeSlider");
            var musicVolumeSlider = root.Q<Slider>("musicVolumeSlider");
            var backButton = root.Q<VisualElement>("AudioMenu").Q<Button>("backButton");

            masterVolumeSlider.RegisterValueChangedCallback(OnMasterVolumeChanged);
            sfxVolumeSlider.RegisterValueChangedCallback(OnSFXVolumeChanged);
            musicVolumeSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
            backButton.clicked += OnBackClicked;
        }

        private void OnMasterVolumeChanged(ChangeEvent<float> evt)
        {
            // Implement your logic to change the master volume
            Debug.Log("Master volume changed to " + evt.newValue);
        }

        private void OnSFXVolumeChanged(ChangeEvent<float> evt)
        {
            // Implement your logic to change the sound effects volume
            Debug.Log("Sound effects volume changed to " + evt.newValue);
        }

        private void OnMusicVolumeChanged(ChangeEvent<float> evt)
        {
            // Implement your logic to change the music volume
            Debug.Log("Music volume changed to " + evt.newValue);
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