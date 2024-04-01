using Dreamteck.Splines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Start,
    Catching,
    End
}

/// <summary>
/// A singleton that manages the catch scene.
/// - Starts stopwatch to manage transitions between scenes
/// - Controls UI elements based on game state
/// - When badger is caught/timer runs out, ends the game and switches back to home
/// </summary>
public class CatchSceneManager : MonoBehaviour
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
    [SerializeField]
    [Tooltip("Will be found if not set")]
    private SplineComputer splineComputer;
    [SerializeField]
    [Tooltip("Will be found if not set")]
    private TransitionManager transitionManager;

    // Game state
    internal bool isBadgerCaught = false;
    public delegate void OnGameStateChange(GameState newState);
    public static event OnGameStateChange GameStateChanged;
    private GameState currentState;

    private System.Diagnostics.Stopwatch stopwatch;
    private readonly int startDelay = 1;
    private readonly int countdownSeconds = 3;
    private readonly int catchSeconds = 60;
    private readonly int endSeconds = 3;
    private readonly float spawnXRange = 2;
    private readonly float spawnZRange = 2;

    void Awake() {
        GameStateChanged += OnGameStateChanged;
    }

    void Start()
    {
        // find if not initialized
        splineComputer = splineComputer == null ? FindObjectOfType<SplineComputer>() : splineComputer;
        transitionManager = transitionManager == null ? FindObjectOfType<TransitionManager>() : transitionManager;

        // initialize variables
        currentState = GameState.Start;
        GameStateChanged.Invoke(currentState);
        if (MainManager.Instance != null)
        {
            int catchLocation = MainManager.Instance.catchLocation;
            Debug.Log("Catch location: " + catchLocation);
        }

        // start stopwatch and countdown
        stopwatch = new System.Diagnostics.Stopwatch();
        Invoke(nameof(StartCountdown), startDelay);

        // spawn badger and configure catch location
        // TODO: Catch location configuration
        Vector3 spawnPosition = GetNonOverlappingSpawnPosition();

        GameObject badger = Instantiate(badgerPrefab, spawnPosition, badgerPrefab.transform.rotation);

        splineComputer.transform.position = spawnPosition;

        SplineFollower splineFollower = badger.AddComponent<SplineFollower>();
        splineFollower.spline = splineComputer;
        splineFollower.follow = true;
        splineFollower.followMode = SplineFollower.FollowMode.Uniform;
        splineFollower.wrapMode = SplineFollower.Wrap.Loop;
        splineFollower.followSpeed = 1;
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
        GameStateChanged.Invoke(currentState);
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
                    Debug.Log(MainManager.Instance);
                    MainManager.Instance.AddBadger(MainManager.Instance.badgerToCatch);
                } else {
                    completionText.text = "You missed the badger...";
                }

                Invoke(nameof(GoToHomeScreen), endSeconds);
                break;
        }
    }

    private void GoToHomeScreen() {
        transitionManager.GoToScene(0);
    }

    private Vector3 GetNonOverlappingSpawnPosition()
    {
        Vector3 randomPosition = new(Random.Range(-spawnXRange, spawnXRange), 0f, Random.Range(-spawnZRange, spawnZRange));

        // TODO: Ensure that badger isn't spawned on world obstacles

        return randomPosition;
    }
}