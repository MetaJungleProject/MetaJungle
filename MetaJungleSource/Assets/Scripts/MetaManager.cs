using Cinemachine;
using MoralisUnity;
using MoralisUnity.Platform.Objects;
using MoralisUnity.Platform.Queries;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

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

    public localuserData userData = new localuserData();
    public static string username;
    public static string userethAdd;
    public static string useruniqid;


    private void Awake()
    {
        insta = this;
    }

    private void Start()
    {
        Moralis.Start();
        getUserDataonStart();
    }
    public async void getUserDataonStart()
    {
        var user = await Moralis.GetClient().GetCurrentUserAsync();
        if (user == null) return;
        username = user.username;
        userethAdd = user.ethAddress;
        useruniqid = user.objectId;
        UIManager.insta.UpdateUserName(username, userethAdd);
        CheckUserData();

    }

    public async void submitName(string _name)
    {
        var user = await Moralis.GetClient().GetCurrentUserAsync();
        if (user == null) return;
        user.username = _name;

       

        var result = await user.SaveAsync();

        if (result)
        {
            Debug.Log("ReadyToPlay " + result.ToString());
            UIManager.insta.UpdateUserName(user.username, user.ethAddress);
            //CheckUserData();
        }

    }


    public async void UpdateUserDatabase()
    {
        UIManager.insta.UpdatePlayerUIData(true);
        MoralisQuery<MetaJungleDatabase> monster = await Moralis.Query<MetaJungleDatabase>();
        monster = monster.WhereEqualTo("userid", useruniqid);
        IEnumerable<MetaJungleDatabase> result = await monster.FindAsync();
        foreach (MetaJungleDatabase mon in result)
        {
            mon.userdata = JsonConvert.SerializeObject(userData);
            await mon.SaveAsync();
            Debug.Log("UpdateUserDatabase");
            break;
        }
    }

    public async void CheckUserData()
    {
        MoralisQuery<MetaJungleDatabase> monster = await Moralis.Query<MetaJungleDatabase>();
        monster = monster.WhereEqualTo("userid", useruniqid);
        IEnumerable<MetaJungleDatabase> result = await monster.FindAsync();

        var datathere = false;
    
        foreach (MetaJungleDatabase mon in result)
        {
            Debug.Log("My username " + mon.userid + " and  data " + mon.userdata);
            userData = JsonConvert.DeserializeObject<localuserData>(mon.userdata);
            datathere = true;
            UIManager.insta.UpdatePlayerUIData(true, true);
            //UpdateUserDatabase(_uniqid);
            break;
        }

        if (!datathere)
        {
            Debug.Log("addNewUserData");
            addNewUserData();
        }
        else
        {
            Debug.Log("userDataAlreadyThere");
        }
        // return false;
    }

    public async void addNewUserData()
    {
        MetaJungleDatabase monster = Moralis.GetClient().Create<MetaJungleDatabase>();
        monster.userid = useruniqid;
        monster.userdata = JsonConvert.SerializeObject(userData);
        monster.gamedata = "gamedataNeedToAdd";
        var result = await monster.SaveAsync();

        if (result)
        {
            Debug.Log("CheckUserData Again");
            CheckUserData();
        }

    }

}


