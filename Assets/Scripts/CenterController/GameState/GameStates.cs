using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Analysis;

public class GameStates
{
    private Type formalState;
    private Dictionary<string,GameObject> pageDic;
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
        formalState = typeof(MenuPage);
        
        //Initialize all the buttons to make them function for transition between scenes
        foreach (var button in ButtonRegister.Instance.ToSettingPage) button.onClick.AddListener(() => { _fsm.TransitionTo<SettingPage>(); });
        foreach (var button in ButtonRegister.Instance.ToTextingPage) button.onClick.AddListener(() => { _fsm.TransitionTo<TextPage>(); });
        foreach (var button in ButtonRegister.Instance.ToPhoneCallPage) button.onClick.AddListener(() => { _fsm.TransitionTo<DialPage>(); });
        foreach (var button in ButtonRegister.Instance.ToFinalRitualPage) button.onClick.AddListener(() => { _fsm.TransitionTo<FinalRitualPage>(); });
        foreach (var button in ButtonRegister.Instance.ToMainMenuPage) button.onClick.AddListener(() => { _fsm.TransitionTo<MenuPage>(); });
        
        //find all the pages
        RegistePage();
        //pull saved infos
    }

    public void Update()
    {
        _fsm.Update();
    }
   
    public void Clear(){}

    #region States switch situation
    public class GameStatesList : FSM<GameStates>.State{}

    #endregion

    #region States detail perform

    public class MenuPage : GameStatesList
    {
        public override void OnEnter()
        {
            GameObject Page;
            Context.pageDic.TryGetValue("MenuPage", out Page);
            
            Debug.Assert(Page.GetComponent<CanvasGroup>());
            Context.CanvasOn(Page.GetComponent<CanvasGroup>());
        }
        
        public override void OnExit()
        {
            GameObject Page;
            Context.pageDic.TryGetValue("MenuPage", out Page);
            
            Debug.Assert(Page.GetComponent<CanvasGroup>());
            Context.CanvasOff(Page.GetComponent<CanvasGroup>());
        }
    }

    public class DialPage : GameStatesList
    {
        public override void OnEnter()
        {
            GameObject Page;
            Context.pageDic.TryGetValue("DialPage", out Page);
            
            Debug.Assert(Page.GetComponent<CanvasGroup>());
            Context.CanvasOn(Page.GetComponent<CanvasGroup>());
        }
        
        public override void OnExit()
        {
            GameObject Page;
            Context.pageDic.TryGetValue("DialPage", out Page);
            
            Debug.Assert(Page.GetComponent<CanvasGroup>());
            Context.CanvasOff(Page.GetComponent<CanvasGroup>());
        }
    }
    
    public class PhoneCallPage : GameStatesList{}

    public class TextPage : GameStatesList
    {
        public override void OnEnter()
        {
            GameObject Page;
            Context.pageDic.TryGetValue("TextPage", out Page);
            
            Debug.Assert(Page.GetComponent<CanvasGroup>());
            Context.CanvasOn(Page.GetComponent<CanvasGroup>());
        }
        
        public override void OnExit()
        {
            GameObject Page;
            Context.pageDic.TryGetValue("TextPage", out Page);
            
            Debug.Assert(Page.GetComponent<CanvasGroup>());
            Context.CanvasOff(Page.GetComponent<CanvasGroup>());
        }
    }
        
    public class FinalRitualPage : GameStatesList{}

    public class SettingPage : GameStatesList
    {
        public override void OnEnter()
        {
            
        }
        
        public override void OnExit()
        {
            
        }
    }

    #endregion

    #region Static functions

    void RegistePage()
    {
        pageDic.Add("TextPage", GameObject.Find("TextingScene"));
        pageDic.Add("MenuPage",GameObject.Find("MainMenuScene"));
        pageDic.Add("DialPage",GameObject.Find("PhoneCallScene"));
    }

    void CanvasOn(CanvasGroup canvas)
    {
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
        //TODO
        //someswitching effect can be added here
        canvas.alpha = 1;
    }
    
    void CanvasOff(CanvasGroup canvas)
    {
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
        //TODO
        //someswitching effect can be added here
        canvas.alpha = 0;
    }

    #endregion
}


