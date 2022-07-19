using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessaeBox : MonoBehaviour
{
    public static MessaeBox insta;
    [SerializeField] GameObject msgBoxUI;
    [SerializeField] GameObject okBtn;
    [SerializeField] TMP_Text msgText;

    private void Awake()
    {
        insta = this;
        msgBoxUI.SetActive(false);
    }
    public void showMsg(string _msg, bool showBtn) {
        msgBoxUI.SetActive(true);
        if (showBtn) okBtn.SetActive(true);
        else okBtn.SetActive(false);

        msgText.text = _msg;

    }

    public void OkButton() {
        msgBoxUI.SetActive(false);
    }
}
