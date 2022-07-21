using Cinemachine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] Camera cam;

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
    [SerializeField] Vector3[] weaponStartPosz;
    [SerializeField] Quaternion[] weaponStartRotz;
    Vector3 weaponLastPoz;
    Quaternion weaponLastRot;


    //ui manger
    [SerializeField] GameObject healthUI;
    [SerializeField] Image healthbarIng;
    //int playerHealth = 100;


    [Header("Shooting Properties")]
    [SerializeField] Button shootingAreaBtn;
    [SerializeField] Button shootBulletBtn;
    [SerializeField] TMP_Text Text_shootTimer;
    [SerializeField] TMP_Text Text_shootCounter;
    [SerializeField] GameObject bullet_prefab;    
    [SerializeField] float bullet_speed;
    [SerializeField] Transform bullet_start_pos;
    [SerializeField] GameObject crossHair;
    [SerializeField] LayerMask aimColliderMask;
    [SerializeField] Vector3 aimWorldPos;
    Cinemachine3rdPersonFollow followCam;
   
    
    private void Awake()
    {
        WeaponCollider.SetActive(false);

    }
    private void Attack_canceled(InputAction.CallbackContext obj)
    {
        tController.isDragging = false;
    }

    private void Attack_performed(InputAction.CallbackContext obj)
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
        tController.isDragging = true;
    }
    private void Start()
    {

        pview = GetComponent<PhotonView>();
        _inputs = GetComponentInParent<StarterAssetsInputs>();
        _customInput = new StarterAssets.StarterAssets();
        _customInput.Player.Enable();


        weaponStartPosz = new Vector3[weapons.Length];
        weaponStartRotz = new Quaternion[weapons.Length];
        for (int i = 0; i < weapons.Length; i++)
        {
            weaponStartPosz[i] = weapons[i].transform.localPosition;
            weaponStartRotz[i] = weapons[i].transform.localRotation;
        }
        


        if (pview.IsMine)
        {

            _customInput.Player.Attack.performed += Attack_performed;
            _customInput.Player.Attack.canceled += Attack_canceled;

            MetaManager.insta.myPlayer = gameObject;
            MetaManager.insta.playerCam.Follow = vCamTarget.transform;
            MetaManager.insta.fpsCam.Follow = vCamTarget.transform;
            MetaManager.insta.uiInput.starterAssetsInputs = _inputs;

            CameraSwitcher.Register(MetaManager.insta.playerCam);
            CameraSwitcher.Register(MetaManager.insta.fpsCam);
            CameraSwitcher.SwitchCamera(MetaManager.insta.playerCam);

            meetObj.SetActive(true);
            cam = Camera.main;

            followCam = MetaManager.insta.playerCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

            Text_shootTimer = MetaManager.insta.Text_ShootTimer;
            Text_shootCounter = MetaManager.insta.Text_ShootCounter;
            Text_shootTimer.gameObject.SetActive(false);
            Text_shootCounter.gameObject.SetActive(false);

            shootingAreaBtn = MetaManager.insta.shootingAreaBtn;
            shootBulletBtn = MetaManager.insta.shootBulletBtn;
            crossHair = MetaManager.insta.crossHair;


            shootingAreaBtn.onClick.AddListener(GoToShootMode);
            shootBulletBtn.onClick.AddListener(ShootBullet);




        }
        else
        {

        }
        meetUI.SetActive(false);
        virtualWorldUI.SetActive(false);
        showHealthBar(false);

        playerNo = int.Parse(pview.Owner.CustomProperties["char_no"].ToString());
        Debug.Log("Player Number " + playerNo);
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
                if (tController.Grounded && (_inputs.move.x != 0 || _inputs.move.y != 0))
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



           
                if (inShootingMode)
                {

                    Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
                    Ray ray = cam.ScreenPointToRay(center);
                    if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, aimColliderMask))
                    {
                        aimWorldPos = raycastHit.point;                        
                        crossHair.SetActive(true);
                        
                    }
                    else
                    {
                        crossHair.SetActive(false);
                    }

                    
                    if (_customInput.Player.Attack.triggered && shootBulletBtn.interactable && (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()))
                    {
                        ShootBullet();
                    }
                }
            

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
        if (!MetaManager.isFighting && !MetaManager.isShooting)
        {
            if (!pview.IsMine && (bool)pview.Owner.CustomProperties["isfighting"] == false && !MetaManager.inVirtualWorld)
            {
                if (other.CompareTag("Meet") && !other.GetComponentInParent<MyCharacter>().inShootingMode && !inShootingMode)
                {
                    Debug.Log("Meet him");
                    meetUI.SetActive(true);
                    virtualWorldUI.SetActive(true);
                }
            }
            else if (other.CompareTag("shootingArea")) {                
                shootingAreaBtn.gameObject.SetActive(true);
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
        }else if (other.CompareTag("shootingArea"))
        {
            shootingAreaBtn.gameObject.SetActive(false);
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
        /* weaponLastPoz = weapons[currentWeapon].transform.localPosition;
         weaponLastRot = weapons[currentWeapon].transform.localRotation;*/
        weapons[currentWeapon].transform.localPosition = weaponStartPosz[currentWeapon];
        weapons[currentWeapon].transform.localRotation = weaponStartRotz[currentWeapon];

        weapons[currentWeapon].SetActive(true);
        weapons[currentWeapon].transform.parent = weaponParent[playerNo].transform;
    }

    void ResetWeapon()
    {
        if (pview.IsMine) MetaManager.isFighting = false;
        weapons[currentWeapon].transform.parent = weaponObj.transform;
        // weapons[currentWeapon].transform.localPosition = weaponLastPoz;
        //weapons[currentWeapon].transform.localRotation = weaponLastRot;
        weapons[currentWeapon].transform.localPosition = weaponStartPosz[currentWeapon];
        weapons[currentWeapon].transform.localRotation = weaponStartRotz[currentWeapon];

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

    public void VisitVirtualWorld()
    {

        if (SingletonDataManager.insta.otherPlayerNFTData != null) SingletonDataManager.insta.otherPlayerNFTData.Clear();
        SingletonDataManager.insta.otherPlayerNFTData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MyMetadataNFT>>(pview.Owner.CustomProperties["virtualworld"].ToString());
        Debug.Log("data" + pview.Owner.CustomProperties["virtualworld"].ToString());

        if (SingletonDataManager.insta.otherPlayerNFTData != null) UIManager.insta.VisitOtherPlayerVirtualWorld();
        else MessaeBox.insta.showMsg("No virtual world item", true);



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

    #region Balloon Shoot Area

    [SerializeField] bool inShootingMode = false;    
    [SerializeField] float totalShootTime;
    [SerializeField] float currentShootTime;
    [SerializeField] int balloonBursted;   


    public void HitBalloon()
    {
        balloonBursted ++;
        Text_shootCounter.text = "Score : " + balloonBursted.ToString();
    }
    private void GoToShootMode()
    {
        if (!inShootingMode)
        {
            AudioManager.insta.playSound(10);
            AudioManager.insta.playSound(12);

            MetaManager.isShooting = true;
            //MetaManager.insta.ShootArea.GetComponent<SphereCollider>().isTrigger = false;
            shootingAreaBtn.gameObject.SetActive(false);
            shootBulletBtn.gameObject.SetActive(true);
            crossHair.SetActive(true);
            balloonBursted = 0;

            CameraSwitcher.SwitchCamera(MetaManager.insta.fpsCam);
            //StartCoroutine(LerpVector_Cam_Offset(new Vector3(7.8f, 0.8f, 2.25f), 0.3f));         

            currentShootTime = totalShootTime;
            inShootingMode = true;

            SendShootAreaCode(pview.Owner.UserId,true);
            ToggleGun(true);
            StartCoroutine(StopShootingMode(totalShootTime));
        }
    }
    private void ExitShootingMode()
    {
        if (inShootingMode)
        {
            CameraSwitcher.SwitchCamera(MetaManager.insta.playerCam);

            if (balloonBursted >= 5 && SingletonDataManager.insta)
            {
                SingletonDataManager.userData.score++;
                SingletonDataManager.insta.UpdateUserDatabase();
            }

            AudioManager.insta.playSound(11);
            MetaManager.isShooting = false;

            shootBulletBtn.gameObject.SetActive(false);
            

            //StartCoroutine(LerpVector_Cam_Offset(new Vector3(1, 0, 0), 0.3f));
            
            inShootingMode = false;
            crossHair.SetActive(false);
            SendShootAreaCode(pview.Owner.UserId, false);
            ToggleGun(false);
            Text_shootTimer.gameObject.SetActive(false);
            Text_shootCounter.gameObject.SetActive(false);
        }
    }
    IEnumerator LerpVector_Cam_Offset(Vector3 position, float time)
    {
        Vector3 start = followCam.ShoulderOffset;
        Vector3 end = position;
        float t = 0;


        while (t < 1)
        {
            yield return null;
            t += Time.deltaTime / time;
            followCam.ShoulderOffset = Vector3.Lerp(start, end, t);
        }
        followCam.ShoulderOffset = end;

    }
    IEnumerator StopShootingMode(float time)
    {
        Text_shootTimer.gameObject.SetActive(true);
        Text_shootCounter.gameObject.SetActive(true);
        Text_shootTimer.text ="Remaining time : " +  time.ToString();
        Text_shootCounter.text ="Score : " + "0";
        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time -= 1;
            Text_shootTimer.text = "Remaining time : " + time.ToString();
        }

        ExitShootingMode();
    }

    private void ShootBullet()
    {
        AudioManager.insta.playSound(13);
        Rigidbody bullet_rb = Instantiate(bullet_prefab, bullet_start_pos.position,Quaternion.LookRotation( (aimWorldPos-bullet_start_pos.position).normalized)).GetComponent<Rigidbody>();
        bullet_rb.AddForce(bullet_rb.transform.forward * bullet_speed, ForceMode.Impulse);
        shootBulletBtn.interactable = false;
        myAnim.SetBool("pistol", true);
        tController.ResetRotation(bullet_rb.transform.eulerAngles.y,0.1f);
        StartCoroutine(waitForBulletReset());
    }
    
    
    IEnumerator waitForBulletReset()
    {
        yield return new WaitForSeconds(0.35f);
        myAnim.SetBool("pistol", false);
        shootBulletBtn.interactable = true;

    }
    private void StopShooting()
    {
        MetaManager.insta.ShootArea.GetComponent<SphereCollider>().isTrigger = true;
    }

    void ToggleGun(bool enableGun)
    {
        if (!pview.IsMine)
        {
            inShootingMode = enableGun;
        }
        
        if (enableGun)
        {
            Debug.Log("ENABLE GUN HERE");

            
            weaponObj.SetActive(true);
            /* weaponLastPoz = weapons[2].transform.localPosition;
             weaponLastRot = weapons[2].transform.localRotation;*/

           /* weapons[2].transform.localPosition = weaponStartPosz[2];
            weapons[2].transform.localRotation = weaponStartRotz[2];*/

            weapons[2].SetActive(true);
            weapons[2].transform.parent = weaponParent[playerNo].transform;
        }
        else
        {
            weapons[2].transform.parent = weaponObj.transform;
            weapons[2].transform.localPosition= weaponStartPosz[2];
            weapons[2].transform.localRotation= weaponStartRotz[2];
            

            weapons[2].SetActive(false);
            weaponObj.SetActive(false);
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
                    SingletonDataManager.userData.score++;
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

        if (pview.IsMine)
        {
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
    public const byte ShootAreaCode = 3;

    private void SendFightAction(bool _action, string _p1uid, string _p2uid)
    {
        Debug.Log("SendFightAction OnEvent");
        object[] content = new object[] { _action, _p1uid, _p2uid }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(FightEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
    private void SendShootAreaCode( string _p_uid,bool enableGun)
    {
        Debug.Log("SendFightAction OnEvent");
        object[] content = new object[] { _p_uid, enableGun}; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(ShootAreaCode, content, raiseEventOptions, SendOptions.SendReliable);
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
        else if (eventCode == ShootAreaCode) {
            object[] data = (object[])photonEvent.CustomData;            
            for (int i = 1; i <PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                if(PhotonNetwork.PlayerList[i].UserId.Equals((string)data[0]))                
                {   
                    ToggleGun((bool)data[1]);
                }
            }
        }
    }
}
