using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calling666 : MonoBehaviour
{
    //button
    public Button[] numberButtons;

    public Button callButton;
    
    //this will be the text in the call
    public Text dialText;
    
    //This should be set to true when there are three 6s
    private int dialed6;


    public Image BgCalling, BgAnswered, BgDemonCalling, BgThankYou;
    private int waitTime;

    public GameObject ringTone;
    public Boolean setCall;

    public GameObject backButton;

    public GameObject pointer, textingUI, chatText;

    public GameObject declineCall, answerCall; 
    
    // Start is called before the first frame update
    void Start()
    {
        //Find all the buttons Add Dial6 as Listener
        for (int i = 0; i <numberButtons.Length; i++)
        {
           numberButtons[i].onClick.AddListener(Dial6);
        }
        
        callButton.onClick.AddListener(CallButtonClick);
   
   
      
        //This is where the 6s will be added
        dialText = dialText.GetComponent<Text>();
        dialText.text = "";
        
        //Should be set to 0 when at the start
        dialed6 = 0; 
             
    }

    public void ClearCall()
    {
        //This is to clear the call number and reset so player can dial 6
        dialText.text = "";
        dialed6 = 0;
        callButton.interactable = false;
    }

    void Dial6()
    {
        //When there is less than 3 6s add a 6 to the text
        if (dialed6 < 3)
        {
            dialText.text = dialText.text+ "6"; //
            
        } 
        
        if (dialed6 > 1) //when you have 666 you can call
        {
            callButton.interactable = true;
        }
        dialed6++; 
    }

    void CallButtonClick()
    {

       PlayerCallingDemon();
      
   
    }
    
    public void PlayerCallingDemon()
    {
        BgCalling.enabled = true;
        BgAnswered.enabled = false;
        BgDemonCalling.enabled = false;
        StartCoroutine(DialingTime());

    }
    
    public void DemonCallingPlayer()
    {
        StartCoroutine(DemonCallYou());
    }
    
    public void AnsweredPhone()
    {
        BgCalling.enabled = false;
        BgAnswered.enabled = true;
        BgDemonCalling.enabled = false;
        gameObject.GetComponent<AudioSource>().Play();
        Debug.Log("Answered Phone");
    }
    
    public void DemonCalled()
    {
        BgCalling.enabled = false;
        BgAnswered.enabled = false;
        BgDemonCalling.enabled = false;
        BgThankYou.enabled = true;
        
       
        Debug.Log("DemonCalling");
    }

    public void SetCallUp()
    {
        Debug.Log("Setting up call");
        setCall = true;
    }

    public void ChangeButton()
    {
        DemonCallingPlayer();
    }

    IEnumerator DemonCallYou()
    {
        yield return new WaitForSeconds(0.5f);
        BgCalling.enabled = false;
        BgAnswered.enabled = false;
        BgDemonCalling.enabled = true;
        pointer.SetActive(false);
        textingUI.SetActive(false);
        declineCall.SetActive(true);
        answerCall.SetActive(true);
        chatText.SetActive(false);
        ringTone.GetComponent<AudioSource>().Play();
        Handheld.Vibrate();
    }

    IEnumerator DialingTime()
    {
        Debug.Log("Stared the dialing time");
        //This is the amount of time it takes for the Demon to answer
        //in the future we should make this a variable
        //this needs help 
        yield return new WaitForSeconds(2);
        Debug.Log("made it past null");
        AnsweredPhone();
    }
    
    


}
