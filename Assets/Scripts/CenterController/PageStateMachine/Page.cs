using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


namespace DialADemon.Page
{
    public enum BelongedSystem
    {
        MainMenu = 0,
        Text,
        PhoneCall,
        Setting,
        Ritual
    }

    public enum RenderLayer
    {
        FrontLayer,
        BackLayer
    }

    public enum LoadBehavior
    {
        Plain,
        Loading
    }

    public class Pages : Info
    {
        public GameObject pageObj;
        public BelongedSystem system;
        public RenderLayer layer;
        public LoadBehavior load;

        public Dictionary<Type, Enum> Property = new Dictionary<Type, Enum>
        {
            {typeof(BelongedSystem), default(BelongedSystem)},
            {typeof(RenderLayer), default(BelongedSystem)},
            {typeof(LoadBehavior), default(BelongedSystem)}
        };
    }


    public class PageState
    {
        private Type formalState;
        private readonly List<Enum> _propertyEnums = new List<Enum>{ new BelongedSystem(), new RenderLayer(), new LoadBehavior()};
        private Dictionary<string, GameObject> pageDic = new Dictionary<string, GameObject>();
        private CSM<PageState,Pages> _csm;
        private bool CSMInit = false;

        public void ChangeGameState(Pages state)
        {
            _csm.TransitionTo(state);
        }

        public void TransitToPreviousState()
        {
            _csm.TransitionToPreviousState();
        }

        public Pages GetCurrentState()
        {
            return _csm.CurrentState;
        }

        public void AddState(int id, string name, GameObject[] relatedObj,params Enum[] properties)
        {
            _csm.AddState(id, name, relatedObj, properties);
        }

        #region Class state dynamic

        public void Init()
        {
            // Initialize the CSM with the context (in this case the critter whose states we're managing)
            _csm = new CSM<PageState, Pages>(this);

            /*// Set the initial state. Don't forget to do this!!
            foreach (var value in Services.referenceInfo.BigPage.Values)
            {
                value.SetActive(false);
                Debug.Log(value);
            }

            //SetInitialScene();
            formalState = typeof(MenuPage);*/
            
            //set the initiate state
            ChangeGameState(_csm.stateList.MainPage);

            //initiate  the propertyEnum and all the behavior state
            _csm.Init<PageStatesList>(_propertyEnums);
            //pull saved infos
            
        }

        public void Start()
        {
        }
        
        public void Update()
        {
            _csm.Update();
            /*if (SceneManager.GetActiveScene().isLoaded)
            {
                ((GameStatesList) _fsm.CurrentState).OnSceneChanged();
            }*/
        }

        public void Clear()
        {
        }

        #endregion

        #region States switch situation

        public class PageStatesList : CSM<PageState,Pages>.StateBehavior
        { 
            public virtual void OnSceneChanged(){}
        }

        #endregion

        #region States detail perform

        public class MainMenu : PageStatesList
        {
            public new Enum enumId = BelongedSystem.MainMenu;
            public override void OnEnter(){}

            public override void OnExit(){}

            public override void OnSceneChanged(){}
        }
        
        public class PhoneCall : PageStatesList
        {
            public new Enum enumId = BelongedSystem.PhoneCall;
            public override void OnEnter(){}

            public override void OnExit(){}

            public override void OnSceneChanged(){}
        }
        
        public class Text : PageStatesList
        {
            public new Enum enumId = BelongedSystem.Text;
            public override void OnEnter(){}

            public override void OnExit(){}

            public override void OnSceneChanged(){}
        }
        
        public class Setting : PageStatesList
        {
            public new Enum enumId = BelongedSystem.Setting;
            public override void OnEnter(){}

            public override void OnExit(){}

            public override void OnSceneChanged(){}
        }
        
        public class Ritual : PageStatesList
        {
            public new Enum enumId = BelongedSystem.Ritual;
            public override void OnEnter(){}

            public override void OnExit(){}

            public override void OnSceneChanged(){}
        }
        
        public class BackLayer : PageStatesList
        {
            public new Enum enumId = RenderLayer.BackLayer;
            public override void OnEnter(){}

            public override void OnExit(){}

            public override void OnSceneChanged(){}
        }
        
        public class FrontLayer : PageStatesList
        {
            public new Enum enumId = RenderLayer.FrontLayer;
            public override void OnEnter(){}

            public override void OnExit(){}

            public override void OnSceneChanged(){}
        }

        public class PlainLoad : PageStatesList
        {
            public new Enum enumId = LoadBehavior.Plain;
            public override void OnEnter()
            {
                var cav = Parent.CurrentState.relatedObj[typeof(GameObject)].GetComponent<CanvasGroup>();
                if (cav) Services.canvasEffect.CanvasOn(cav);
            }

            public override void OnExit()
            {
                var cav = Parent.CurrentState.relatedObj[typeof(GameObject)].GetComponent<CanvasGroup>();
                if (cav) Services.canvasEffect.CanvasOn(cav);

            }

            public override void OnSceneChanged()
            {
            }
        }
        
        public class LoadingLoad : PageStatesList
        {
            public new Enum enumId = LoadBehavior.Loading;
            public override void OnEnter()
            {
                
            }

            public override void OnExit()
            {
                
            }

            public override void OnSceneChanged()
            {
            }
        }
        
        #endregion
    }



    /*
    public class PageFunc
    {
        public int currentPage;

        //id start from 1
        public void AddPage(string name, BelongedSystem system, RenderLayer layer)
        {
            Pages newPage = new Pages();
            newPage.name = name;
            newPage.system = system;
            newPage.layer = layer;
            var i = Services.referenceInfo.pageList.Count;
            newPage.id = i;
            Services.referenceInfo.pageList.Add(newPage);
        }
        
    }
*/

}

