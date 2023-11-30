using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StartGame : NetworkBehaviour
{
    [SerializeField]
    private string sceneName;

    private void OnGUI()
    {
        if (!IsServer) return;

        if (GUI.Button(new Rect(10, 90, 100, 30), "Start match"))
            StartMatch();
    }

    private void StartMatch()
    {
        NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;

    private void OnValidate()
    {
        if (sceneAsset != null) sceneName = sceneAsset.name;
    }
#endif
}