public enum AmmoType
{
    Light,
    Heavy,
    Shells,
    Energy
}

[System.Serializable]
public struct AmmoEntry
{
    public AmmoType type;
    public int amount;
}
