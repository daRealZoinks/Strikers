using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : NetworkBehaviour
{
    [SerializeField] private string lobbySceneName;

    [SerializeField] private string joinHostScreenSceneName;

    [SerializeField] private List<Transform> blueSpawnPoints;
    [SerializeField] private List<Transform> orangeSpawnPoints;

    [SerializeField] private GameObject ball;

    [SerializeField] private Transform ballSpawnPoint;

    private readonly NetworkVariable<int> _blueScore = new();
    private readonly NetworkVariable<int> _orangeScore = new();

    private readonly NetworkList<long> _blueSpawnPointsRandomIndices = new();
    private readonly NetworkList<long> _orangeSpawnPointsRandomIndices = new();

    public event Action<int, int> OnScoreChanged;

    public override void OnNetworkSpawn()
    {
        _blueScore.OnValueChanged += (_, newBlueScore) => { OnScoreChanged?.Invoke(newBlueScore, _orangeScore.Value); };

        _orangeScore.OnValueChanged += (_, newOrangeScore) =>
        {
            OnScoreChanged?.Invoke(_blueScore.Value, newOrangeScore);
        };

        if (!IsServer) return;

        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObject = player.PlayerObject;

            if (playerObject.GetComponent<NetworkPlayerManager>().team.Value == Team.Blue)
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

        RandomizeSpawnPointIndices(_blueSpawnPointsRandomIndices);
        RandomizeSpawnPointIndices(_orangeSpawnPointsRandomIndices);

        ResetPlayerClientRpc();
    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsClient) return;

        if (NetworkManager.Singleton.ShutdownInProgress)
        {
            LeaveGame();
        }
    }

    private void LeaveGame()
    {
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        SceneManager.LoadScene(joinHostScreenSceneName);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

        _ = ResetGameAsync();
    }

    public void OnOrangeGoal()
    {
        _blueScore.Value++;

        _ = ResetGameAsync();
    }

    private async Task ResetGameAsync()
    {
        SetBallActiveClientRpc(false);

        Timer.Instance.TimerIsRunning.Value = false;

        await Task.Delay(3000);

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

        switch (playerObject.GetComponent<NetworkPlayerManager>().team.Value)
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

        var playerRigidbody = playerObject.GetComponent<Rigidbody>();

        playerRigidbody.position = spawnPoint.position;
        playerRigidbody.rotation = spawnPoint.rotation;
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;

        playerObject.GetComponentInChildren<GunManager>().ChangeToPistol();
    }

    public void OnTimerEnd()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.SceneManager.LoadScene(lobbySceneName, LoadSceneMode.Single);
        }
    }

#if UNITY_EDITOR
    [SerializeField] private SceneAsset lobbySceneAsset;

    [SerializeField] private SceneAsset joinHostScreenSceneAsset;

    private void OnValidate()
    {
        if (lobbySceneAsset) lobbySceneName = lobbySceneAsset.name;

        if (joinHostScreenSceneAsset) joinHostScreenSceneName = joinHostScreenSceneAsset.name;
    }
#endif
}