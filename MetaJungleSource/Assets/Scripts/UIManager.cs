using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager insta;
    [SerializeField] GameObject usernameUI;
    [SerializeField] TMP_Text usernameText;

    [SerializeField] TMP_InputField nameInput;
    [SerializeField] Button[] gender;

    [SerializeField] TMP_Text statusText;

    // fight manager
    [SerializeField] GameObject FightRequestUI;
    [SerializeField] TMP_Text fightRequestText;


    public static string username;
    public static int usergender;

    [Header("GameplayMenu")]
    [SerializeField] GameObject GameplayUI;
    [SerializeField] TMP_Text scoreTxt;
    [SerializeField] TMP_Text winCountTxt;
    [SerializeField] TMP_Text lostCountTxt;
    [SerializeField] Slider healthSlider;

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
        statusText.gameObject.SetActive(true);
        statusText.text = "";
        healthSlider.value = 1;

       UpdatePlayerUIData(true, true);
        UpdateUserName(SingletonDataManager.username, SingletonDataManager.userethAdd);

    }

    public void UpdatePlayerUIData(bool _show, bool _init = false)
    {
        if (_show)
        {
            if (_init) {
                nameInput.text = SingletonDataManager.username;
                SelectGender(SingletonDataManager.userData.characterNo);
            }
            if (!GameplayUI.activeSelf) GameplayUI.SetActive(true);
            scoreTxt.text = SingletonDataManager.userData.score.ToString();
            winCountTxt.text = SingletonDataManager.userData.fightWon.ToString();
            lostCountTxt.text = SingletonDataManager.userData.fightLose.ToString();
            if(PhotonNetwork.LocalPlayer.CustomProperties["health"] != null) healthSlider.value = float.Parse(PhotonNetwork.LocalPlayer.CustomProperties["health"].ToString());
        }
        else
        {
            GameplayUI.SetActive(false);
        }
    }

    public void UpdateUserName(string _name, string _ethad)
    {
        usernameText.text = "Hi, " + _name + "\n Your crypto address is : " + _ethad;
    }

    public void UpdateStatus(string _msg)
    {
        statusText.text = _msg;
        StartCoroutine(ResetUpdateText());
    }

    IEnumerator ResetUpdateText()
    {
        yield return new WaitForSeconds(2);
        statusText.text = "";
    }

    public void GetName()
    {
        if (nameInput.text.Length > 0 && !nameInput.text.Contains("Enter")) username = nameInput.text;
        else username = "Player_" + Random.Range(11111, 99999);

        usernameUI.SetActive(false);

        SingletonDataManager.insta.submitName(username);

        MPNetworkManager.insta.OnConnectedToServer();
    }

    public void SelectGender(int _no)
    {
        if (_no == 0)
        {
            gender[0].interactable = false;
            gender[1].interactable = true;
        }
        else
        {
            gender[1].interactable = false;
            gender[0].interactable = true;
        }

        usergender = _no;
        SingletonDataManager.userData.characterNo = _no;
    }


    #region FightREquest
    public void FightReq(string _userdata)
    {
        FightRequestUI.SetActive(true);
        fightRequestText.text = _userdata + " want to fight with you !";
    }

    public void FightReqAcion(bool _accept)
    {
        if (_accept)
        {

        }
        else
        {

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
