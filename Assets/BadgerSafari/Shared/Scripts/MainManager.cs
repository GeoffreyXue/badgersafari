using System;
using System.IO;
using UnityEngine;

/// <summary>
/// A singleton class that manages persistent and session state across the game.
/// </summary>
public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    // Location to catch the badger in, determines the world
    public int CatchLocation { get; set; }

    // constant for the badger file path
    public const string badgerFilePath = "/badgers.json";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public BadgerData[] LoadBadgers()
    {
        string path = Application.persistentDataPath + badgerFilePath;
        if (File.Exists(path))
        {
            try {
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<BadgerData[]>(json);
            }
            catch (Exception e) {
                Debug.LogError("Error reading badger data: " + e.Message);
                return new BadgerData[0];
            }
        }
        else {
            return new BadgerData[0];
        }
    }

    public void AddBadger(BadgerData newBadger)
    {
        BadgerData[] badgers = LoadBadgers();
        Array.Resize(ref badgers, badgers.Length + 1);
        badgers[badgers.Length - 1] = newBadger;
        
        try {
            string path = Application.persistentDataPath + badgerFilePath;
            string json = JsonUtility.ToJson(badgers);
            File.WriteAllText(path, json);
        }
        catch (Exception e) {
            Debug.LogError("Error writing badger data: " + e.Message);
        }
    }
}