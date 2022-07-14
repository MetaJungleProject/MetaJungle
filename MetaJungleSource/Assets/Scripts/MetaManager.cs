using Cinemachine;
using MoralisUnity;
using MoralisUnity.Platform.Objects;
using MoralisUnity.Platform.Queries;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

public class MetaManager : MonoBehaviour
{

    public static MetaManager insta;

    public Transform[] playerPoz;
    public CinemachineVirtualCamera playerCam;
    public UICanvasControllerInput uiInput;
    public GameObject myCam;
    public GameObject myPlayer;


    //public static GameObject fightPlayer;
    //public static Photon.Pun.PhotonView fighterView;
    public static bool isFighting = false;
    public static bool isAtttacking = false;
    public static Photon.Realtime.Player fightReqPlayer;

    public static string _fighterid;

    private void Awake()
    {
        insta = this;
    }

    private void Start()
    {
        Moralis.Start();
    }

    public async void submitName(string _name)
    {
        var user = await Moralis.GetClient().GetCurrentUserAsync();
        if (user == null) return;
        user.username = _name;

       

        var result = await user.SaveAsync();

        if (result)
        {
            Debug.Log("ReadyToPlay");
            CheckUserData(user.objectId);
        }

    }


    public async void updateObjectInDB()
    {
        MoralisQuery<MetaJungleDatabase> monster = await Moralis.Query<MetaJungleDatabase>();
        monster = monster.WhereEqualTo("Name", "Aegon");
        IEnumerable<MetaJungleDatabase> result = await monster.FindAsync();
        foreach (MetaJungleDatabase mon in result)
        {
            // mon.userid = "the_great_mage";
            // mon.strength = 6000;
            await mon.SaveAsync();
            // Debug.Log("The monster is " + mon.Name + " with " + mon.strength + " strength");
            // output : The monster is the_great_mage with 6000 strength
        }
    }

    public async void CheckUserData(string _uniqid)
    {
        MoralisQuery<MetaJungleDatabase> monster = await Moralis.Query<MetaJungleDatabase>();
        monster = monster.WhereEqualTo("userid", _uniqid);
        IEnumerable<MetaJungleDatabase> result = await monster.FindAsync();

        var datathere = false;
    
        foreach (MetaJungleDatabase mon in result)
        {
            Debug.Log("My username " + mon.userid + " and  data " + mon.userdata);
            datathere = true;
            break;
        }

        if (!datathere)
        {
            Debug.Log("addNewUserData");
            addNewUserData(_uniqid);
        }
        else
        {
            Debug.Log("userDataAlreadyThere");
        }
        // return false;
    }

    public async void addNewUserData(string _name)
    {
        MetaJungleDatabase monster = Moralis.GetClient().Create<MetaJungleDatabase>();
        monster.userid = _name;
        monster.userdata = "userdataNeedToAdd";
        monster.gamedata = "gamedataNeedToAdd";
        var result = await monster.SaveAsync();

        if (result)
        {
            Debug.Log("CheckUserData Again");
            CheckUserData(_name);
        }

    }

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

