using System;
using System.Collections;

using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

/*public class GameStates
{
    private Type formalState;
    private Dictionary<string,GameObject> pageDic = new Dictionary<string, GameObject>();
    private FSM<GameStates> _fsm;

    public void ChangeGameState<T>(T state) where T: GameStatesList
    {
        _fsm.TransitionTo<T>();
    }

    public void TransitToPreviousState()
    {
        _fsm.TransitionToPreviousState();
    }

    public FSM<GameStates>.State GetCurrentState()
    {
        return _fsm.CurrentState;
    }

    #region Class state dynamic

    public void Init()
        {
            // Initialize the FSM with the context (in this case the critter whose states we're managing)
            _fsm = new FSM<GameStates>(this);
    
            // Set the initial state. Don't forget to do this!!
            foreach (var value in Services.referenceInfo.BigPage.Values)
            {
                value.SetActive(false);
                Debug.Log(value);
            }

            //SetInitialScene();
            formalState = typeof(MenuPage);
            
            //Initialize all the buttons to make them function for transition between scenes
            for(int i = 0; i < Services.referenceInfo.ToSpecifiedPage.Length; i++)
            {
                foreach (var button in Services.referenceInfo.ToSpecifiedPage[i])
                {
                    switch (i)
                    {
                        case 0:
                            button.onClick.AddListener(() => { ChangeGameState<MenuPage>(new MenuPage()); });
                            break;
                        case 1:
                            button.onClick.AddListener(() => { ChangeGameState<TextingPage>(new TextingPage()); });
                            break;
                        case 2:
                            button.onClick.AddListener(() => { ChangeGameState<PhoneCallPage>(new PhoneCallPage()); });
                            break;
                        case 3:
                            button.onClick.AddListener(() => { ChangeGameState<DialPage>(new DialPage()); });
                            break;
                        case 4:
                            button.onClick.AddListener(() => { ChangeGameState<SettingPage>(new SettingPage()); });
                            break;
                        case 5:
                            button.onClick.AddListener(() => { ChangeGameState<OpeningRitualPage>(new OpeningRitualPage()); });
                            break;
                        case 6:
                            button.onClick.AddListener(() => { ChangeGameState<FinalRitualPage>(new FinalRitualPage()); });
                            break;
                        case 7:
                            button.onClick.AddListener(() => { _fsm.TransitionToPreviousState(); });
                            break;
                        
                    }
                }
            }
            
            /*foreach (var button in Services.referenceInfo.ToSettingPage) button.onClick.AddListener(() => { _fsm.TransitionTo<SettingPage>(); });
            foreach (var button in Services.referenceInfo.ToTextingPage) button.onClick.AddListener(() => { _fsm.TransitionTo<TextingPage>(); });
            foreach (var button in Services.referenceInfo.ToPhoneCallPage) button.onClick.AddListener(() => { _fsm.TransitionTo<DialPage>(); });
            foreach (var button in Services.referenceInfo.ToFinalRitualPage) button.onClick.AddListener(() => { _fsm.TransitionTo<FinalRitualPage>(); });
            foreach (var button in Services.referenceInfo.ToMainMenuPage) button.onClick.AddListener(() => { _fsm.TransitionTo<MenuPage>(); });#1#
            
            RegistePage();
            //pull saved infos

            
            
        }

    public void Start()
    {
        /*foreach (var itemList in Services.referenceInfo.CameraRenderingItem)
        {
            foreach (var item in itemList)
            {
                item.SetActive(false);
            }
        }#1#
        SetInitialScene();        
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
                

                /*foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.MainMenu.GetHashCode()])
                {
                    renderedItem.SetActive(true);
                }#1#

                //if(Services.gameStates._fsm.PreviousState.GetType() == typeof(OpeningRitualPage)) Services.textManager.DialogueSys.GetComponent<DialogueRunner>().StartDialogue();
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
            
            /*foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.MainMenu.GetHashCode()])
            {
                renderedItem.SetActive(false);
            }#1#
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
                
                /*foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.DialPage.GetHashCode()])
                {
                    renderedItem.SetActive(true);
                }#1#
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
            
            /*foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.DialPage.GetHashCode()])
            {
                renderedItem.SetActive(false);
            }#1#
        }
       
        public override void OnSceneChanged(){}
    }

    public class PhoneCallPage : GameStatesList
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
                Services.referenceInfo.MenuPage.TryGetValue("PhoneCallPage", out Page);
                Context.CanvasOn(Page.GetComponent<CanvasGroup>());
                
                /*foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.PhoneCallPage.GetHashCode()])
                {
                    renderedItem.SetActive(true);
                }#1#
            }
        }
        
        public override void OnExit()
        {
            GameObject mainmenu;
            Services.referenceInfo.BigPage.TryGetValue("MainMenu", out mainmenu);
            mainmenu.SetActive(false);
                
            GameObject Page;
            Services.referenceInfo.MenuPage.TryGetValue("PhoneCallPage", out Page);
            Context.CanvasOff(Page.GetComponent<CanvasGroup>());
            
            /*foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.PhoneCallPage.GetHashCode()])
            {
                renderedItem.SetActive(false);
            }#1#
        }
    }

    public class TextingPage : GameStatesList
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
                Services.referenceInfo.MenuPage.TryGetValue("TextingPage", out Page);
                Context.CanvasOn(Page.GetComponent<CanvasGroup>());
                
                /*foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.TextingPage.GetHashCode()])
                {
                    renderedItem.SetActive(true);
                }#1#
                
                //text state start to function
                //Services.textStates.ChangeGameState<TextStates.NormalText>(new TextStates.NormalText());
            }
        }
        
        public override void OnExit()
        {
            GameObject mainmenu;
            Services.referenceInfo.BigPage.TryGetValue("MainMenu", out mainmenu);
            mainmenu.SetActive(false);
                
            GameObject Page;
            Services.referenceInfo.MenuPage.TryGetValue("TextingPage", out Page);
            Context.CanvasOff(Page.GetComponent<CanvasGroup>());
            
            /*foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.TextingPage.GetHashCode()])
            {
                renderedItem.SetActive(false);
            }
            #1#

            //text state end function
            //Services.textStates.ChangeGameState<TextStates.NotInText>(new TextStates.NotInText());
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
                
                /*foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.FinalRitualPage.GetHashCode()])
                {
                    renderedItem.SetActive(true);
                }#1#
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
            
            /*foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.FinalRitualPage.GetHashCode()])
            {
                renderedItem.SetActive(false);
            }#1#
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
                
                /*foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.SettingPage.GetHashCode()])
                {
                    renderedItem.SetActive(true);
                }#1#
            }
        }
        
        public override void OnExit()
        {
            GameObject setting;
            Services.referenceInfo.BigPage.TryGetValue("Setting", out setting);
            setting.SetActive(false);
            
            /*foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.SettingPage.GetHashCode()])
            {
                renderedItem.SetActive(true);
            }#1#
        }

        public override void OnSceneChanged(){}
    }
    
    public class OpeningRitualPage : GameStatesList
    {
        public override void OnEnter()
        {
            
            //remember to change
            if (SceneManager.GetActiveScene().buildIndex != Services.referenceInfo.GetSceneWithName("Main"))
            {
                
                SceneManager.LoadSceneAsync(Services.referenceInfo.GetSceneWithName("Main"));
                TransitionTo<SceneChanging>();
            }
            
            GameObject openingRitual;
            Services.referenceInfo.BigPage.TryGetValue("OpeningRitual", out openingRitual);
            openingRitual.SetActive(true);
        }

        public override void OnExit()
        {
            GameObject openingRitual;
            Services.referenceInfo.BigPage.TryGetValue("OpeningRitual", out openingRitual);
            openingRitual.SetActive(false);
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
        //canvas.alpha = 1;
        canvas.DOFade(1.0f, 0.2f);
    }
    
    void CanvasOff(CanvasGroup canvas)
    {
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
        //TODO
        //someswitching effect can be added here
        canvas.DOFade(0.0f, 0.2f);
    }
    

    void SetInitialScene()
    {
   
        if (SceneManager.GetActiveScene().buildIndex == Services.referenceInfo.GetSceneWithName("Main")) {ChangeGameState(new OpeningRitualPage()); return;}
        //if (SceneManager.GetActiveScene().buildIndex == Services.referenceInfo.GetSceneWithName("OpeningRitual")){_fsm.TransitionTo<OpeningRitualPage>(); return;}
        
        //_fsm.TransitionTo<OpeningRitualPage>();
    }
    
    #endregion
}


*/
