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
    private GameObject normalBadgerPrefab;
    [SerializeField]
    private GameObject waterBadgerPrefab;
    [SerializeField]
    private GameObject infoPanelPrefab;
    
    [SerializeField]
    [Tooltip("Will be found if not set")]
    private TransitionManager transitionManager;

    private int badgerLength;

    private readonly float spawnArea = 5f;
    private readonly float badgerSize = 1f; 

    void Start()
    {
        if (!normalBadgerPrefab || !waterBadgerPrefab)
        {
            Debug.LogError("Badger prefabs not set in HomeSceneManager.");
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
            Debug.Log(badgers);
            badgerLength = badgers.Length;

            foreach (BadgerData badgerData in badgers)
            {
                Vector3 spawnPosition = GetNonOverlappingSpawnPosition();
                GameObject newBadger = Instantiate(
                    badgerData.type == BadgerType.Normal ? normalBadgerPrefab : waterBadgerPrefab, 
                    spawnPosition, 
                    Quaternion.identity
                );

                HomeBadgerBehavior badgerBehavior = newBadger.AddComponent<HomeBadgerBehavior>();
                badgerBehavior.Init(badgerData, infoPanelPrefab);
            }
        }
    }

    public void OnCatchButtonClicked()
    {
        // configure catch location and badger to catch
        BadgerType type = BadgerType.Water; // Random.Range(0, 2) == 0 ? BadgerType.Normal : BadgerType.Water;

        BadgerData badger = new() {
            name = $"Badger the {badgerLength + 1}",
            dateCaught = System.DateTime.Now,
            type = type
        };
        MainManager.Instance.badgerToCatch = badger;
        MainManager.Instance.catchLocation = type == BadgerType.Normal ? 1 : 2;
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
