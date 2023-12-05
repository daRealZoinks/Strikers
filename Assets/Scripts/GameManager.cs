using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : NetworkBehaviour
{
    [SerializeField]
    private string sceneName;

    [SerializeField]
    private List<Transform> spawnPoints;

    [SerializeField]
    private GameObject ball;

    [SerializeField]
    private Transform ballSpawnPoint;

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

        if (IsServer)
        {
            StartCoroutine(ResetGameAfterGoal());
        }
    }

    public void OnOrangeGoal()
    {
        _blueScore.Value++;
        Debug.Log($"Blue score: {_blueScore.Value}");

        if (IsServer)
        {
            StartCoroutine(ResetGameAfterGoal());
        }
    }

    [ClientRpc]
    private void OnGoalScoredClientRpc()
    {
        if (!IsServer)
        {
            StartCoroutine(ResetGameAfterGoal());
        }
    }

    private IEnumerator ResetGameAfterGoal()
    {
        // Disable the ball
        ball.SetActive(false);

        // Only the server should modify the game state
        if (IsServer)
        {
            // Stop the timer
            Timer.Instance.timerIsRunning.Value = false;

            // Wait for a few seconds
            yield return new WaitForSeconds(3);

            // Place all players at their spawn points
            var clients = NetworkManager.Singleton.ConnectedClientsList;
            for (int i = 0; i < clients.Count; i++)
            {
                var player = clients[i].PlayerObject;
                player.transform.SetPositionAndRotation(spawnPoints[i % spawnPoints.Count].position, spawnPoints[i % spawnPoints.Count].rotation);
            }

            // Place the ball at the center of the field and set its velocity to zero
            ball.transform.position = ballSpawnPoint.position;
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;

            // Start the timer
            Timer.Instance.timerIsRunning.Value = true;
        }
        else
        {
            // Clients should only wait for the server to update the game state
            yield return new WaitForSeconds(3);
        }

        // Enable the ball
        ball.SetActive(true);
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