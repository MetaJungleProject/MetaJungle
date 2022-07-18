using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyNFTCollection : MonoBehaviour
{
    public static MyNFTCollection insta;
    [SerializeField] GameObject itemPanelUI;
    [SerializeField] GameObject itemPurchaseUI;

    [SerializeField] Transform itemParent;
    //item panel stuff
    [SerializeField] GameObject itemButtonPrefab;


    //purcahse panel stuff
    [SerializeField] RawImage purchaseItemImg;
    [SerializeField] TMP_Text purchaseItemText;

    int currentSelectedItem = -1;

    private void Awake()
    {
        insta = this;
    }

    private void Start()
    {
        for (int i = 0; i < SingletonDataManager.myNFTData.Count; i++)
        {
            var temp = Instantiate(itemButtonPrefab, itemParent);
            temp.GetComponent<RawImage>().texture = SingletonDataManager.metanftlocalData[SingletonDataManager.myNFTData[i].itemid].imageTexture;
        }

       
    }
    public void SelectItem(int _no)
    {
        currentSelectedItem = _no;
        itemPanelUI.SetActive(false);
        itemPurchaseUI.SetActive(true);
        //purchaseItemImg.texture = itemButtons[_no].GetComponent<RawImage>().texture;
        purchaseItemText.text = SingletonDataManager.metanftlocalData[_no].description;
        Debug.Log("Selected item " + _no);
    }

    public void purchaseItem()
    {
        Debug.Log("purchaseItem");
        MetadataNFT meta = new MetadataNFT();
        meta.itemid = SingletonDataManager.metanftlocalData[currentSelectedItem].itemid;
        meta.name = SingletonDataManager.metanftlocalData[currentSelectedItem].name;
        meta.description = SingletonDataManager.metanftlocalData[currentSelectedItem].description;
        meta.image = SingletonDataManager.metanftlocalData[currentSelectedItem].imageurl;
        //meta.itemid = SingletonDataManager.metanftlocalData[currentSelectedItem].

        NFTPurchaser.insta.StartCoroutine(NFTPurchaser.insta.UploadNFTMetadata(Newtonsoft.Json.JsonConvert.SerializeObject(meta)));
    }

    public void ClosePurchasePanel()
    {
        itemPanelUI.SetActive(true);
        itemPurchaseUI.SetActive(false);
    }

    public void CloseItemPanel()
    {
        itemPanelUI.SetActive(false);
        itemPurchaseUI.SetActive(false);
    }
}
