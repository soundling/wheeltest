using System;
using System.Collections.Generic;

[Serializable]
public class WheelPartData
{
    public int euros;
    public bool isJackpot;
}

[Serializable]
public class WheelInfosData
{
    public List<WheelPartData> data;
    public string key;
}

[Serializable]
public class WheelResultData
{
    public int index;
    public int euros;
    public bool isJackpot;
}

[Serializable]
public class Key
{
    public string key;
}