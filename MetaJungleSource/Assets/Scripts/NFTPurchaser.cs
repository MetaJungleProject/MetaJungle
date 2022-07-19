using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Defective.JSON;
using MoralisUnity;
using Nethereum.Hex.HexTypes;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NFTPurchaser : MonoBehaviour
{
    public static NFTPurchaser insta;

    public static event Action PurchaseStarted;
    public static event Action<string> PurchaseCompleted;
    public static event Action PurchaseFailed;

    private void Awake()
    {
        insta = this;
    }

    private void Start()
    {
        //StartCoroutine(UploadPNG());
    }



    public IEnumerator UploadNFTMetadata(string _metadata, int cost)
    {
        MessaeBox.insta.showMsg("NFT purchase process started\nThis can up to minute", false);
        Debug.Log("Creating and saving metadata to IPFS...");
        WWWForm form = new WWWForm();
        form.AddField("meta", _metadata);

        using (UnityWebRequest www = UnityWebRequest.Post("https://api.nft.storage/store", form))
        {
            www.SetRequestHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkaWQ6ZXRocjoweDZBNDA4Q0ZiOTJDNDlCYjk3ZDhENDg4NUE3MGE3NkNhOWVBYUIyNjIiLCJpc3MiOiJuZnQtc3RvcmFnZSIsImlhdCI6MTY1Nzg3NDg0ODU1MywibmFtZSI6Ik1ldGFKdW5nbGUifQ.DHiD9jVmKkMJQaZtF6WLUO7QpGwnXiAi2s4l_Lt_BRA");
            www.timeout = 70;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Debug.Log("UploadNFTMetadata upload error " + www.downloadHandler.text);
                MessaeBox.insta.showMsg("Server error\nPlease try again", true);
            }
            else
            {
                Debug.Log("UploadNFTMetadata upload complete! " + www.downloadHandler.text);

                JSONObject j = new JSONObject(www.downloadHandler.text);
                if (j.HasField("value"))
                {
                    //Debug.Log("Predata " + j.GetField("value").GetField("ipnft").stringValue);
                    SingletonDataManager.nftmetaCDI = j.GetField("value").GetField("url").stringValue; //ipnft
                    //SingletonDataManager.tokenID = j.GetField("value").GetField("ipnft").stringValue; //ipnft
                    Debug.Log("Metadata saved successfully");
                    PurchaseItem(cost);
                }
            }
        }
    }

    public string metaData;
    IEnumerator UploadPNG()
    {
        // We should only read the screen after all rendering is complete
        yield return new WaitForEndOfFrame();

        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        // Create a Web Form
        WWWForm form = new WWWForm();
       // form.AddField("meta", metaData);
        form.AddBinaryData("file", bytes, "screenShot.png", "image/*");

        var data = new List<IMultipartFormSection> {
             new MultipartFormDataSection("foo", "bar"),
             new MultipartFormFileSection("file", bytes, "test.png", "image/*")
         };

        // Upload to a cgi script
        using (var w = UnityWebRequest.Post("https://api.nft.storage/upload", form))
        {
            w.SetRequestHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkaWQ6ZXRocjoweDZBNDA4Q0ZiOTJDNDlCYjk3ZDhENDg4NUE3MGE3NkNhOWVBYUIyNjIiLCJpc3MiOiJuZnQtc3RvcmFnZSIsImlhdCI6MTY1Nzg3NDg0ODU1MywibmFtZSI6Ik1ldGFKdW5nbGUifQ.DHiD9jVmKkMJQaZtF6WLUO7QpGwnXiAi2s4l_Lt_BRA");
            yield return w.SendWebRequest();
            if (w.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(w.error);
                Debug.Log("Form upload error " + w.downloadHandler.text);
            }
            else
            {
                Debug.Log("Form upload complete! " + w.downloadHandler.text);
            }
        }
    }


    public async void PurchaseItem(int cost)
    {
        PurchaseStarted?.Invoke();


        //transactionInfoText.text = "Creating and saving metadata to IPFS...";

        var metadataUrl = SingletonDataManager.nftmetaCDI;// + SingletonDataManager.postfixMetaUrl;


       

        long currentTime = DateTime.Now.Ticks;
        var _currentTokenID = new BigInteger(currentTime);

        //transactionInfoText.text = "Metadata saved successfully";
       // Debug.Log("Toekn " + currentTime + " | " + SingletonDataManager.tokenID.Length);
        Debug.Log("Toekn Trim " + _currentTokenID);

        long tokenId = MoralisTools.ConvertStringToLong(SingletonDataManager.useruniqid);

        //transactionInfoText.text = "Please confirm transaction in your wallet";
        MessaeBox.insta.showMsg("Please confirm transaction in your wallet", false);
        Debug.Log("Please confirm transaction in your wallet " + tokenId);
        var result = await PurchaseItemFromContract(metadataUrl);

        if (result is null)
        {
            // transactionInfoText.text = "Transaction failed";
            // StartCoroutine(DisableInfoText());
            Debug.Log("Transaction failed!");
            MessaeBox.insta.showMsg("Transaction failed!", true);
            PurchaseFailed?.Invoke();
            return;
        }
        Debug.Log("Transaction completed! " + result.ToString());

        //reudce coins
        SingletonDataManager.userData.score = SingletonDataManager.userData.score - cost;
        SingletonDataManager.insta.UpdateUserDatabase();
        MessaeBox.insta.showMsg("Transaction completed!\nClose this window and go to my collection button", true);
        // transactionInfoText.text = "Transaction completed!";
        // StartCoroutine(DisableInfoText());

        SingletonDataManager.insta.LoadPurchasedItems();

        PurchaseCompleted?.Invoke(SingletonDataManager.nftmetaCDI);
        SingletonDataManager.nftmetaCDI = null;
       // SingletonDataManager.tokenID = null;
    }
    private BigInteger _currentTokenId;
    // We are minting the NFT and transferring it to the player
    private async Task<string> PurchaseItemFromContract( string metadataUrl)
    {

#if UNITY_WEBGL
        string[] data = new string[0];
         long currentTime = DateTime.Now.Ticks;
        _currentTokenId = new BigInteger(currentTime);

        object[] parameters = {
            _currentTokenId.ToString(),
            metadataUrl,
            data
        };
#else
        byte[] data = Array.Empty<byte>();
        long currentTime = DateTime.Now.Ticks;
        _currentTokenId = new BigInteger(currentTime);

        object[] parameters = {
            _currentTokenId.ToString("x"),
            metadataUrl,
            data
        };
#endif

        // Set gas estimate
        HexBigInteger value = new HexBigInteger(0);
        HexBigInteger gas = new HexBigInteger(0);
        HexBigInteger gasPrice = new HexBigInteger(0);

        //Debug.Log("DataTRansfer " + JsonConvert.SerializeObject(parameters));
            

        string resp = await Moralis.ExecuteContractFunction(SingletonDataManager.insta.contract_ethAddress, SingletonDataManager.contract_abi, "buyItem", parameters, value, gas, gasPrice);


        return resp;
    }
}
