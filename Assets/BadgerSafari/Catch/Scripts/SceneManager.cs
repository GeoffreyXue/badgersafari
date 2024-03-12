using UnityEngine;

public enum GameState
{
    Start,
    Catching,
    End
}

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject badgerPrefab;
    internal bool isBadgerCaught = false;
    internal GameState gameState = GameState.Start;

    private readonly float startDelay = 3;
    private readonly float catchingTime = 60;
    private readonly float spawnXRange = 10;
    private readonly float spawnZRange = 10;

    void Start()
    {
        // spawn in random position
        Vector3 randomPosition = new Vector3(Random.Range(-spawnXRange, spawnXRange), 0, Random.Range(-spawnZRange, spawnZRange));
        Instantiate(badgerPrefab, randomPosition, Quaternion.identity);

        // start catching after delay
        Invoke(nameof(StartCatching), startDelay);
    }

    void StartCatching() {
        gameState = GameState.Catching;
        Debug.Log("Start catching!");

        // finish catching after catching time
        Invoke(nameof(FinishCatching), catchingTime);
    }

    void FinishCatching() {
        gameState = GameState.End;
        Debug.Log("Finish catching!");
    }

    void Update() {
        if (gameState == GameState.End) {
            if (isBadgerCaught) {
                Debug.Log("You caught the badger!");
            } else {
                Debug.Log("You missed the badger!");
            }
        }
    }
}