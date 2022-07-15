using MoralisUnity;
using MoralisUnity.Platform.Queries;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Defective.JSON;

public class SingletonDataManager : MonoBehaviour
{
    public static SingletonDataManager insta;

    public static localuserData userData = new localuserData();
    public static string username;
    public static string userethAdd;
    public static string useruniqid;
    [SerializeField]
    public static List<MetaJungleNFTLocal> metanftlocalData = new List<MetaJungleNFTLocal>();

    public string contract_abi;
    public string contract_ethAddress;
    public const string postfixMetaUrl = ".ipfs.nftstorage.link/metadata.json";
    public static string nftmetaCDI;

    bool initData = false;


    public string jsonData;

    private void Awake()
    {
        if (insta == null)
        {
            insta = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        Moralis.Start();
        //getUserDataonStart();

        //JsonReader jr = JSON. JsonConvert.DeserializeObject(jsonData);

       
      

    }


    public async void getUserDataonStart()
    {
        getNFTDetailsData();

        var user = await Moralis.GetClient().GetCurrentUserAsync();
        if (user == null) return;
        username = user.username;
        userethAdd = user.ethAddress;
        useruniqid = user.objectId;
        //UIManager.insta.
        initData = true;
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
            //UIManager.insta.UpdateUserName(user.username, user.ethAddress);
            //CheckUserData();
        }

    }

    public async void UpdateUserDatabase()
    {
        if (UIManager.insta) UIManager.insta.UpdatePlayerUIData(true);
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
            if (initData)
            {
                initData = false;
                UnityEngine.SceneManagement.SceneManager.LoadScene("OpenWorld");
            }
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


    public async void getNFTDetailsData()
    {

        //
        MoralisQuery<MetaJungleNFT> _getAllItemsQuery = await Moralis.Query<MetaJungleNFT>();
        IEnumerable<MetaJungleNFT> databaseItems = await _getAllItemsQuery.FindAsync();

        var databaseItemsList = databaseItems.ToList();
        if (!databaseItemsList.Any()) return;


        foreach (var databaseItem in databaseItemsList)
        {
            //PopulateShopItem(databaseItem);
            if (databaseItem.name != null)
            {
                MetaJungleNFTLocal _data = new MetaJungleNFTLocal();
                _data.imageurl = databaseItem.imageurl;
                _data.name = databaseItem.name;
                _data.description = databaseItem.description;
                _data.cost = databaseItem.cost;
                _data.itemid = databaseItem.itemid;
                metanftlocalData.Add(_data);
            }
        }

        Debug.Log("DAta is " + JsonConvert.SerializeObject(metanftlocalData));
        GetAllNFTImg();
        // return false;
    }

    //public List<Texture> nftImg = new List<Texture>();
    public void GetAllNFTImg() {
        for (int i = 0; i < metanftlocalData.Count; i++) {
            StartCoroutine(GetTexture(metanftlocalData[i].imageurl, i));
        }
      
    }

    IEnumerator GetTexture(string _url, int _index)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            metanftlocalData[_index].imageTexture = (((DownloadHandlerTexture)www.downloadHandler).texture);
        }
    }

}
