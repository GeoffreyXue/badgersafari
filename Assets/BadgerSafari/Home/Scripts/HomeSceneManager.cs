using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singleton that manages the home scene.
/// - Spawns badgers into the home scene
/// </summary>
public class HomeSceneManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> badgerPrefabs;
    [SerializeField]
    private GameObject infoPanelPrefab;
    
    [SerializeField]
    [Tooltip("Will be found if not set")]
    private TransitionManager transitionManager;

    private int badgerLength;

    private readonly float spawnArea = 5f;
    private readonly float badgerSize = 1f; 

    private readonly List<string> badgerNames = new() {
        "Bucky Badger",
        "Alice",
        "Existential Crisis Bob",
        "Bob",
        "Amazon Web Services",
        "Eleanor of Aquitaine",
        "2007 Black & Orange Porsche GT3 RS",
    };

    private readonly List<string> badgerFavoriteFoods = new() {
        "Cheese Curds",
        "Rathskeller Burger",
        "Ginger Root Sesame Chicken",
        "Ruyi's Beef Hand Pulled Noodles",
        "Short Stack Pancakes",
        "Jalisco Steak Fajitas",
        "McDonald's French Fries",
        "Grass",
        "Subway Footlong Italian BMT"
    };

    void Start()
    {
        if (badgerPrefabs.Count != System.Enum.GetValues(typeof(BadgerType)).Length)
        {
            Debug.LogError("Not enough badger prefabs not set in HomeSceneManager.");
        }

        if (!infoPanelPrefab) {
            Debug.LogError("Info panel prefab not set in HomeBadgerBehavior.");
        }

        if (!transitionManager)
        {
            transitionManager = FindObjectOfType<TransitionManager>();
        }

        if (MainManager.Instance)
        {
            BadgerData[] badgers = MainManager.Instance.LoadBadgers();
            Debug.Log($"Loaded {badgers.Length} badgers.");
            badgerLength = badgers.Length;

            foreach (BadgerData badgerData in badgers)
            {
                Vector3 spawnPosition = GetNonOverlappingSpawnPosition();
                GameObject newBadger = Instantiate(
                    badgerPrefabs[(int)badgerData.type], 
                    spawnPosition, 
                    Quaternion.identity
                );

                // scale badger based on size
                newBadger.transform.localScale = new Vector3(badgerData.size, badgerData.size, badgerData.size);

                HomeBadgerBehavior badgerBehavior = newBadger.AddComponent<HomeBadgerBehavior>();
                badgerBehavior.Init(badgerData, infoPanelPrefab);
            }
        }
    }

    public void OnCatchButtonClicked()
    {
        // configure catch location and badger to catch
        Random.InitState(System.DateTime.Now.Millisecond);

        // choose random badger name
        string badgerName = badgerNames[Random.Range(0, badgerNames.Count)];
        // choose random badger favorite food
        string favoriteFood = badgerFavoriteFoods[Random.Range(0, badgerFavoriteFoods.Count)];
        // choose random badger type
        int decision = Random.Range(0, System.Enum.GetValues(typeof(BadgerType)).Length);
        BadgerType type = (BadgerType)decision;
        // choose random badger size
        float size = Random.Range(0.7f, 1.3f);

        BadgerData badger = new() {
            name = badgerName,
            favoriteFood = favoriteFood,
            dateCaught = System.DateTime.Now,
            type = type,
            size = size
        };

        MainManager.Instance.badgerToCatch = badger;
        MainManager.Instance.catchLocation = (int)type + 1;
        transitionManager.GoToScene(MainManager.Instance.catchLocation);
    }

    private Vector3 GetNonOverlappingSpawnPosition()
    {
        Vector3 randomPosition = Vector3.zero;
        bool isValidSpawnPosition = false;

        // ensure that the badger is not spawned on top of another badger
        while (!isValidSpawnPosition)
        {
            randomPosition = new Vector3(Random.Range(-spawnArea, spawnArea), 0f, Random.Range(-spawnArea, spawnArea));

            // use sphere overlap to check for badgers in the area
            Collider[] colliders = Physics.OverlapSphere(randomPosition, badgerSize);
            colliders = System.Array.FindAll(colliders, c => c.CompareTag("Badger"));

            if (colliders.Length == 0)
            {
                isValidSpawnPosition = true;
            }
        }

        return randomPosition;
    }
}
