using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public static StoreManager insta;
    [SerializeField] GameObject itemPanelUI;
    [SerializeField] GameObject itemPurchaseUI;

    //item panel stuff
    [SerializeField] Button[] itemButtons;


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
        for (int i = 0; i < itemButtons.Length; i++) {
            if (SingletonDataManager.insta.nftImg[i]) {
                itemButtons[i].GetComponent<RawImage>().texture = SingletonDataManager.insta.nftImg[i];
            }
        }
    }
    public void SelectItem(int _no)
    {
        currentSelectedItem = _no;
        itemPanelUI.SetActive(false);
        itemPurchaseUI.SetActive(true);
        purchaseItemImg.texture = itemButtons[_no].GetComponent<RawImage>().texture;
        Debug.Log("Selected item " + _no);
    }

    public void purchaseItem()
    {

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
