using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
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
            this.gameObject.SetActive(false);
            Hit();
            MetaManager.insta.myPlayer.GetComponent<MyCharacter>().HitBalloon();
        }
    }
}
