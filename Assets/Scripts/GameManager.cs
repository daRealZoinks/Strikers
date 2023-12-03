using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private NetworkVariable<int> _blueScore;
    private NetworkVariable<int> _orangeScore;

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
    private static void OnTimerEndClientRpc()
    {
        Debug.Log("Timer ended ClientRpc");
    }
}