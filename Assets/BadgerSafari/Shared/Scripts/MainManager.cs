using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// A singleton class that manages persistent and session state across the game.
/// </summary>
public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    // Badger to catch in the catch scene
    [HideInInspector]
    public BadgerData badgerToCatch;
    // Location to catch the badger in, determines the world
    [HideInInspector]
    public int catchLocation;


    private const string badgerFilePath = "/badgers.json";
    private class SaveData {
        public BadgerData[] badgers;
    }

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
        Debug.Log(path);
        if (File.Exists(path))
        {
            try {
                string json = File.ReadAllText(path);
                BadgerData[] badgers = JsonUtility.FromJson<SaveData>(json).badgers;
                Assert.IsNotNull(badgers);
                return badgers;
            }
            catch (Exception e) {
                Debug.LogError("Error reading badger data: " + e.Message);
                // delete file to ensure that we can start with a fresh file
                File.Delete(path);
                return new BadgerData[0];
            }
        }
        else {
            Debug.Log("File doesn't exist.");
            return new BadgerData[0];
        }
    }

    public void AddBadger(BadgerData newBadger)
    {
        Debug.Log(newBadger);
        BadgerData[] badgers = LoadBadgers();
        Array.Resize(ref badgers, badgers.Length + 1);
        badgers[badgers.Length - 1] = newBadger;
        
        try {
            string path = Application.persistentDataPath + badgerFilePath;
            string json = JsonUtility.ToJson(new SaveData() { badgers = badgers });
            Debug.Log(json);
            File.WriteAllText(path, json);
        }
        catch (Exception e) {
            Debug.LogError("Error writing badger data: " + e.Message);
        }
    }
}