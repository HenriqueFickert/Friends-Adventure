﻿[System.Serializable]
public class LocalizationData
{
    public LocalizationItem[] Items;
}

[System.Serializable]
public class LocalizationItem
{
    public string key;
    public string value;
}
