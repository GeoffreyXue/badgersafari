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
    // // Serialized fields
    // [SerializeField]
    // private TextMeshPro countdownText = new TextMeshPro();
    // [SerializeField]
    // // private TextMeshPro timerText = new();
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
                if (stopwatch.Elapsed.Seconds >= countdownSeconds)
                {
                    ChangeGameState(GameState.Catching);
                    stopwatch.Restart();
                }
                else
                {
                    // countdownText.text = (countdownSeconds - stopwatch.Elapsed.Seconds).ToString();
                }
                break;
            case GameState.Catching:
                if (stopwatch.Elapsed.Seconds >= catchSeconds)
                {
                    ChangeGameState(GameState.End);
                    stopwatch.Stop();
                }
                else
                {
                    // timerText.text = (catchSeconds - stopwatch.Elapsed.Seconds).ToString();
                }
                break;
            case GameState.End:
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
        if (newState == GameState.End) {
            if (isBadgerCaught) {
                Debug.Log("You caught the badger!");
            } else {
                Debug.Log("You missed the badger!");
            }
        }
    }
}