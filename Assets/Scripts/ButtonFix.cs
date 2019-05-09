using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

//figure out how to get rid of this script during summer or before showcase
public class ButtonFix : MonoBehaviour
{
    
    public Page whereToGo; 
    
    //buttons For phone Call 
    
    //audio for demon call 
    public AudioSource DemonRinging;
    public AudioSource hangingUpSound;
     
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void goToPage()
    {
        switch (whereToGo)
        {
            case Page.DialPage:
            {
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
            case Page.PhoneCallPage:
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
