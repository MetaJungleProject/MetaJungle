using System.Collections.Generic;
using UnityEngine;
public class VirtualWorldManager : MonoBehaviour
{


    [SerializeField]
    List<GameObject> userworldObj = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
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
    }

    public void GoToOpenWorld()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("OpenWorld");
    }


}
