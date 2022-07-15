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
    [SerializeField] Image purchaseItemImg;
    [SerializeField] TMP_Text purchaseItemText;

    int currentSelectedItem = -1;

    private void Awake()
    {
        insta = this;
    }

    public void SelectItem(int _no)
    {
        currentSelectedItem = _no;
        itemPanelUI.SetActive(false);
        itemPurchaseUI.SetActive(true);
        purchaseItemImg.sprite = itemButtons[_no].image.sprite;
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
