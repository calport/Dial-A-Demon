using System;
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
            //Commenting these out for now
            /*
            foreach (var button in Services.referenceInfo.ToSettingPage) button.onClick.AddListener(() => { _fsm.TransitionTo<SettingPage>(); });
            foreach (var button in Services.referenceInfo.ToTextingPage) button.onClick.AddListener(() => { _fsm.TransitionTo<TextPage>(); });
            foreach (var button in Services.referenceInfo.ToPhoneCallPage) button.onClick.AddListener(() => { _fsm.TransitionTo<DialPage>(); });
            foreach (var button in Services.referenceInfo.ToFinalRitualPage) button.onClick.AddListener(() => { _fsm.TransitionTo<FinalRitualPage>(); });
            foreach (var button in Services.referenceInfo.ToMainMenuPage) button.onClick.AddListener(() => { _fsm.TransitionTo<MenuPage>(); });
            */
            RegistePage();
            //pull saved infos

            
            
        }

    public void Start()
    {
        foreach (var itemList in Services.referenceInfo.CameraRenderingItem)
        {
            foreach (var item in itemList)
            {
                item.SetActive(false);
            }
        }
        SetInitialScene();        
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
        if (SceneManager.GetActiveScene().isLoaded)
        {
            ((GameStatesList) _fsm.CurrentState).OnSceneChanged();
        }
    }

    public void Clear(){}

    #endregion

    #region States switch situation

    public class GameStatesList : FSM<GameStates>.State
    {
        public virtual void OnSceneChanged(){}
    }

    #endregion

    #region States detail perform

    public class MenuPage : GameStatesList
    {
        public override void OnEnter()
        {
            
            //remember to change
            if (SceneManager.GetActiveScene().buildIndex != Services.referenceInfo.GetSceneWithName("Main"))
            {
                
                SceneManager.LoadSceneAsync(Services.referenceInfo.GetSceneWithName("Main"));
                TransitionTo<SceneChanging>();
            }
            else
            {
                GameObject mainmenu;
                Services.referenceInfo.BigPage.TryGetValue("MainMenu", out mainmenu);
                mainmenu.SetActive(true);
                
                GameObject Page;
                Services.referenceInfo.MenuPage.TryGetValue("MenuPage", out Page);
                Context.CanvasOn(Page.GetComponent<CanvasGroup>());

                foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.MainMenu.GetHashCode()])
                {
                    renderedItem.SetActive(true);
                }
            }
        }
        
        public override void OnExit()
        {
            GameObject mainmenu;
            Services.referenceInfo.BigPage.TryGetValue("MainMenu", out mainmenu);
            mainmenu.SetActive(false);
                
            GameObject Page;
            Services.referenceInfo.MenuPage.TryGetValue("MenuPage", out Page);
            Context.CanvasOff(Page.GetComponent<CanvasGroup>());
            
            foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.MainMenu.GetHashCode()])
            {
                renderedItem.SetActive(false);
            }
        }
        
        public override void OnSceneChanged(){}
    }

    public class SceneChanging : GameStatesList
    {
        public override void OnSceneChanged()
        {
            TransitionToPreviousState();
        }
    }
    
    public class DialPage : GameStatesList
    {
        
        public override void OnEnter()
        {
            
            //remember to change
            if (SceneManager.GetActiveScene().buildIndex != Services.referenceInfo.GetSceneWithName("Main"))
            {
                
                SceneManager.LoadSceneAsync(Services.referenceInfo.GetSceneWithName("Main"));
                TransitionTo<SceneChanging>();
            }
            else
            {
                GameObject mainmenu;
                Services.referenceInfo.BigPage.TryGetValue("MainMenu", out mainmenu);
                mainmenu.SetActive(true);
                Debug.Log("seet true");
                
                GameObject Page;
                Services.referenceInfo.MenuPage.TryGetValue("DialPage", out Page);
                Context.CanvasOn(Page.GetComponent<CanvasGroup>());
                
                foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.DialPage.GetHashCode()])
                {
                    renderedItem.SetActive(true);
                }
            }
        }
        
        public override void OnExit()
        {
            GameObject mainmenu;
            Services.referenceInfo.BigPage.TryGetValue("MainMenu", out mainmenu);
            mainmenu.SetActive(false);
                
            GameObject Page;
            Services.referenceInfo.MenuPage.TryGetValue("DialPage", out Page);
            Context.CanvasOff(Page.GetComponent<CanvasGroup>());
            
            foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.DialPage.GetHashCode()])
            {
                renderedItem.SetActive(false);
            }
        }
       
        public override void OnSceneChanged(){}
    }
    
    public class PhoneCallPage : GameStatesList{}

    public class TextPage : GameStatesList
    {
        public override void OnEnter()
        {
            
            //remember to change
            if (SceneManager.GetActiveScene().buildIndex != Services.referenceInfo.GetSceneWithName("Main"))
            {
                
                SceneManager.LoadSceneAsync(Services.referenceInfo.GetSceneWithName("Main"));
                TransitionTo<SceneChanging>();
            }
            else
            {
                GameObject mainmenu;
                Services.referenceInfo.BigPage.TryGetValue("MainMenu", out mainmenu);
                mainmenu.SetActive(true);
                
                GameObject Page;
                Services.referenceInfo.MenuPage.TryGetValue("TextPage", out Page);
                Context.CanvasOn(Page.GetComponent<CanvasGroup>());
                
                foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.TextingPage.GetHashCode()])
                {
                    renderedItem.SetActive(true);
                }
            }
        }
        
        public override void OnExit()
        {
            GameObject mainmenu;
            Services.referenceInfo.BigPage.TryGetValue("MainMenu", out mainmenu);
            mainmenu.SetActive(false);
                
            GameObject Page;
            Services.referenceInfo.MenuPage.TryGetValue("TextPage", out Page);
            Context.CanvasOff(Page.GetComponent<CanvasGroup>());
            
            foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.TextingPage.GetHashCode()])
            {
                renderedItem.SetActive(false);
            }
        }
        
        public override void OnSceneChanged(){}
    }

    public class FinalRitualPage : GameStatesList
    {
        
        public override void OnEnter()
        {
            
            //remember to change
            if (SceneManager.GetActiveScene().buildIndex != Services.referenceInfo.GetSceneWithName("Main"))
            {
                
                SceneManager.LoadSceneAsync(Services.referenceInfo.GetSceneWithName("Main"));
                TransitionTo<SceneChanging>();
               
            }
            else
            {
                GameObject mainmenu;
                Services.referenceInfo.BigPage.TryGetValue("MainMenu", out mainmenu);
                mainmenu.SetActive(true);
                
                GameObject Page;
                Services.referenceInfo.MenuPage.TryGetValue("FinalRitualPage", out Page);
                Context.CanvasOn(Page.GetComponent<CanvasGroup>());
                
                foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.FinalRitualPage.GetHashCode()])
                {
                    renderedItem.SetActive(true);
                }
            }
        }
        
        public override void OnExit()
        {
            GameObject mainmenu;
            Services.referenceInfo.BigPage.TryGetValue("MainMenu", out mainmenu);
            mainmenu.SetActive(false);
                
            GameObject Page;
            Services.referenceInfo.MenuPage.TryGetValue("FinalRitualPage", out Page);
            Context.CanvasOff(Page.GetComponent<CanvasGroup>());
            
            foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.FinalRitualPage.GetHashCode()])
            {
                renderedItem.SetActive(false);
            }
        }
        public override void OnSceneChanged(){}
    }

    public class SettingPage : GameStatesList
    {
        public override void OnEnter()
        {
            
            //remember to change
            if (SceneManager.GetActiveScene().buildIndex != Services.referenceInfo.GetSceneWithName("Main"))
            {
                
                SceneManager.LoadSceneAsync(Services.referenceInfo.GetSceneWithName("Main"));
                TransitionTo<SceneChanging>();
            }
            else
            {
                GameObject setting;
                Services.referenceInfo.BigPage.TryGetValue("Setting", out setting);
                setting.SetActive(true);
                
                foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.SettingPage.GetHashCode()])
                {
                    renderedItem.SetActive(true);
                }
            }
        }
        
        public override void OnExit()
        {
            GameObject setting;
            Services.referenceInfo.BigPage.TryGetValue("Setting", out setting);
            setting.SetActive(false);
            
            foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.SettingPage.GetHashCode()])
            {
                renderedItem.SetActive(true);
            }
        }

        public override void OnSceneChanged(){}
    }
    
    public class OpeningRitualPage : GameStatesList
    {
        public override void OnEnter()
        {
            
            //remember to change
            if (SceneManager.GetActiveScene().buildIndex != Services.referenceInfo.GetSceneWithName("OpeningRitual"))
            {
                
                SceneManager.LoadSceneAsync(Services.referenceInfo.GetSceneWithName("OpeningRitual"));
                TransitionTo<SceneChanging>();
            }
        }
        
        public override void OnSceneChanged(){}
    }

    #endregion

    #region Static functions

    void RegistePage()
    {
        if(GameObject.Find("MenuPage") && !pageDic.ContainsKey("MenuPage")) pageDic.Add("MenuPage",GameObject.Find("MenuPage"));
        if(GameObject.Find("TextingPage") && !pageDic.ContainsKey("TextPage")) pageDic.Add("TextPage", GameObject.Find("TextingPage"));
        if(GameObject.Find("DialPage") && !pageDic.ContainsKey("DialPage")) pageDic.Add("DialPage",GameObject.Find("DialPage"));
        if(GameObject.Find("DemonCallingPage") && !pageDic.ContainsKey("DemonCallingPage")) pageDic.Add("DemonCallingPage",GameObject.Find("DemonCallingPage"));
        if(GameObject.Find("FinalRitualPage") && !pageDic.ContainsKey("FinalRitualPage")) pageDic.Add("FinalRitualPage",GameObject.Find("FinalRitualPage"));
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
        if (SceneManager.GetActiveScene().buildIndex == Services.referenceInfo.GetSceneWithName("OpeningRitual")){_fsm.TransitionTo<OpeningRitualPage>(); return;}
        
        _fsm.TransitionTo<OpeningRitualPage>();
    }
    
    #endregion
}


