using Cinemachine;
using MoralisUnity;
using StarterAssets;
using System.Collections.Generic;
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

 
    

}


