using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : NetworkBehaviour
{
    [SerializeField] private string sceneName;

    [SerializeField] private List<Transform> blueSpawnPoints;
    [SerializeField] private List<Transform> orangeSpawnPoints;

    [SerializeField] private GameObject ball;

    [SerializeField] private Transform ballSpawnPoint;

    private readonly NetworkVariable<int> _blueScore = new();
    private readonly NetworkVariable<int> _orangeScore = new();

    private readonly NetworkList<long> _blueSpawnPointsRandomIndices = new();
    private readonly NetworkList<long> _orangeSpawnPointsRandomIndices = new();

    public event Action<int, int> OnScoreChanged;

    private void Start()
    {
        if (!IsServer) return;

        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObject = player.PlayerObject;

            if (playerObject.GetComponent<NetworkPlayerManager>().Team == Team.Blue)
            {
                _blueSpawnPointsRandomIndices.Add((long)playerObject.OwnerClientId);
            }
            else
            {
                _orangeSpawnPointsRandomIndices.Add((long)playerObject.OwnerClientId);
            }
        }

        for (var i = 0; i <= blueSpawnPoints.Count - _blueSpawnPointsRandomIndices.Count; i++)
        {
            _blueSpawnPointsRandomIndices.Add(-1);
        }

        for (var i = 0; i <= orangeSpawnPoints.Count - _orangeSpawnPointsRandomIndices.Count; i++)
        {
            _orangeSpawnPointsRandomIndices.Add(-1);
        }

        ResetPlayerClientRpc();

        RandomizeSpawnPointIndices(_blueSpawnPointsRandomIndices);
        RandomizeSpawnPointIndices(_orangeSpawnPointsRandomIndices);
    }

    private static void RandomizeSpawnPointIndices(NetworkList<long> spawnPointsRandomIndices)
    {
        for (var i = 0; i < spawnPointsRandomIndices.Count; i++)
        {
            var temp = spawnPointsRandomIndices[i];
            var randomIndex = Random.Range(i, spawnPointsRandomIndices.Count);
            spawnPointsRandomIndices[i] = spawnPointsRandomIndices[randomIndex];
            spawnPointsRandomIndices[randomIndex] = temp;
        }
    }

    public void OnBlueGoal()
    {
        _orangeScore.Value++;

        OnScoreChanged?.Invoke(_blueScore.Value, _orangeScore.Value);

        StartCoroutine(ResetGame());
    }

    public void OnOrangeGoal()
    {
        _blueScore.Value++;

        OnScoreChanged?.Invoke(_blueScore.Value, _orangeScore.Value);

        StartCoroutine(ResetGame());
    }

    private IEnumerator ResetGame()
    {
        SetBallActiveClientRpc(false);

        Timer.Instance.TimerIsRunning.Value = false;

        yield return new WaitForSeconds(3);

        ResetPlayerClientRpc();

        ResetBall();

        Timer.Instance.TimerIsRunning.Value = true;

        RandomizeSpawnPointIndices(_blueSpawnPointsRandomIndices);
        RandomizeSpawnPointIndices(_orangeSpawnPointsRandomIndices);

        SetBallActiveClientRpc(true);
    }

    [ClientRpc]
    private void SetBallActiveClientRpc(bool active)
    {
        ball.SetActive(active);
    }

    private void ResetBall()
    {
        ball.transform.position = ballSpawnPoint.position;
        var ballRigidbody = ball.GetComponent<Rigidbody>();

        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
    }

    [ClientRpc]
    private void ResetPlayerClientRpc()
    {
        var playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;

        List<Transform> spawnPoints;
        NetworkList<long> spawnPointsRandomIndices;

        switch (playerObject.GetComponent<NetworkPlayerManager>().Team)
        {
            case Team.Blue:
                spawnPoints = blueSpawnPoints;
                spawnPointsRandomIndices = _blueSpawnPointsRandomIndices;
                break;
            case Team.Orange:
                spawnPoints = orangeSpawnPoints;
                spawnPointsRandomIndices = _orangeSpawnPointsRandomIndices;
                break;
            default:
                return;
        }

        var localClientId = NetworkManager.Singleton.LocalClientId;

        var index = spawnPointsRandomIndices.IndexOf((long)localClientId);

        var spawnPoint = spawnPoints[index];

        playerObject.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        var playerRigidbody = playerObject.GetComponent<Rigidbody>();

        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;

        playerObject.GetComponentInChildren<GunManager>().ChangeToPistol();
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