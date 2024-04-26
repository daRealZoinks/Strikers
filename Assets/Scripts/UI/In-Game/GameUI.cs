using UnityEngine;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
{
    private GameManager _gameManager;
    private Timer _timer;

    private Label _blueTeamScoreLabel;
    private Label _orangeTeamScoreLabel;
    private Label _timerLabel;
    private Label _lastSecondsLabel;


    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();

        _gameManager.OnScoreChanged += UpdateScore;

        _timer = FindObjectOfType<Timer>();

        _timer.OnTimeChanged += UpdateTime;
    }

    private void UpdateTime(int time)
    {
        var seconds = time % 60;
        var minutes = time / 60;

        _timerLabel.text = $"{minutes:00}:{seconds:00}";

        _lastSecondsLabel.text = time <= 10 ? time.ToString() : string.Empty;
        _lastSecondsLabel.style.fontSize = new StyleLength(100 * (11 - time) + 100);
    }

    private void UpdateScore(int blueScore, int orangeScore)
    {
        _blueTeamScoreLabel.text = blueScore.ToString();
        _orangeTeamScoreLabel.text = orangeScore.ToString();
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _blueTeamScoreLabel = root.Q<Label>("BlueTeamScoreLabel");
        _orangeTeamScoreLabel = root.Q<Label>("OrangeTeamScoreLabel");
        _timerLabel = root.Q<Label>("TimerLabel");
        _lastSecondsLabel = root.Q<Label>("LastSecondsLabel");
    }
}