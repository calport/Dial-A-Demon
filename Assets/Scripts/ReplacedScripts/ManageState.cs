using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    ProtectionCircle,
    GameSettings,
    OpeningRitual, 
    Texting,
    Contract,
    PhoneDial,
    PhoneDialing,
    PhoneRinging,
    Party
    
}

public class ManageState : MonoBehaviour
{
    private GameState currentState;
    
    public GameObject mainMenu;
    public GameObject textingScreen;
    public GameObject phoneDial;
    //public GameObject settingSelect;
    
    //how do I reference more easily the gameobject attached to this script
    
    
    //Main Menu Buttons
    public Button goToTexting;
    public Button goToDial;
    public Button settingsButton;
    public Button backButtonChat;
    public Button backButtonDial;
    
    //calling buttons 
    private Button numberButton;
    private Button callButton;
    
    //Textbox
    //private TextBox dialTextBox;
    private Text dialText;
    
    // Start is called before the first frame update
    void Start()
    {     
        currentState = GameState.MainMenu;
        
        //Texting Button
        goToTexting.onClick.AddListener(ToTexting);
        
        //Phone Call Button
        goToDial.onClick.AddListener(GoToDial);

        //Setting Button
        
        //Back Buttons
        backButtonChat.onClick.AddListener(ToMainMenu);
        backButtonDial.onClick.AddListener(ToMainMenu);
        
       

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
                textingScreen.SetActive(false);
                phoneDial.SetActive(false);
                mainMenu.SetActive(true);
                
                break;
            case GameState.ProtectionCircle:
                break;
            case GameState.GameSettings:
                SceneManager.LoadScene("Menu");
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
                numberButton = GameObject.Find("666_button").GetComponent<Button>();
                numberButton.GetComponentsInChildren<Button>();
                numberButton.onClick.AddListener(Dial6);
        
                //textbox
                dialText = GameObject.Find("DialText").GetComponent<Text>();
               
                
                break;
            case GameState.PhoneDialing:
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

    void ToMainMenu()
    {
        currentState = GameState.MainMenu;
    }

    void Dial6()
    {
        dialText.text = "666";
    }

    public void GoToSettings()
    {
        currentState = GameState.GameSettings;
    }

    public void backToMain()
    {
        SceneManager.LoadScene("Texting");
    }
}
