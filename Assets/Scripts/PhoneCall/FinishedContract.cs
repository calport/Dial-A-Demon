using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishedContract : MonoBehaviour
{
    public bool leftContract = false;

    public GameObject DemonCall; 
    // Start is called before the first frame update
    void Start()
    {
        DemonCall = GameObject.Find("DemonCall");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LeaveContract()
    {
        //this is dirty code checking to see if player left contract
        DemonCall.GetComponent<Calling666>().DemonCallingPlayer();
        leftContract = true;
    }
}
