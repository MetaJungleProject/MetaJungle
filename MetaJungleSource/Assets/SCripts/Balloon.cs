using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;


public class Balloon : MonoBehaviourPun, IPunOwnershipCallbacks
{
    // Start is called before the first frame update
    [SerializeField] MeshRenderer renderer;
    [SerializeField] Material[] materials;
    [SerializeField] Photon.Pun.PhotonView pv;
    private void Awake()
    {
        renderer.material = materials[Random.Range(0, materials.Length)];
        LeanTween.scale(this.gameObject, Vector3.one * 0.5f, 0.5f);
        LeanTween.moveY(this.gameObject, this.transform.position.y + 10f, Random.Range(5f,8f)).setOnComplete(()=> {
            LeanTween.scale(this.gameObject, Vector3.zero, 0.5f).setOnComplete(() =>
            {

                Hit();
            });
        });

    }

    public void Hit()
    {
        LeanTween.cancel(this.gameObject);
        if (pv && pv.IsMine)
        {
            Photon.Pun.PhotonNetwork.Destroy(pv);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Bullet"))
        {
            AudioManager.insta.playSound(14);
            if (base.photonView.IsMine) PhotonNetwork.Destroy(gameObject);
            else base.photonView.RequestOwnership();
            //this.gameObject.SetActive(false);
            Hit();
            MetaManager.insta.myPlayer.GetComponent<MyCharacter>().HitBalloon();
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        // throw new System.NotImplementedException();
        if (targetView != base.photonView) return;
        base.photonView.TransferOwnership(requestingPlayer);

    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        // throw new System.NotImplementedException();
        if (targetView != base.photonView) return;

        PhotonNetwork.Destroy(gameObject);
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
       // throw new System.NotImplementedException();
    }
}
