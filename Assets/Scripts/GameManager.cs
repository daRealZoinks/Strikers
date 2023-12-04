using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : NetworkBehaviour
{
    [SerializeField]
    private string sceneName;

    private readonly NetworkVariable<int> _blueScore = new();
    private readonly NetworkVariable<int> _orangeScore = new();

    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 50, 10, 100, 30), $"{_blueScore.Value} - {_orangeScore.Value}");
    }

    public void OnBlueGoal()
    {
        _orangeScore.Value++;
        Debug.Log($"Orange score: {_orangeScore.Value}");
        OnBlueGoalClientRpc();
    }

    [ClientRpc]
    private void OnBlueGoalClientRpc()
    {
        Debug.Log($"Orange score: {_orangeScore.Value} ClientRpc");
    }

    public void OnOrangeGoal()
    {
        _blueScore.Value++;
        Debug.Log($"Blue score: {_blueScore.Value}");
        OnOrangeGoalClientRpc();
    }

    [ClientRpc]
    private void OnOrangeGoalClientRpc()
    {
        Debug.Log($"Blue score: {_blueScore.Value} ClientRpc");
    }

    public void OnTimerEnd()
    {
        OnTimerEndClientRpc();
        Debug.Log("Timer ended");
    }

    [ClientRpc]
    private void OnTimerEndClientRpc()
    {
        Debug.Log("Timer ended ClientRpc");

        NetworkManager.SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
    }

#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;

    private void OnValidate()
    {
        if (sceneAsset != null) sceneName = sceneAsset.name;
    }
#endif
}