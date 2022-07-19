using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FrostweepGames;

public class UIManager : MonoBehaviour
{
    public static UIManager insta;
  
    [Header("GameplayMenu")]
    public  GameObject StartUI;
    public GameObject usernameUI;


    public TMP_Text usernameText;

    [SerializeField] TMP_InputField nameInput;
    [SerializeField] Button[] gender;

    [SerializeField] TMP_Text statusText;

    // fight manager
    [SerializeField] GameObject FightRequestUI;
    [SerializeField] TMP_Text fightRequestText;


    public static string username;
    public static int usergender;

    [Header("GameplayMenu")]
    public GameObject GameplayUI;
    [SerializeField] TMP_Text scoreTxt;
    [SerializeField] TMP_Text winCountTxt;
    [SerializeField] TMP_Text lostCountTxt;
    [SerializeField] Slider healthSlider;

    [SerializeField] FrostweepGames.WebGLPUNVoice.Recorder recorder;
    [SerializeField] FrostweepGames.WebGLPUNVoice.Listener lister;


    [Header("VoiceChat")]
    [SerializeField] Image recorderImg;
    [SerializeField] Image listenerImg;
    [SerializeField] Sprite[] recorderSprites; //0 on 1 off
    [SerializeField] Sprite[] listenerSprites; //0 on 1 off


    [Header("Result")]
    [SerializeField] Image resultImg;
    [SerializeField] Sprite[] resultprites; //0 win 1 lose 2 tie

    public GameObject MyCollectionUIButton;

    [SerializeField] GameObject VirtualWorldObj;

    private void Awake()
    {
        insta = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        GameplayUI.SetActive(false);
        resultImg.gameObject.SetActive(false);
        StartUI.SetActive(true);
        usernameUI.SetActive(false);
        gender[0].interactable = false;
        gender[1].interactable = true;
        statusText.gameObject.SetActive(true);
        statusText.text = "";
        healthSlider.value = 1;

        UpdatePlayerUIData(true, true);
        UpdateUserName(SingletonDataManager.username, SingletonDataManager.userethAdd);

    }

    public void ShowResult(int _no) {

        LeanTween.scale(resultImg.gameObject, Vector2.one, 1.5f).setFrom(Vector2.zero).setEaseOutBounce();
        StartCoroutine(gameResult(_no));

    }

    IEnumerator gameResult(int _no) {
        resultImg.gameObject.SetActive(true);
        resultImg.sprite = resultprites[_no];
        yield return new WaitForSeconds(3);
        resultImg.gameObject.SetActive(false);
    }

    public void VisitVirtualWorld(bool _show) {
        if (_show)
        {
            SingletonDataManager.isMyVirtualWorld = true;
            VirtualWorldObj.SetActive(true);
        }
        else {
            VirtualWorldObj.SetActive(false);
        }
    }

    public void StartGame() {
        //StartUI.SetActive(false);
        MPNetworkManager.insta.OnConnectedToServer();
    }

    public void UpdatePlayerUIData(bool _show, bool _init = false)
    {
        if (_show)
        {
            if (_init) {
                nameInput.text = SingletonDataManager.username;
                SelectGender(SingletonDataManager.userData.characterNo);
            }
          
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

    public void MuteUnmute() {
        if (recorder.recording)
        {
            recorder.recording = false;
            recorderImg.sprite = recorderSprites[1];
        }
        else {
            recorder.recording = true;
            recorderImg.sprite = recorderSprites[0];
        }

        
    }

    public void MuteUnmuteListner() {
        if (lister._listening)
        {
            lister._listening = false;
            listenerImg .sprite = listenerSprites[1];
        }
        else {
            lister._listening = true;
            listenerImg.sprite = listenerSprites[0];
        }
    }

    public void openMyWorld() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("VirtualWorld");
    }
    public void UpdateUserName(string _name, string _ethad = null)
    {
        if (_ethad != null)
        {
            usernameText.text = "Hi, " + _name + "\n Your crypto address is : " + _ethad;
            username = _name;
        }
        else usernameText.text = _name;
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


    public void EditUserProfile() {
        usernameUI.SetActive(true);
        StartUI.SetActive(false);
    }
    public void GetName()
    {
        if (nameInput.text.Length > 0 && !nameInput.text.Contains("Enter")) username = nameInput.text;
        else username = "Player_" + Random.Range(11111, 99999);

       
            usernameUI.SetActive(false);

            SingletonDataManager.insta.submitName(username);

            StartUI.SetActive(true);
        
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
