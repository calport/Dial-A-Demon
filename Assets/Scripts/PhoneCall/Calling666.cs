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
    private Text dialText;
    
    //This should be set to true when there are three 6s
    private int dialed6;

    public GameObject phoneCallScript;

    

    
    // Start is called before the first frame update
    void Start()
    {
        //Find all the buttons Add Dial6 as Listener
        for (int i = 0; i <numberButtons.Length; i++)
        {
           numberButtons[i].onClick.AddListener(Dial6);
        }
        
        callButton.onClick.AddListener(CallButtonClick);
        phoneCallScript.GetComponent<SettingPhoneBG>();
   
      
        //This is where the 6s will be added
        dialText = GameObject.Find("DialText").GetComponent<Text>();
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
            dialText.text = dialText.text+ "6";
            
        } 
        
        if (dialed6 > 1) //when you have 666 you can call
        {
            callButton.interactable = true;
        }
        dialed6++; 
    }

    void CallButtonClick()
    {

       // phoneCallScript.GetComponent<SettingPhoneBG>().BgCalling(); 
      
       
        Debug.Log("call demon");
    }


}
