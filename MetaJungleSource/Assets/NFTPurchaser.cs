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

    private async UniTask<string> CreateIpfsMetadata()
    {
        // 1. Build Metadata
        object metadata = MoralisTools.BuildMetadata("name", "description", "imageUrl");

        string metadataName = $"{"name"}_{"objectId"}.json";

        // 2. Encoding JSON
        string json = JsonConvert.SerializeObject(metadata);
        string base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

        // 3. Save metadata to IPFS
        //string ipfsMetadataPath = await MoralisTools.SaveToIpfs(metadataName, base64Data);

        return json;
    }


    public IEnumerator UploadNFTMetadata(string _metadata)
    {
        Debug.Log("Creating and saving metadata to IPFS...");
        WWWForm form = new WWWForm();
        form.AddField("meta", _metadata);

        using (UnityWebRequest www = UnityWebRequest.Post("https://api.nft.storage/store", form))
        {
            www.SetRequestHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkaWQ6ZXRocjoweDZBNDA4Q0ZiOTJDNDlCYjk3ZDhENDg4NUE3MGE3NkNhOWVBYUIyNjIiLCJpc3MiOiJuZnQtc3RvcmFnZSIsImlhdCI6MTY1Nzg3NDg0ODU1MywibmFtZSI6Ik1ldGFKdW5nbGUifQ.DHiD9jVmKkMJQaZtF6WLUO7QpGwnXiAi2s4l_Lt_BRA");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Debug.Log("UploadNFTMetadata upload error " + www.downloadHandler.text);
            }
            else
            {
                Debug.Log("UploadNFTMetadata upload complete! " + www.downloadHandler.text);

                JSONObject j = new JSONObject(www.downloadHandler.text);
                if (j.HasField("value"))
                {
                    //Debug.Log("Predata " + j.GetField("value").GetField("ipnft").stringValue);
                    SingletonDataManager.nftmetaCDI = j.GetField("value").GetField("ipnft").stringValue;
                    Debug.Log("Metadata saved successfully");
                    PurchaseItem();
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


    public async void PurchaseItem()
    {
        PurchaseStarted?.Invoke();

       
        //transactionInfoText.text = "Creating and saving metadata to IPFS...";

        var metadataUrl = SingletonDataManager.nftmetaCDI + SingletonDataManager.postfixMetaUrl;

     
        //transactionInfoText.text = "Metadata saved successfully";

        long tokenId = MoralisTools.ConvertStringToLong(SingletonDataManager.nftmetaCDI);

        //transactionInfoText.text = "Please confirm transaction in your wallet";

        Debug.Log("Please confirm transaction in your wallet");
        var result = await PurchaseItemFromContract(tokenId, metadataUrl);

        if (result is null)
        {
            // transactionInfoText.text = "Transaction failed";
            // StartCoroutine(DisableInfoText());
            Debug.Log("Transaction failed!");
            PurchaseFailed?.Invoke();
            return;
        }
        Debug.Log("Transaction completed! " + result.ToString());
       // transactionInfoText.text = "Transaction completed!";
       // StartCoroutine(DisableInfoText());

        PurchaseCompleted?.Invoke(SingletonDataManager.nftmetaCDI);
        SingletonDataManager.nftmetaCDI = null;
    }

    // We are minting the NFT and transferring it to the player
    private async Task<string> PurchaseItemFromContract(BigInteger tokenId, string metadataUrl)
    {
        byte[] data = Array.Empty<byte>();

        object[] parameters = {
            tokenId.ToString("x"),
            metadataUrl,
            data
        };

        // Set gas estimate
        HexBigInteger value = new HexBigInteger("0x0");
        HexBigInteger gas = new HexBigInteger(0);
        HexBigInteger gasPrice = new HexBigInteger("0x0");

        string resp = await Moralis.ExecuteContractFunction(SingletonDataManager.insta.contract_ethAddress, SingletonDataManager.insta.contract_abi, "buyItem", parameters, value, gas, gasPrice);

        return resp;
    }
}
