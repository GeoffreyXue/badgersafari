using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Start,
    Catching,
    End
}

public class SceneManager : MonoBehaviour
{
    // Serialized fields
    [SerializeField]
    private TextMeshProUGUI countdownText;
    [SerializeField]
    private TextMeshProUGUI timerText;
    [SerializeField]
    private TextMeshProUGUI completionText;
    [SerializeField]
    private GameObject badgerPrefab;

    // Game state
    internal bool isBadgerCaught = false;
    public delegate void OnGameStateChange(GameState newState);
    public static event OnGameStateChange GameStateChanged;
    private GameState currentState;

    private System.Diagnostics.Stopwatch stopwatch;
    private readonly int startDelay = 1;
    private readonly int countdownSeconds = 3;
    private readonly int catchSeconds = 60;
    private readonly float spawnXRange = 2;
    private readonly float spawnZRange = 2;

    void Start()
    {
        currentState = GameState.Start;
        GameStateChanged += OnGameStateChanged;

        stopwatch = new System.Diagnostics.Stopwatch();
        Invoke(nameof(StartCountdown), startDelay);

        // spawn badger in random position
        Vector3 randomPosition = new Vector3(Random.Range(-spawnXRange, spawnXRange), 0, Random.Range(-spawnZRange, spawnZRange));
        Instantiate(badgerPrefab, randomPosition + badgerPrefab.transform.position, Quaternion.identity);
    }

    void StartCountdown()
    {
        stopwatch.Start();
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.Start:
                if (stopwatch.IsRunning) {
                    if (stopwatch.Elapsed.Seconds >= countdownSeconds)
                    {
                        ChangeGameState(GameState.Catching);
                    }
                    else
                    {
                        countdownText.text = (countdownSeconds - stopwatch.Elapsed.Seconds).ToString();
                    }
                }
                break;
            case GameState.Catching:
                if (stopwatch.Elapsed.Seconds >= catchSeconds)
                {
                    ChangeGameState(GameState.End);
                }
                else
                {
                    timerText.text = (catchSeconds - stopwatch.Elapsed.Seconds).ToString();
                }
                break;
            default:
                break;
        }
    }

    public GameState GetGameState()
    {
        return currentState;
    }

    public void ChangeGameState(GameState newState)
    {
        currentState = newState;
        GameStateChanged?.Invoke(currentState);
    }

    void OnGameStateChanged(GameState newState)
    {
        Debug.Log("Game state changed to: " + newState);
        switch(newState)
        {
            case GameState.Start:
                countdownText.gameObject.SetActive(true);
                timerText.gameObject.SetActive(false);
                completionText.gameObject.SetActive(false);
                break;
            case GameState.Catching:
                stopwatch.Restart();
                countdownText.gameObject.SetActive(false);
                timerText.gameObject.SetActive(true);
                completionText.gameObject.SetActive(false);
                break;
            case GameState.End:
                stopwatch.Stop();
                countdownText.gameObject.SetActive(false);
                timerText.gameObject.SetActive(false);
                completionText.gameObject.SetActive(true);

                if (isBadgerCaught) {
                    completionText.text = "You caught the badger!";
                } else {
                    completionText.text = "You missed the badger...";
                }
                break;
        }
    }
}