using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
public class UIManager : MonoBehaviour
{
    public static UIManager insta;
    [SerializeField] GameObject usernameUI;
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] Button[] gender;

    [SerializeField] TMP_Text statusText;

    // fight manager
    [SerializeField] GameObject FightRequestUI;
    [SerializeField] TMP_Text fightRequestText;


    public static string username;
    public static int usergender;


    private void Awake()
    {
        insta = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        usernameUI.SetActive(true);
        gender[0].interactable = false;
        gender[1].interactable = true;
        statusText.text = "";

    }

    public void UpdateStatus(string _msg) {
        statusText.text = _msg;
        StartCoroutine(ResetUpdateText());
    }

    IEnumerator ResetUpdateText() {
        yield return new WaitForSeconds(1);
        statusText.text = "";
    }

    public void GetName() {
        if (nameInput.text.Length > 0 &&  !nameInput.text.Contains("Enter")) username = nameInput.text;
        else username = "Player_" + Random.Range(11111, 99999);

        usernameUI.SetActive(false);

        MPNetworkManager.insta.OnConnectedToServer();
    }

    public void SelectGender(int _no) {
        if (_no == 0)
        {
            gender[0].interactable = false;
            gender[1].interactable = true;
        }
        else {
            gender[1].interactable = false;
            gender[0].interactable = true;
        }

        usergender = _no;
    }


    #region FightREquest
    public void FightReq(string _userdata) {
        FightRequestUI.SetActive(true);
        fightRequestText.text = _userdata + " want to fight with you !";
    }

    public void FightReqAcion(bool _accept)
    {
        if (_accept)
        {
           
        }
        else { 
        
        }
        MetaManager.insta.myPlayer.GetComponent<MyCharacter>().RequestFightAction(_accept);
        FightRequestUI.SetActive(false);
        Debug.Log("Fight Action " + _accept);
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("UpdateHealthMe", RpcTarget.All, PhotonNetwork.LocalPlayer.UserId);
    }
    [PunRPC]
    void UpdateHealthMe(string _uid)
    {
        Debug.Log("CheckID " + _uid);
    }

    #endregion
}
