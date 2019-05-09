using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

//figure out how to get rid of this script during summer or before showcase
public class ButtonFix : MonoBehaviour
{    
    public Page whereToGo;
    public GameObject callDemon;
    
    //buttons For phone Call 
    
    //don't hate me later
     
    
    // Start is called before the first frame update
    void Start()
    {
        //callDemon = GameObject.Find("Background");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void goToPage()
    {
        switch (whereToGo)
        {
            case Page.DialPage: // call the demon
            {
                //callDemon.GetComponent<Calling666>().MakeACall();
                Services.gameStates.ChangeGameState<GameStates.DialPage>(new GameStates.DialPage());
                break;
            }
            case Page.MainMenu:
            {
                Services.gameStates.ChangeGameState<GameStates.MenuPage>(new GameStates.MenuPage());
                break;
            }
            case Page.SettingPage:
                Services.gameStates.ChangeGameState<GameStates.SettingPage>(new GameStates.SettingPage());
                break;
            case Page.PhoneCallPage: // demon call you 
            {    
                Services.gameStates.ChangeGameState<GameStates.PhoneCallPage>(new GameStates.PhoneCallPage());
                
                break;
            }
            case Page.TextingPage:
            {
                Services.gameStates.ChangeGameState<GameStates.TextPage>(new GameStates.TextPage());
                break;
            }
        }
    }
    


   
    
}
