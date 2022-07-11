using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using StarterAssets;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MyCharacter : MonoBehaviour
{
    [SerializeField] ThirdPersonController tController;

    [SerializeField] Animator myAnim;
    [SerializeField] CharacterController mController;
    [SerializeField] GameObject vCamTarget;
    [SerializeField] PlayerInput _playerInput;
    [SerializeField] StarterAssetsInputs _inputs;

    [SerializeField] GameObject[] myPlayers;
    [SerializeField] TMP_Text usernameText;
    int playerNo;
    public PhotonView pview;
    [SerializeField] GameObject meetObj;

    [SerializeField] GameObject meetUI;



    Photon.Realtime.Player fightReqPlayer;
    //bool isFighting;

    //weapon items
    [SerializeField] GameObject weaponObj;
    [SerializeField] GameObject[] weaponParent;
    [SerializeField] GameObject[] weapons;


    //ui manger
    [SerializeField] GameObject healthUI;
    [SerializeField] Image healthbarIng;
    //int playerHealth = 100;

    private void Start()
    {

        pview = GetComponent<PhotonView>();
        _inputs = GetComponentInParent<StarterAssetsInputs>();
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
        showHealthBar(false);

        playerNo = int.Parse(pview.Owner.CustomProperties["char_no"].ToString());
        myPlayers[playerNo].SetActive(true);
        myAnim = myPlayers[playerNo].GetComponent<Animator>();
        usernameText.text = pview.Owner.NickName;


    }

    private void LateUpdate()
    {


        usernameText.transform.LookAt(MetaManager.insta.myCam.transform);
        usernameText.transform.rotation = Quaternion.LookRotation(MetaManager.insta.myCam.transform.forward);


    }

    private void Update()
    {
        if (tController.Grounded && (mController.velocity.x != 0 || mController.velocity.z != 0))
        {
            myAnim.SetBool("walk", true);
        }
        else
        {
            myAnim.SetBool("walk", false);
        }

        if (_inputs.attack && tController.Grounded)
        {
            myAnim.SetBool("attack", true);
            StartCoroutine(waitForEnd(myAnim.GetCurrentAnimatorStateInfo(0).length));
        }

        if (!tController.Grounded)
            _inputs.attack = false;

        if (pview.IsMine) {
            MetaManager.isAtttacking = _inputs.attack;
        }

    }

    IEnumerator waitForEnd(float _time, int _action = 0)
    {

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(_time);
        AnimationOver();
    }
    public void AnimationOver()
    {
        _inputs.attack = false;
        myAnim.SetBool("attack", false);
    }


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
                }
            }
        }
        else {
            if (!pview.IsMine && (bool)pview.Owner.CustomProperties["isfighting"])
            {
                if (other.CompareTag("Weapon0") && MetaManager.isAtttacking)
                {
                    Debug.Log("Attacked");
                    UpdateHealth();
                    pview.RPC("UpdateHealth", pview.Owner);
                }
            }
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
            }
        }
    }

    #region weapons

    int currentWeapon = 0;
    public void SelectWeapon() {
        Debug.Log("SelectWeapon");
        meetUI.SetActive(false);
        if(pview.IsMine)MetaManager.isFighting = true;
        weaponObj.SetActive(true);
        showHealthBar(true);
        weapons[currentWeapon].SetActive(true);
        weapons[currentWeapon].transform.SetParent(weaponParent[playerNo].transform);
    }

    void ResetWeapon() {
       
        weapons[currentWeapon].transform.SetParent(weaponObj.transform);
        weapons[currentWeapon].SetActive(false);
        weaponObj.SetActive(false);
    }

    #endregion

    #region RPC
    public void RequestFight()
    {
        Debug.Log("RequestFight" + pview.Owner.NickName);

       MetaManager.insta.myPlayer.GetComponent<MyCharacter>().pview.RPC("RequestFightRPC", pview.Owner);
    }
    [PunRPC]
    void RequestFightRPC(PhotonMessageInfo info)
    {
        if (fightReqPlayer != null || MetaManager.isFighting) return;

        Debug.LogFormat("Info: {0} {1}", info.Sender, info.photonView.IsMine);

        fightReqPlayer = info.Sender;
        MetaManager.fighterView = info.photonView;
        MetaManager.fightPlayer = info.photonView.gameObject;
        UIManager.insta.FightReq(info.Sender.ToString());
    }

    public void RequestFightAction(bool _action)
    {
        Debug.Log("RequestFightAction" + pview.Owner.NickName + " | " + pview.IsMine);

        MetaManager.fighterView.RPC("RequestFightActionRPC", fightReqPlayer, _action);
        pview.RPC("RequestFightActionRPC", fightReqPlayer, _action);

        fightReqPlayer = null;
        if (_action)
        {
            SelectWeapon();
            MetaManager.fightPlayer.GetComponent<MyCharacter>().SelectWeapon();

            var hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash["isfighting"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    [PunRPC]
    void RequestFightActionRPC(bool _action, PhotonMessageInfo info)
    {
        if (_action)
        {
            Debug.Log(info.Sender + " is ready to fight " + pview.IsMine);
            SelectWeapon();
            if (pview.IsMine) {
                var hash = PhotonNetwork.LocalPlayer.CustomProperties;
                hash["isfighting"] = true;
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }
        else
            Debug.Log(info.Sender + " rejected fight");
       
    }



    #endregion


    #region Health
    void showHealthBar(bool _show) {
        if (_show)
        {
            healthUI.SetActive(true);
        }
        else {
            healthUI.SetActive(false);
            healthbarIng.fillAmount = 1;
        }
    }

    [PunRPC]
    void UpdateHealth() {
        if (healthbarIng.fillAmount > 0)
        {
            healthbarIng.fillAmount -= 0.1f;
        }
        else {
            showHealthBar(false);
        }
    }
    #endregion
}
