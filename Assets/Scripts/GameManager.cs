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
    [SerializeField] private string sceneName;

    [SerializeField] private List<Transform> blueSpawnPoints;
    [SerializeField] private List<Transform> orangeSpawnPoints;

    [SerializeField] private GameObject ball;

    [SerializeField] private Transform ballSpawnPoint;

    private readonly NetworkVariable<int> _blueScore = new();
    private readonly NetworkVariable<int> _orangeScore = new();

    private readonly NetworkList<long> _blueSpawnPointsRandomIndices = new();
    private readonly NetworkList<long> _orangeSpawnPointsRandomIndices = new();

    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2f - 50, 10, 100, 30), $"{_blueScore.Value} - {_orangeScore.Value}");
    }

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

        StartCoroutine(ResetGameAfterGoal());
    }

    public void OnOrangeGoal()
    {
        _blueScore.Value++;

        StartCoroutine(ResetGameAfterGoal());
    }

    private IEnumerator ResetGameAfterGoal()
    {
        SetBallActiveClientRpc(false);

        Timer.Instance.timerIsRunning.Value = false;

        yield return new WaitForSeconds(3);

        ResetPlayerClientRpc();

        ResetBall();

        Timer.Instance.timerIsRunning.Value = true;

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
        var ballRigibody = ball.GetComponent<Rigidbody>();

        ballRigibody.velocity = Vector3.zero;
        ballRigibody.angularVelocity = Vector3.zero;
    }

    [ClientRpc]
    private void ResetPlayerClientRpc()
    {
        var playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;

        List<Transform> spawnPoints;
        NetworkList<long> spawnPointsRandomIndices;

        if (playerObject.GetComponent<NetworkPlayerManager>().Team == Team.Blue)
        {
            spawnPoints = blueSpawnPoints;
            spawnPointsRandomIndices = _blueSpawnPointsRandomIndices;
        }
        else
        {
            spawnPoints = orangeSpawnPoints;
            spawnPointsRandomIndices = _orangeSpawnPointsRandomIndices;
        }

        var localClientId = NetworkManager.Singleton.LocalClientId;

        var index = spawnPointsRandomIndices.IndexOf((long)localClientId);

        var spawnPoint = spawnPoints[index];

        playerObject.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        var rigidbody = playerObject.GetComponent<Rigidbody>();

        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
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