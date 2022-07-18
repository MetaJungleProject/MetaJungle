using System.Collections.Generic;
using UnityEngine;
public class VirtualWorldManager : MonoBehaviour
{


    [SerializeField]
    List<GameObject> userworldObj = new List<GameObject>();
    [SerializeField] Transform playerLocation;
    Transform playerLastLocation;

    // Start is called before the first frame update
    void OnEnable()
    {
        playerLastLocation = MetaManager.insta.myPlayer.transform;

        for (int i = 0; i < userworldObj.Count; i++)
        {
            userworldObj[i].SetActive(false);
            if (SingletonDataManager.isMyVirtualWorld)
            {
                for (int j = 0; j < SingletonDataManager.myNFTData.Count; j++)
                {
                    if (SingletonDataManager.myNFTData[j].itemid == i)
                    {
                        userworldObj[i].SetActive(true);
                    }
                }
            }
            else
            {
                for (int j = 0; j < SingletonDataManager.otherPlayerNFTData.Count; j++)
                {
                    if (SingletonDataManager.otherPlayerNFTData[j].itemid == i)
                    {
                        userworldObj[i].SetActive(true);
                    }
                }
            }
        }

        MetaManager.insta.myPlayer.transform.position = playerLocation.position;
        MetaManager.insta.myPlayer.transform.rotation = playerLocation.rotation;
    }


    private void OnDisable()
    {
        //MetaManager.insta.myPlayer.transform.SetPositionAndRotation(playerLastLocation.position, playerLastLocation.rotation);
        MetaManager.insta.myPlayer.transform.position = playerLastLocation.position;
        MetaManager.insta.myPlayer.transform.rotation = playerLastLocation.rotation;
    }
    public void GoToOpenWorld()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("OpenWorld");
    }


}
