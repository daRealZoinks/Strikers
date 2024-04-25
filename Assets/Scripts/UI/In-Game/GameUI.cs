using UnityEngine;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
{
    private GameManager _gameManager;
    private Timer _timer;

    private VisualElement crosshair;
    private Label blueTeamScore;
    private Label orangeTeamScore;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();

        _gameManager.OnScoreChanged += UpdateScore;

        _timer = FindObjectOfType<Timer>();

        _timer.OnTimeChanged += UpdateTime;
    }

    private void UpdateScore(int blueScore, int orangeScore)
    {
        blueTeamScore.text = blueScore.ToString();
        orangeTeamScore.text = orangeScore.ToString();
    }

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        blueTeamScore = root.Q<Label>("BlueTeamScore");
        orangeTeamScore = root.Q<Label>("OrangeTeamScore");
    }
}