using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStates
{

    private FSM<GameStates> _fsm;

    public void ChangeGameState<T>(T state) where T: GameStatesList
    {
        _fsm.TransitionTo<T>();
    }

    //********************************
    //*     class state dynamic      *
    //********************************

    public void Init()
    {
        // Initialize the FSM with the context (in this case the critter whose states we're managing)
        _fsm = new FSM<GameStates>(this);

        // Set the initial state. Don't forget to do this!!
        _fsm.TransitionTo<MenuPage>();
    }

    public void Update()
    {
        _fsm.Update();
    }
   
    public void Clear(){}
    
    //********************************
    //*   states switch situation    *
    //********************************
    public class GameStatesList : FSM<GameStates>.State{}
   
    
    //********************************
    //*    states detail perform     *
    //********************************
    public class MenuPage: GameStatesList{}
    
    public class DialPage : GameStatesList{}

    public class PhoneCallPage : GameStatesList{}

    public class TextPage : GameStatesList{}
    
    public class FinalRitualPage : GameStatesList{}
    
    public class SettingPage : GameStatesList{}
}


