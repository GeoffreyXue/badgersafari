using System;
using UnityEngine;

public enum BadgerType
{
    Normal,
    Water
}

/// <summary>
/// Serializable representation of badger, used for persistence
/// </summary>
[Serializable]
public class BadgerData
{
    public string name;
    [SerializeField] long _timecode;
    public DateTimeOffset dateCaught {
        get {
            return DateTimeOffset.FromUnixTimeMilliseconds(_timecode);
        }
        set {
            _timecode = value.ToUnixTimeMilliseconds();
        }
    }
    public BadgerType type;
}