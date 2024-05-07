#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var playButton = root.Q<Button>("playButton");
        var optionsButton = root.Q<Button>("optionsButton");
        var exitButton = root.Q<Button>("exitButton");

        playButton.clicked += OnPlayClicked;
        optionsButton.clicked += OnOptionsClicked;
        exitButton.clicked += OnExitClicked;
    }

    private void OnPlayClicked()
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    private void OnOptionsClicked()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var mainMenu = root.Q<VisualElement>("MainMenu");
        var optionsMenu = root.Q<VisualElement>("OptionsMenu");

        mainMenu.style.display = DisplayStyle.None;
        optionsMenu.style.display = DisplayStyle.Flex;
    }

    private static void OnExitClicked()
    {
        Application.Quit();
    }

#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;

    private void OnValidate()
    {
        if (sceneAsset) sceneName = sceneAsset.name;
    }
#endif
}