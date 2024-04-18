using System;

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
    public DateTime dateCaught;
    public BadgerType type;
}