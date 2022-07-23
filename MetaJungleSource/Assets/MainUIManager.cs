using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIManager : MonoBehaviour
{

    [SerializeField] GameObject authObj;
    // Start is called before the first frame update
    void Start()
    {
        authObj.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
