using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calling666 : MonoBehaviour
{
    //button
    public Button[] numberButtons;

    private Button callButton;
    
    //this will be the text in the call
    private Text dialText;
    
    //This should be set to true when there are three 6s
    private int dialed6;

    //just for milestone 2 demo
    public AudioSource DemonRinging;
    public AudioSource hangingUpSound;
    public GameObject callScreen;
    public Button declineCall; 
    
    // Start is called before the first frame update
    void Start()
    {
        //Find all the buttons Add Dial6 as Listener
        for (int i = 0; i <numberButtons.Length; i++)
        {
           numberButtons[i].onClick.AddListener(Dial6);
        }
      
        //This is where the 6s will be added
        dialText = GameObject.Find("DialText").GetComponent<Text>();
        dialText.text = "";
        
        //Should be set to 0 when at the start
        dialed6 = 0; 
        
        //just for milestone 2 demo 
        declineCall.onClick.AddListener(hangUp);
        
    }

    public void CallingDemon()
    {
       
        callScreen.SetActive(true);
        Debug.Log("started ringing");
        
    }
    
 

    IEnumerator HangUpCall()
    {
        yield return new WaitForSeconds(1);
        DemonRinging.Stop();
        
        callScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Dial6()
    {
        if (dialed6 < 3)
        {
            dialText.text = dialText.text+ "6";
        }     
        dialed6++; 
    }

    void hangUp()
    {
        DemonRinging.Stop();
        hangingUpSound.Play();
        StartCoroutine(HangUpCall());
    }

}
