using UnityEngine;

/// <summary>
/// A singleton class that manages persistent and session state across the game.
/// </summary>
public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    // Location to catch the badger in, determines the world
    public int catchLocation;

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

    public void SetCatchLocation(int location)
    {
        MainManager.Instance.catchLocation = location;
    }
}