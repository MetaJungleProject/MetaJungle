using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] MeshRenderer renderer;
    [SerializeField] Material[] materials;
    private void Awake()
    {
        renderer.material = materials[Random.Range(0, materials.Length)];
        LeanTween.scale(this.gameObject, Vector3.one * 0.5f, 0.5f);
        LeanTween.moveY(this.gameObject, this.transform.position.y + 10f, Random.Range(5f,8f)).setOnComplete(()=> {
            LeanTween.scale(this.gameObject, Vector3.zero, 0.5f).setOnComplete(() =>
            {
                Destroy(this.gameObject);
            });
        });

    }

   

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Bullet"))
        {
            AudioManager.insta.playSound(14);
            this.gameObject.SetActive(false);            
            MetaManager.insta.myPlayer.GetComponent<MyCharacter>().HitBalloon();
        }
    }
}
