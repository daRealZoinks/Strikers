using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StartGame : MonoBehaviour
{
    [SerializeField]
    private string sceneName;

    private void OnGUI()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (GUI.Button(new Rect(10, 90, 100, 30), "Start match"))
            StartMatch();
    }

    private void StartMatch()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;

    private void OnValidate()
    {
        if (sceneAsset != null) sceneName = sceneAsset.name;
    }
#endif
}