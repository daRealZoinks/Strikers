using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StartGame : NetworkBehaviour
{
    [SerializeField] private string sceneName;

    public static StartGame Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void StartMatch()
    {
        UnpauseAllClientsClientRpc();

        NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    [ClientRpc]
    private void UnpauseAllClientsClientRpc()
    {
        if (IsServer) return;

        GetComponent<ConnectedMenu>().IsPaused = false;
    }

#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;

    private void OnValidate()
    {
        if (sceneAsset != null) sceneName = sceneAsset.name;
    }
#endif
}