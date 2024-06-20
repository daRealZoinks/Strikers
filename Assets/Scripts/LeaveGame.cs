using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LeaveGame : NetworkBehaviour
{
    [SerializeField] private string sceneName;

    public static LeaveGame Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void LeaveMatch()
    {
        UnpauseAllClientsClientRpc();

        NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    [ClientRpc]
    private void UnpauseAllClientsClientRpc()
    {
        if (IsServer) return;

        GetComponent<PauseMenu>().IsPaused = false;
    }

#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;

    private void OnValidate()
    {
        if (sceneAsset != null) sceneName = sceneAsset.name;
    }
#endif
}