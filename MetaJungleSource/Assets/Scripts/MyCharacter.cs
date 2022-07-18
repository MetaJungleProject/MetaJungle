using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using StarterAssets;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MyCharacter : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] ThirdPersonController tController;

    [SerializeField] Animator myAnim;
    [SerializeField] CharacterController mController;
    [SerializeField] GameObject vCamTarget;
    [SerializeField] PlayerInput _playerInput;
    [SerializeField] StarterAssetsInputs _inputs;
    [SerializeField] StarterAssets.StarterAssets _customInput;

    [SerializeField] GameObject[] myPlayers;
    [SerializeField] TMP_Text usernameText;
    int playerNo;
    PhotonView pview;
    [SerializeField] GameObject meetObj;

    [SerializeField] GameObject virtualWorldUI;
    [SerializeField] GameObject meetUI;

    [SerializeField] GameObject WeaponCollider;



    //bool isFighting;

    //weapon items
    [SerializeField] GameObject weaponObj;
    [SerializeField] GameObject[] weaponParent;
    [SerializeField] GameObject[] weapons;
    Vector3 weaponLastPoz;
    Quaternion weaponLastRot;


    //ui manger
    [SerializeField] GameObject healthUI;
    [SerializeField] Image healthbarIng;
    //int playerHealth = 100;

    private void Awake()
    {
        WeaponCollider.SetActive(false);
       
    }
    private void Start()
    {

        pview = GetComponent<PhotonView>();
        _inputs = GetComponentInParent<StarterAssetsInputs>();
        _customInput = new StarterAssets.StarterAssets();
        _customInput.Player.Enable();

        if (pview.IsMine)
        {
            MetaManager.insta.myPlayer = gameObject;
            MetaManager.insta.playerCam.Follow = vCamTarget.transform;
            MetaManager.insta.uiInput.starterAssetsInputs = _inputs;
            meetObj.SetActive(true);

        }
        else
        {

        }
        meetUI.SetActive(false);
        virtualWorldUI.SetActive(false);
        showHealthBar(false);

        playerNo = int.Parse(pview.Owner.CustomProperties["char_no"].ToString());
        myPlayers[playerNo].SetActive(true);
        myAnim = myPlayers[playerNo].GetComponent<Animator>();
        usernameText.text = pview.Owner.NickName;

        if ((bool)pview.Owner.CustomProperties["isfighting"])
        {
            SelectWeapon();
            showHealthBar(true);
            healthbarIng.fillAmount = float.Parse(pview.Owner.CustomProperties["health"].ToString());

        }
    }

    private void LateUpdate()
    {


        usernameText.transform.LookAt(MetaManager.insta.myCam.transform);
        usernameText.transform.rotation = Quaternion.LookRotation(MetaManager.insta.myCam.transform.forward);

        healthbarIng.transform.LookAt(MetaManager.insta.myCam.transform);
        healthbarIng.transform.rotation = Quaternion.LookRotation(MetaManager.insta.myCam.transform.forward);

        meetUI.transform.LookAt(MetaManager.insta.myCam.transform);
        meetUI.transform.rotation = Quaternion.LookRotation(MetaManager.insta.myCam.transform.forward);

        virtualWorldUI.transform.LookAt(MetaManager.insta.myCam.transform);
        virtualWorldUI.transform.rotation = Quaternion.LookRotation(MetaManager.insta.myCam.transform.forward);


    }

    private void Update()
    {
        if (pview.IsMine)
        {
            if (!myAnim.GetBool("attack"))
            {
                if (tController.Grounded && (mController.velocity.x != 0 || mController.velocity.z != 0))
                {
                    myAnim.SetBool("walk", true);
                }
                else
                {
                    myAnim.SetBool("walk", false);
                }
            }

            if (_customInput.Player.Attack.triggered && tController.Grounded && MetaManager.isFighting && !myAnim.GetBool("attack"))
            {
                myAnim.SetBool("attack", true);
                StartCoroutine(waitForEnd(myAnim.GetCurrentAnimatorStateInfo(0).length));
                AudioManager.insta.playSound(8);
                WeaponCollider.SetActive(true);
                Debug.Log("Attack Collider");
            }
            else
            {
                WeaponCollider.SetActive(false);
            }

            MetaManager.isAtttacking = myAnim.GetBool("attack");



        }

        //if (!tController.Grounded)
        //_inputs.attack = false;

    }

    IEnumerator waitForEnd(float _time, int _action = 0)
    {

        yield return new WaitForSeconds(0.31f);
        myAnim.SetBool("attack", false);
    }
   

    bool _waitToReattack = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!MetaManager.isFighting)
        {
            if (!pview.IsMine && (bool)pview.Owner.CustomProperties["isfighting"] == false)
            {
                if (other.CompareTag("Meet"))
                {
                    Debug.Log("Meet him");
                    meetUI.SetActive(true);
                    virtualWorldUI.SetActive(true);
                }
            }
        }
        else
        {
            if (!pview.IsMine && (bool)pview.Owner.CustomProperties["isfighting"])
            {

                if (other.CompareTag("Weapon0") && MetaManager.isAtttacking)
                {
                    if (!_waitToReattack)
                    {
                        Debug.Log("Fight My " + pview.Owner.UserId + " | figher " + MetaManager._fighterid);

                        if (MetaManager._fighterid.Equals(pview.Owner.UserId))
                        {
                            StartCoroutine(waitToReattack());
                        }


                    }

                }
            }
        }

    }

    IEnumerator waitToReattack()
    {
        _waitToReattack = true;
        Debug.Log("Attacked " + pview.Owner);
        // UpdateHealth();
        AudioManager.insta.playSound(playerNo);
        pview.RPC("UpdateHealth", RpcTarget.All, pview.Owner.UserId);
        yield return new WaitForSeconds(0.2f);
        _waitToReattack = false;
    }

    void ResetFight()
    {
        if (pview.IsMine)
        {
            var hash = PhotonNetwork.LocalPlayer.CustomProperties;
            healthbarIng.fillAmount = 1;
            hash["health"] = healthbarIng.fillAmount;
            hash["isfighting"] = false;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (MetaManager.isFighting) return;
        if (!pview.IsMine)
        {
            if (other.CompareTag("Meet"))
            {
                Debug.Log("Meet bye");
                meetUI.SetActive(false);
                virtualWorldUI.SetActive(false);
            }
        }
    }

    #region weapons

    int currentWeapon = 0;
    public void SelectWeapon()
    {
        Debug.Log("SelectWeapon");
        meetUI.SetActive(false);
        if (pview.IsMine) MetaManager.isFighting = true;
        weaponObj.SetActive(true);
        showHealthBar(true);
        weaponLastPoz = weapons[currentWeapon].transform.localPosition;
        weaponLastRot = weapons[currentWeapon].transform.localRotation;
        weapons[currentWeapon].SetActive(true);
        weapons[currentWeapon].transform.parent = weaponParent[playerNo].transform;
    }

    void ResetWeapon()
    {
        if (pview.IsMine) MetaManager.isFighting = false;
        weapons[currentWeapon].transform.parent = weaponObj.transform;
       // weapons[currentWeapon].transform.localPosition = weaponLastPoz;
        //weapons[currentWeapon].transform.localRotation = weaponLastRot;

        weapons[currentWeapon].SetActive(false);
        weaponObj.SetActive(false);


    }

    #endregion

    #region RPC
    public void RequestFight()
    {
        Debug.Log("RequestFight" + pview.Owner.NickName);
        Debug.Log("RequestFightID" + pview.Owner.UserId);
        MetaManager._fighterid = pview.Owner.UserId;
        AudioManager.insta.playSound(2);
        // MetaManager.insta.myPlayer.GetComponent<MyCharacter>().pview.RPC("RequestFightRPC", RpcTarget.All, pview.Owner.UserId);
        pview.RPC("RequestFightRPC", RpcTarget.All, pview.Owner.UserId);

        Debug.Log("RequestFight My " + MetaManager.insta.myPlayer.GetComponent<PhotonView>().Owner.UserId + " | figher " + MetaManager._fighterid);

        UIManager.insta.UpdateStatus("Fight request sent to\n" + pview.Owner.NickName);

    }
    [PunRPC]
    void RequestFightRPC(string _uid, PhotonMessageInfo info)
    {
        Debug.Log("uidPre " + _uid);
        if (pview.IsMine)
        {

            Debug.Log("uid " + _uid);
            if (pview.Owner.UserId.Equals(_uid))
            {
                if (MetaManager.fightReqPlayer != null || MetaManager.isFighting) return;

                Debug.LogFormat("Info: {0} {1}", info.Sender, info.photonView.IsMine);

                MetaManager._fighterid = info.Sender.UserId;
                MetaManager.fightReqPlayer = info.Sender;
                //MetaManager.fighterView = info.photonView;
                //MetaManager.fightPlayer = info.photonView.gameObject;
                UIManager.insta.FightReq(info.Sender.ToString());
                AudioManager.insta.playSound(3);

                Debug.Log("RequestFightRPC My " + MetaManager.insta.myPlayer.GetComponent<PhotonView>().Owner.UserId + " | figher " + MetaManager._fighterid);

            }
        }
    }

    public void RequestFightAction(bool _action)
    {
        Debug.Log("RequestFightAction" + pview.Owner.NickName + " | " + pview.IsMine);


        SendFightAction(_action, MetaManager.fightReqPlayer.UserId, PhotonNetwork.LocalPlayer.UserId);
        MetaManager.fightReqPlayer = null;

    }




    #endregion

    #region Health
    void showHealthBar(bool _show)
    {
        if (_show)
        {
            healthUI.SetActive(true);
        }
        else
        {
            healthUI.SetActive(false);
            healthbarIng.fillAmount = 1;
        }
    }

    [PunRPC]
    void UpdateHealth(string _uid)
    {
        if (pview.Owner.UserId.Equals(_uid))
        {
            if (healthbarIng.fillAmount > 0.1)
            {
                healthbarIng.fillAmount -= 0.1f;
                if (pview.IsMine)
                {
                    AudioManager.insta.playSound(playerNo);
                    var hash = PhotonNetwork.LocalPlayer.CustomProperties;
                    hash["health"] = healthbarIng.fillAmount;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

                    UIManager.insta.UpdatePlayerUIData(true);
                }
            }
            else
            {
                showHealthBar(false);
               
                ResetWeapon();
                ResetFight();

                if (pview.IsMine)
                {
                    Debug.Log("UserLost");
                    AudioManager.insta.playSound(7);
                    SingletonDataManager.userData.fightLose++;
                    SingletonDataManager.insta.UpdateUserDatabase();
                    UIManager.insta.ShowResult(1);
                }
            }

           
        }
    }
    #endregion


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (targetPlayer.UserId.Equals(MetaManager._fighterid))
        {
            if (!(bool)targetPlayer.CustomProperties["isfighting"] && healthUI.activeSelf)
            {
                if ((bool)pview.Owner.CustomProperties["isfighting"] && pview.IsMine)
                {
                    Debug.Log("User Winner");
                    AudioManager.insta.playSound(6);
                    SingletonDataManager.userData.fightWon++;
                    SingletonDataManager.insta.UpdateUserDatabase();
                    UIManager.insta.ShowResult(0);
                }

              
                showHealthBar(false);
                ResetFight();
                ResetWeapon();
             
            }

        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //base.OnPlayerLeftRoom(otherPlayer);

        if (pview.IsMine) {
            if (otherPlayer.UserId.Equals(MetaManager._fighterid))
            {
                if ((bool)otherPlayer.CustomProperties["isfighting"] && healthUI.activeSelf)
                {
                    AudioManager.insta.playSound(9);

                    Debug.Log("Player left");
                    showHealthBar(false);
                    ResetFight();
                    ResetWeapon();
                    UIManager.insta.ShowResult(2);
                }

            }
        }
    }


    // If you have multiple custom events, it is recommended to define them in the used class
    public const byte FightEventCode = 1;

    private void SendFightAction(bool _action, string _p1uid, string _p2uid)
    {
        Debug.Log("SendFightAction OnEvent");
        object[] content = new object[] { _action, _p1uid, _p2uid }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(FightEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    //
    public void OnEvent(EventData photonEvent)
    {
        
        byte eventCode = photonEvent.Code;
        if (eventCode == FightEventCode)
        {
            if ((bool)pview.Owner.CustomProperties["isfighting"]) return;

            object[] data = (object[])photonEvent.CustomData;
            bool _action = (bool)data[0];
            for (int i = 1; i < data.Length; i++)
            {
                if (pview.Owner.UserId.Equals((string)data[i]))
                {
                    if (_action)
                    {
                        //Debug.Log(info.Sender + " is ready to fight " + pview.IsMine);

                        UIManager.insta.UpdateStatus(PhotonNetwork.CurrentRoom.Players[photonEvent.Sender].NickName + " is ready to fight");
                        if (pview.IsMine)
                        {
                            var hash = PhotonNetwork.LocalPlayer.CustomProperties;
                            hash["isfighting"] = true;
                            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                        }
                        SelectWeapon();
                        AudioManager.insta.playSound(4);
                    }
                    else
                    {
                        //Debug.Log(info.Sender + " rejected fight");
                        AudioManager.insta.playSound(5);
                        UIManager.insta.UpdateStatus(PhotonNetwork.CurrentRoom.Players[photonEvent.Sender].NickName + " rejected fight");
                    }
                }
            }


        }
    }
}
