using Defective.JSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using Cysharp.Threading.Tasks;


public class CovalentManager : MonoBehaviour
{
    public static CovalentManager insta;


    public List<string> myTokenID = new List<string>();
    public List<string> otherTokenID = new List<string>();

    //public List<MyMetadataNFT> myNFTData = new List<MyMetadataNFT>();

    public bool loadingData = false;
    public static bool isMyVirtualWorld = true;

    private void Awake()
    {
        insta = this;
    }


    public void GetNFTUserBalance()
    {
        if (!loadingData) MetaJungleManager.Instance.GetNFTList();
        else Debug.Log("Already loading GetNFTBalance");
    }
  

}
