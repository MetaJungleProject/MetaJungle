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
public class MetaJungleDatabase : MoralisObject
{
    public string userid { get; set; }
    public string userdata { get; set; }
    public string gamedata { get; set; }

    public MetaJungleDatabase() : base("MetaJungleDatabase") { }
}
#endregion
