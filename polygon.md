## Polygon mumbai

### Contract Address : 0xf639f267ad2495166037b863bec81fe6151591fb
### Blockchain: Polygon Mumbai - Testnet
### Explore : https://mumbai.polygonscan.com/address/0xf639f267ad2495166037b863bec81fe6151591fb

https://github.com/ThunderGameStudio/MetaJungle/blob/main/MetaJungleSource/Assets/Scripts/NFTPurchaser.cs

### Polygon NFT 1155 contract interaction and miniting process

``` c#
 public async void PurchaseItem(int cost, int _id)
    {
        PurchaseStarted?.Invoke();

        var metadataUrl = SingletonDataManager.nftmetaCDI;// + SingletonDataManager.postfixMetaUrl;
        long currentTime = DateTime.Now.Ticks;
        var _currentTokenID = new BigInteger(currentTime);

        Debug.Log("Toekn Trim " + _currentTokenID);

        //long tokenId = MoralisTools.ConvertStringToLong(SingletonDataManager.useruniqid);
        long tokenId = (long)(_id + 200);

        //transactionInfoText.text = "Please confirm transaction in your wallet";
        MessaeBox.insta.showMsg("Please confirm transaction in your wallet", false);
        Debug.Log("Please confirm transaction in your wallet " + tokenId);
        var result = await PurchaseItemFromContract(tokenId, metadataUrl);

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
        MessaeBox.insta.showMsg("Transaction completed!\nTransaction can take some time to complete on blockchain", true);
        PurchaseCompleted?.Invoke(SingletonDataManager.nftmetaCDI);
        SingletonDataManager.nftmetaCDI = null;
        StoreManager.insta.ClosePurchasePanel();
        StoreManager.insta.CloseItemPanel();
        
    }
``` 
![Polygon use](/Images/PolygonMatic.jpg)
