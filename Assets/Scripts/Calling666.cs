using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calling666 : MonoBehaviour
{
    //button
    private Button numberButton;
    private Button callButton;
    
    //Textbox
    //private TextBox dialTextBox;
    private Text dialText;

    
    //just for milestone 2 demo
    public AudioSource DemonRinging;
    public AudioSource hangingUpSound;
    public GameObject callScreen;
    public Button declineCall; 
    
    // Start is called before the first frame update
    void Start()
    {
        //just for milestone 2 demo 
        declineCall.onClick.AddListener(hangUp);
        
        
        
        numberButton = GameObject.Find("666_button").GetComponent<Button>();
        numberButton.onClick.AddListener(Dial6);
        
        //textbox
        dialText = GameObject.Find("DialText").GetComponent<Text>();
        dialText.text = "";


    }

    public void MakeACall()
    {
        //I'M CUTE DIRTY CODE 
        StartCoroutine(PhoneCall());
    }

    IEnumerator PhoneCall()
    {
        yield return 0; 
        Debug.Log("waited 1 second");
        
        yield return new WaitForSeconds(5);
        Debug.Log("waited for 5 seconds");
        DemonRinging.Play(0);
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
        dialText.text = "help";
    }

    void hangUp()
    {
        DemonRinging.Stop();
        hangingUpSound.Play();
        StartCoroutine(HangUpCall());
    }

}
