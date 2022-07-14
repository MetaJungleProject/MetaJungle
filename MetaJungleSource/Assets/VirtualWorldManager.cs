using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class VirtualWorldManager : MonoBehaviour
{
    [SerializeField]
    List<bool> userworld = new List<bool>();

    [SerializeField]
    List<GameObject> userworldObj = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < userworld.Count; i++) {
            if (userworld[i]) {
                userworldObj[i].SetActive(true);
            }else
                userworldObj[i].SetActive(false);
        }
        Debug.Log("Check " + JsonConvert.SerializeObject(userworld));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
