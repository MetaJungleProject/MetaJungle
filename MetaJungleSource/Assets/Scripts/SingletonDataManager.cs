using MoralisUnity;
using MoralisUnity.Platform.Queries;
using MoralisUnity.Web3Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using FrostweepGames.Plugins.Native;

public class SingletonDataManager : MonoBehaviour
{
    public static SingletonDataManager insta;

    public static localuserData userData = new localuserData();
    public static string username;
    public static string userethAdd;
    public static string useruniqid;
    [SerializeField]
    public static List<MetaJungleNFTLocal> metanftlocalData = new List<MetaJungleNFTLocal>();
    public static List<MyMetadataNFT> myNFTData = new List<MyMetadataNFT>();
    public List<MyMetadataNFT> otherPlayerNFTData = new List<MyMetadataNFT>();
    public static bool isMyVirtualWorld = true;

    public static string contract_abi = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"indexed\":false,\"internalType\":\"uint256[]\",\"name\":\"values\",\"type\":\"uint256[]\"}],\"name\":\"TransferBatch\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"TransferSingle\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"value\",\"type\":\"string\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"URI\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address[]\",\"name\":\"accounts\",\"type\":\"address[]\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"}],\"name\":\"balanceOfBatch\",\"outputs\":[{\"internalType\":\"uint256[]\",\"name\":\"\",\"type\":\"uint256[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"_tokenUrl\",\"type\":\"string\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"buyItem\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"amounts\",\"type\":\"uint256[]\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeBatchTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"uri\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";
    public string contract_ethAddress;
    public const string postfixMetaUrl = ".ipfs.nftstorage.link/metadata.json";
    public static string nftmetaCDI;
   // public static string tokenID;
    public static ChainList ContractChain = ChainList.mumbai;

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
    byte[] data;
    private void Start()
    {

        // jigar
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            if (QualitySettings.vSyncCount > 0)
                Application.targetFrameRate = 60;
            else
                Application.targetFrameRate = -1;
        }

        CustomMicrophone.RequestMicrophonePermission();
        CustomMicrophone.RefreshMicrophoneDevices();

        Moralis.Start();
        //getUserDataonStart();

        //JsonReader jr = JSON. JsonConvert.DeserializeObject(jsonData);


        //long tokenId = MoralisTools.ConvertStringToLong(jsonData);
       // Debug.Log(tokenId);

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

        LoadPurchasedItems();

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
    public void GetAllNFTImg()
    {
        for (int i = 0; i < metanftlocalData.Count; i++)
        {
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



    public async void LoadPurchasedItems()
    {
        //We get our wallet address.
        // MoralisUser user = await Moralis.GetUserAsync();
        var user = await Moralis.GetClient().GetCurrentUserAsync();
        var playerAddress = user.authData["moralisEth"]["id"].ToString();

        Debug.Log("playerAddress " + playerAddress);
        myNFTData.Clear();
        try
        {
            NftOwnerCollection noc =
                await Moralis.GetClient().Web3Api.Account.GetNFTsForContract(playerAddress.ToLower(),
                    contract_ethAddress,
                    ContractChain);

            List<NftOwner> nftOwners = noc.Result;
            Debug.Log("nftOwners " + nftOwners.Count);
            Debug.Log("nftOwnersJson " + noc.ToJson());
            // We only proceed if we find some
            if (!nftOwners.Any())
            {
                Debug.Log("You don't own any NFT");
                return;
            }


            for (int i = 0; i < nftOwners.Count; i++)
            {
                if (nftOwners[i].Metadata == null)
                {
                    // Sometimes GetNFTsForContract fails to get NFT Metadata. We need to re-sync
                    //Moralis.GetClient().Web3Api.Token.ReSyncMetadata(nftOwner.TokenAddress, nftOwner.TokenId, ContractChain);
                    Debug.Log("We couldn't get NFT Metadata. Re-syncing...");
                    continue;
                }

                var nftMetaData = nftOwners[i].Metadata;
                NftMetadata formattedMetaData = JsonUtility.FromJson<NftMetadata>(nftMetaData);
                Debug.Log(nftOwners[i].TokenId + " nftMetaData " + JsonConvert.DeserializeObject(nftMetaData));
                MyMetadataNFT myData = new MyMetadataNFT();
                myData = JsonConvert.DeserializeObject<MyMetadataNFT>(nftMetaData);
                myData.tokenId = nftOwners[i].TokenId;
                myNFTData.Add(myData);

                //PopulatePlayerItem(nftOwner.TokenId, formattedMetaData);
                Debug.Log("PopulatePlayerItem " + nftOwners[i].TokenId + " | " + formattedMetaData.ToJson());
            }

        }
        catch (Exception exp)
        {
            Debug.LogError(exp.Message);
        }

        if (myNFTData.Count > 0) {
            if (MetaManager.insta) {
                MetaManager.insta.UpdatePlayerWorldProperties();
                Debug.Log("We UpdatePlayerWorldProperties");
            }
        }
    }

}
