using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoralisUnity.Platform.Objects;

public class DataManager
{
  
}

[System.Serializable]
public class localuserData
{
    public int fightWon = 0;
    public int fightLose = 0;
    public int score = 0;
    public int characterNo = 0;
}

#region moralisDatabase
[System.Serializable]
public class MetaJungleDatabase : MoralisObject
{
    public string userid { get; set; }
    public string userdata { get; set; }
    public string gamedata { get; set; }

    public MetaJungleDatabase() : base("MetaJungleDatabase") { }
}

[System.Serializable]
public class MetaJungleNFT : MoralisObject
{
    public int itemid { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string imageurl { get; set; }
    public int cost { get; set; }

    public MetaJungleNFT() : base("MetaJungleNFT") { }
}

[System.Serializable]
public class MetaJungleNFTLocal 
{
    public int itemid;
    public string name;
    public string description;
    public string imageurl;
    public int cost;
}
#endregion
