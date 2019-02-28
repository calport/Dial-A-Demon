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
    
    // Start is called before the first frame update
    void Start()
    {
        currentState = GameState.MainMenu;
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
            
        }
    } 
}
