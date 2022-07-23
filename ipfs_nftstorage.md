## IPFS/Filecoin is used with NFT.Storage to store NFT data


https://github.com/ThunderGameStudio/MetaJungle/blob/main/MetaJungleSource/Assets/Scripts/NFTPurchaser.cs

### Store NFT data on NFT.Storage IPFS
[Official site API](https://nft.storage/api-docs/)

``` C#
    public IEnumerator UploadNFTMetadata(string _metadata, int cost, int _id)
    {
        MessaeBox.insta.showMsg("NFT purchase process started\nThis can up to minute", false);
        Debug.Log("Creating and saving metadata to IPFS...");
        WWWForm form = new WWWForm();
        form.AddField("meta", _metadata);

        using (UnityWebRequest www = UnityWebRequest.Post("https://api.nft.storage/store", form))
        {
            www.SetRequestHeader("Authorization", "Bearer "+ AuthKEY);
            www.timeout = 40;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Debug.Log("UploadNFTMetadata upload error " + www.downloadHandler.text);
                MessaeBox.insta.showMsg("Server error\nPlease try again", true);
                www.Abort();
                www.Dispose();
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
                    PurchaseItem(cost, _id);
                }
            }
        }
    }
```
### IPFS/Filecoin use with NFT.Storage 
![NFT.Storage use](/Images/NFT.Storage.jpg)
