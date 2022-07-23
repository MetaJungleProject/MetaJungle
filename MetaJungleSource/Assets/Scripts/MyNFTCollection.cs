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

    private void OnEnable()
    {
        ClosePurchasePanel();

        if (CovalentManager.loadingData)
        {
            MessaeBox.insta.showMsg("Loading Data", true);
            CloseItemPanel();
            return;
        }

        foreach (Transform child in itemParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < SingletonDataManager.myNFTData.Count; i++)
        {
            var temp = Instantiate(itemButtonPrefab, itemParent);
            temp.GetComponent<RawImage>().texture = SingletonDataManager.metanftlocalData[SingletonDataManager.myNFTData[i].itemid].imageTexture;
            var tempNo = i;
            var tempTexture = temp.GetComponent<RawImage>().texture;
            temp.GetComponent<Button>().onClick.AddListener(() => SelectItem(tempNo, tempTexture));
        }

       
    }

    public void SelectItem(int _no, Texture _texture)
    {
        Debug.Log("Selected item " + _no);
        currentSelectedItem = _no;
        itemPanelUI.SetActive(false);
        itemPurchaseUI.SetActive(true);
        purchaseItemImg.texture = _texture;// itemButtons[_no].GetComponent<RawImage>().texture;
        purchaseItemText.text = SingletonDataManager.myNFTData[_no].description;

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
        if (!CovalentManager.loadingData) CovalentManager.insta.GetNFTUserBalance();
        foreach (Transform child in itemParent)
        {
            Destroy(child.gameObject);
        }
        gameObject.SetActive(false);
    }
}
