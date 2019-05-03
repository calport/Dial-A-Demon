﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Analysis;

public class GameStates
{
    private Type formalState;
    private Dictionary<string,GameObject> pageDic = new Dictionary<string, GameObject>();
    private FSM<GameStates> _fsm;

    public void ChangeGameState<T>(T state) where T: GameStatesList
    {
        _fsm.TransitionTo<T>();
    }

    #region Class state dynamic

    public void Init()
        {
            // Initialize the FSM with the context (in this case the critter whose states we're managing)
            _fsm = new FSM<GameStates>(this);
    
            // Set the initial state. Don't forget to do this!!
            
            SetInitialScene();
            formalState = typeof(MenuPage);
            
            //Initialize all the buttons to make them function for transition between scenes
            foreach (var button in Services.referenceInfo.ToSettingPage) button.onClick.AddListener(() => { _fsm.TransitionTo<SettingPage>(); });
            foreach (var button in Services.referenceInfo.ToTextingPage) button.onClick.AddListener(() => { _fsm.TransitionTo<TextPage>(); });
            foreach (var button in Services.referenceInfo.ToPhoneCallPage) button.onClick.AddListener(() => { _fsm.TransitionTo<DialPage>(); });
            foreach (var button in Services.referenceInfo.ToFinalRitualPage) button.onClick.AddListener(() => { _fsm.TransitionTo<FinalRitualPage>(); });
            foreach (var button in Services.referenceInfo.ToMainMenuPage) button.onClick.AddListener(() => { _fsm.TransitionTo<MenuPage>(); });
            
            RegistePage();
            //pull saved infos
        }

    public void ReInitWhenSceneLoad()
    {
        //remove all the listeners to make sure that no other listeners are here
        //reinit is also mainly only for adding listeners when main page is loaded for the first time
        //mainly write in this way so the code will be clearer
        foreach (var button in Services.referenceInfo.ToSettingPage) button.onClick.RemoveAllListeners();
        foreach (var button in Services.referenceInfo.ToTextingPage) button.onClick.RemoveAllListeners();
        foreach (var button in Services.referenceInfo.ToPhoneCallPage) button.onClick.RemoveAllListeners();
        foreach (var button in Services.referenceInfo.ToFinalRitualPage) button.onClick.RemoveAllListeners();
        foreach (var button in Services.referenceInfo.ToMainMenuPage) button.onClick.RemoveAllListeners();
        
        //then add all the listeners back
        foreach (var button in Services.referenceInfo.ToSettingPage) button.onClick.AddListener(() => { _fsm.TransitionTo<SettingPage>(); });
        foreach (var button in Services.referenceInfo.ToTextingPage) button.onClick.AddListener(() => { _fsm.TransitionTo<TextPage>(); });
        foreach (var button in Services.referenceInfo.ToPhoneCallPage) button.onClick.AddListener(() => { _fsm.TransitionTo<DialPage>(); });
        foreach (var button in Services.referenceInfo.ToFinalRitualPage) button.onClick.AddListener(() => { _fsm.TransitionTo<FinalRitualPage>(); });
        foreach (var button in Services.referenceInfo.ToMainMenuPage) button.onClick.AddListener(() => { _fsm.TransitionTo<MenuPage>(); });
        
        //dont forget to register the page
        pageDic.Clear();
        RegistePage();
    }

    public void Update()
    {
        _fsm.Update();
    }
   
    public void Clear(){}

    #endregion

    #region States switch situation
    public class GameStatesList : FSM<GameStates>.State{}

    #endregion

    #region States detail perform

    public class MenuPage : GameStatesList
    {
        public override void OnEnter()
        {
            //remember to change
            if (SceneManager.GetActiveScene().buildIndex != Services.referenceInfo.GetSceneWithName("Main")) SceneManager.LoadScene(Services.referenceInfo.GetSceneWithName("Main"));
            
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

    public class FinalRitualPage : GameStatesList
    {
        public override void OnEnter()
        {
            GameObject Page;
            Context.pageDic.TryGetValue("FinalRitualPage", out Page);
            
            Debug.Assert(Page.GetComponent<CanvasGroup>());
            Context.CanvasOn(Page.GetComponent<CanvasGroup>());
        }
        
        public override void OnExit()
        {
            GameObject Page;
            Context.pageDic.TryGetValue("FinalRitualPage", out Page);
            
            Debug.Assert(Page.GetComponent<CanvasGroup>());
            Context.CanvasOff(Page.GetComponent<CanvasGroup>());
        }
    }

    public class SettingPage : GameStatesList
    {
        public override void OnEnter()
        {
            if (SceneManager.GetActiveScene().buildIndex != Services.referenceInfo.GetSceneWithName("Setting")) SceneManager.LoadScene(Services.referenceInfo.GetSceneWithName("Setting"));
        }
        
        public override void OnExit()
        {
            
        }
    }
    
    public class OpeningRitualPage : GameStatesList
    {
        public override void OnEnter()
        {
            if (SceneManager.GetActiveScene().buildIndex != Services.referenceInfo.GetSceneWithName("OpeningRitual")) SceneManager.LoadScene(Services.referenceInfo.GetSceneWithName("OpeningRitual"));
        }
        
        public override void OnExit()
        {
           
        }
    }

    #endregion

    #region Static functions

    void RegistePage()
    {
        pageDic.Add("MenuPage",GameObject.Find("MenuPage"));
        pageDic.Add("TextPage", GameObject.Find("TextingPage"));
        pageDic.Add("DialPage",GameObject.Find("DialPage"));
        pageDic.Add("DemonCallingPage",GameObject.Find("DemonCallingPage"));
        pageDic.Add("FinalRitualPage",GameObject.Find("FinalRitualPage"));
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

    void SetInitialScene()
    {
   
        if (SceneManager.GetActiveScene().buildIndex == Services.referenceInfo.GetSceneWithName("Main")) {_fsm.TransitionTo<MenuPage>(); return;}
        if (SceneManager.GetActiveScene().buildIndex == Services.referenceInfo.GetSceneWithName("Setting")) {_fsm.TransitionTo<SettingPage>(); return;}
        if (SceneManager.GetActiveScene().buildIndex == Services.referenceInfo.GetSceneWithName("OpeningRitual")){_fsm.TransitionTo<OpeningRitualPage>(); return;}
        
        _fsm.TransitionTo<MenuPage>();
    }
    #endregion
}


