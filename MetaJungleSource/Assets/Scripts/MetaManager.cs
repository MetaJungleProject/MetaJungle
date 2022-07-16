using Cinemachine;
using Photon.Pun;
using StarterAssets;
using UnityEngine;
using Newtonsoft.Json;

public class MetaManager : MonoBehaviour
{

    public static MetaManager insta;

    public Transform[] playerPoz;
    public CinemachineVirtualCamera playerCam;
    public UICanvasControllerInput uiInput;
    public GameObject myCam;
    public GameObject myPlayer;


    //public static GameObject fightPlayer;
    //public static Photon.Pun.PhotonView fighterView;
    public static bool isFighting = false;
    public static bool isAtttacking = false;
    public static Photon.Realtime.Player fightReqPlayer;

    public static string _fighterid;




    private void Awake()
    {
        insta = this;
    }

    private void Start()
    {
        if (SingletonDataManager.myNFTData.Count > 0)
        {
            UpdatePlayerWorldProperties();
        }
    }

    public void UpdatePlayerWorldProperties()
    {
        var hash = PhotonNetwork.LocalPlayer.CustomProperties;
        hash["virtualworld"] = (JsonConvert.SerializeObject(SingletonDataManager.myNFTData)).ToString();
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        Debug.Log("Updated UpdatePlayerWorldProperties");
    }

}


