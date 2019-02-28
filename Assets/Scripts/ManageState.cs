using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    MainMenu,
    ProtectionCircle,
    GameSettings,
    OpeningRitual, 
    Texting,
    Contract,
    PhoneDial,
    PhoneRinging,
    Party
    
}

public class ManageState : MonoBehaviour
{
    private GameState currentState;
    
    public GameObject mainMenu;
    public GameObject textingScreen;
    public GameObject phoneDial; 
    
    //Main Menu Buttons
    private Button goToTexting;
    private Button goToDial; 
    
    // Start is called before the first frame update
    void Start()
    {
        //mainMenu = GameObject.Find("MainMenu_UI");
        //textingScreen = GameObject.Find("Keyboard");
        //phoneDial = GameObject.Find("PhoneDial");
        
        currentState = GameState.MainMenu;
        
        //Texting Button
        goToTexting = GameObject.Find("GoToTexting").GetComponent<Button>();
        goToTexting.onClick.AddListener(ToTexting);
        
        //Phone Call Button
        goToDial = GameObject.Find("GoToDial").GetComponent<Button>();
        goToDial.onClick.AddListener(GoToDial);

        //Setting Button

    }

    // Update is called once per frame
    void Update()
    {
        checkGameState();
    }

    public void checkGameState()
    {
        switch (currentState)
        {
            case GameState.MainMenu:
                
                break;
            case GameState.ProtectionCircle:
                break;
            case GameState.GameSettings:
                break;
            case GameState.OpeningRitual:
                break;
            case GameState.Texting:
                mainMenu.SetActive(false);
                textingScreen.SetActive(true);
                break;
            case GameState.Contract:
                break;
            case GameState.PhoneDial:
                mainMenu.SetActive(false);
                phoneDial.SetActive(true);
                break;
            case GameState.PhoneRinging:
                break;
            case GameState.Party:
                break;
        }
    }

    void ToTexting()
    {
        currentState = GameState.Texting;
    }
    
    void GoToDial()
    {
        currentState = GameState.PhoneDial;
    }
}
