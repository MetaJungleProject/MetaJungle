
## Covalent is used in Meta Jungle to load player's NFT Balance and NFT Metadata


https://github.com/ThunderGameStudio/MetaJungle/blob/main/MetaJungleSource/Assets/CovalentManager.cs

### Fetch NFT Balance From Covalent API
[Official site API](https://www.covalenthq.com/docs/api/#/0/Get%20token%20balances%20for%20address/USD/80001/?utm_source=covalent&utm_medium=docs)
```c#
 public void GetNFTUserBalance()
    {
       if(!loadingData) StartCoroutine(GetNFTBalance());
        else Debug.Log("Already loading GetNFTBalance");
    }
    IEnumerator GetNFTBalance()
    {
        loadingData = true;
        Debug.Log("GetNFTBalance");
        //yield return new WaitForSeconds(1f);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(BalanceFetchPreURL + SingletonDataManager.userethAdd + BalanceFetchPostURL))
        {

            webRequest.timeout = 60;
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            //string[] pages = uri.Split('/');
            //int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    loadingData = false;
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    loadingData = false;
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    loadingData = false;
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Received: " + webRequest.downloadHandler.text);

                    JSONObject _data = new JSONObject(webRequest.downloadHandler.text);

                    if (_data.GetField("data").HasField("items"))
                    {

                        myTokenID.Clear();
                        SingletonDataManager.myNFTData.Clear();

                        for (int i = 0; i < _data.GetField("data").GetField("items").list.Count; i++)
                        {
                            var _add = _data.GetField("data").GetField("items")[i].GetField("contract_address").stringValue.ToLower();
                            if (SingletonDataManager.insta.contract_ethAddress.ToLower().Equals(_add))
                            {
                                if (_data.GetField("data").GetField("items")[i].GetField("nft_data").list.Count > 0)
                                {
                                    Debug.Log("Found :" + _add + " | NFT" + _data.GetField("data").GetField("items")[i].GetField("nft_data").list.Count);
                                    // myTokenID.Clear();

                                    for (int j = 0; j < _data.GetField("data").GetField("items")[i].GetField("nft_data").list.Count; j++)
                                    {
                                        myTokenID.Add(_data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("token_id").stringValue);
                                        GetNFTMetaDataDetails(_data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("token_id").stringValue);
                                        yield return new WaitForSeconds(0.3f);
                                    }

                                }



                            }
                        }
                    }
                    //yield return new WaitForSeconds(0.5f);
                    //loadingData = false;
                    break;
            }

        }

        yield return new WaitForSeconds(0.5f);
        loadingData = false;
    }
```

### Fetch NFT Metadata From Covalent API
[Official site API](https://www.covalenthq.com/docs/api/#/0/Get%20NFT%20external%20metadata%20for%20contract/USD/80001/?utm_source=covalent&utm_medium=docs)
```c#
 void GetNFTMetaDataDetails(string _tid)
    {
        StartCoroutine(GetNFTMetaData(_tid));
    }
    IEnumerator GetNFTMetaData(string _tokenid)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(GetMetaDataPreURL + SingletonDataManager.insta.contract_ethAddress + GetMetaDataMidURL + _tokenid + GetMetaDataPostURL))
        {
            webRequest.timeout = 30;
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            //string[] pages = uri.Split('/');
            //int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Received: NFTMeta " + webRequest.downloadHandler.text);

                    JSONObject _data = new JSONObject(webRequest.downloadHandler.text);

                    if (_data.GetField("data").HasField("items"))
                    {

                        for (int i = 0; i < _data.GetField("data").GetField("items").list.Count; i++)
                        {
                            var _add = _data.GetField("data").GetField("items")[i].GetField("contract_address").stringValue.ToLower();
                            if (SingletonDataManager.insta.contract_ethAddress.ToLower().Equals(_add))
                            {
                                if (_data.GetField("data").GetField("items")[i].GetField("nft_data").list.Count > 0)
                                {
                                    Debug.Log("Found :" + _add + " | NFT" + _data.GetField("data").GetField("items")[i].GetField("nft_data").list.Count);

                                    for (int j = 0; j < _data.GetField("data").GetField("items")[i].GetField("nft_data").list.Count; j++)
                                    {
                                        Debug.Log("Found Details token_id :" + _data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("token_id").stringValue);
                                        Debug.Log("Found Details item_id :" + (int.Parse(_data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("token_id").stringValue) - 200));
                                        Debug.Log("Found Details name :" + _data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("external_data").GetField("name").stringValue);
                                        Debug.Log("Found Details description :" + _data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("external_data").GetField("description").stringValue);
                                        Debug.Log("Found Details image :" + _data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("external_data").GetField("image").stringValue);

                                        MyMetadataNFT _nftData = new MyMetadataNFT();
                                        _nftData.name = _data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("external_data").GetField("name").stringValue;
                                        _nftData.description = _data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("external_data").GetField("description").stringValue;
                                        _nftData.image = _data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("external_data").GetField("image").stringValue;
                                        _nftData.tokenId = _data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("token_id").stringValue;
                                        _nftData.itemid = (int.Parse(_data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("token_id").stringValue) - 200);
                                        //myNFTData.Add(_nftData);
                                        SingletonDataManager.myNFTData.Add(_nftData);


                                    }
                                }


                            }
                        }
                    }


                    if (MetaManager.insta)
                    {
                        MetaManager.insta.UpdatePlayerWorldProperties();
                        Debug.Log("We UpdatePlayerWorldProperties");
                       // if (MessaeBox.insta) MessaeBox.insta.OkButton();
                    }

                    break;
            }
        }
    }
```
