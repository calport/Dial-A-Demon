using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPhoneBG : MonoBehaviour
{
    public Image BgCalling, BgAnswered, BgDemonCalling;
    private int waitTime;
    public float TimeLeft = 1000f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerCallingDemon()
    {
        BgCalling.enabled = true;
        BgAnswered.enabled = false;
        BgDemonCalling.enabled = false;
        DialingTime();
        
       
        

    }
    
    public void DemonCallingPlayer()
    {
        BgCalling.enabled = false;
        BgAnswered.enabled = false;
        BgDemonCalling.enabled = true;
    }
    
    public void AnsweredPhone()
    {
        BgCalling.enabled = false;
        BgAnswered.enabled = true;
        BgDemonCalling.enabled = false;
    }

    void DialingTime()
    {
        Debug.Log("Stared the dialing time");
        //This is the amount of time it takes for the Demon to answer
        //in the future we should make this a variable
        //this needs help 
        while (TimeLeft > 0 )
        {
            TimeLeft--;
            //TimeLeft -= Time.deltaTime; 
        }

        if (TimeLeft< 0)
        {
            AnsweredPhone();
        }


    }
}
