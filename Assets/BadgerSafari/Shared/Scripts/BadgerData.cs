using System;
using UnityEngine;

public enum BadgerType
{
    Normal,
    Water,
    Fire
}

/// <summary>
/// Serializable representation of badger, used for persistence
/// </summary>
[Serializable]
public class BadgerData
{
    public string name;
    public string favoriteFood;
    [SerializeField] long _timecode;
    public DateTimeOffset dateCaught {
        get {
            return DateTimeOffset.FromUnixTimeMilliseconds(_timecode);
        }
        set {
            _timecode = value.ToUnixTimeMilliseconds();
        }
    }
    public float size;
    public BadgerType type;
}