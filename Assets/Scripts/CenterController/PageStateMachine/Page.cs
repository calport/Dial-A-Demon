using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using DialADemon.Library;


namespace DialADemon.Page
{
    public enum BelongedSystem
    {
        MainMenu = 0,
        Text,
        PhoneCall,
        Setting,
        Ritual,
        ArtEffect
    }

    public enum RenderLayer
    {
        FrontLayer,
        BackLayer
    }

    public enum LoadBehavior
    {
        Plain,
        TransparentLoad,
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
        public CSM<PageState,Pages> CSM
        {
            get { return _csm; }
        }
        private bool CSMInit = false;

        public void ChangeGameState(Pages state)
        {
            if(state == GetCurrentState()) return;
            Services.eventManager.Fire(new Reset());
            _csm.TransitionTo(state);
        }
        
        public void ChangeGameState(string stateName)
        {
            ChangeGameState(GetGameState(stateName));
        }

        public Pages GetGameState(string pageName)
        {
            return _csm.GetGameState(pageName);
        }

        public void TransitToPreviousState()
        {
            _csm.TransitionToPreviousState();
        }

        public Pages GetCurrentState()
        {
            return _csm.CurrentState;
        }

        public void AddState(string name, bool isValid, GameObject[] relatedObj,params Enum[] properties)
        {
            int id = _csm.stateList.Keys.Count;
            _csm.AddState(id, name, isValid, relatedObj, properties);
        }

        #region Class state dynamic

        public void Init()
        {
            // Initialize the CSM with the context (in this case the critter whose states we're managing)
            _csm = new CSM<PageState, Pages>(this);
            
            //initiate  the propertyEnum and all the behavior state
            _csm.Init<PageStatesList>(_propertyEnums);
            //pull saved infos
            
        }

        public void Start()
        {
            //set the initiate state
            ChangeGameState("Menu_Main");
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
            public MainMenu()
            {
                enumId = BelongedSystem.MainMenu;
            }
            public override void OnEnter(){}

            public override void OnExit(){}

            public override void OnSceneChanged(){}
        }
        
        public class PhoneCall : PageStatesList
        {
            public PhoneCall()
            {
                enumId = BelongedSystem.PhoneCall;
            }
            public override void OnEnter(){}

            public override void OnExit(){}

            public override void OnSceneChanged(){}
        }
        
        public class Text : PageStatesList
        {
            public Text()
            {
                enumId = BelongedSystem.Text;
            }
            public override void OnEnter(){}

            public override void OnExit(){}

            public override void OnSceneChanged(){}
        }
        
        public class Setting : PageStatesList
        {
            public Setting()
            {
                enumId = BelongedSystem.Setting;
            }
            public override void OnEnter(){}

            public override void OnExit(){}

            public override void OnSceneChanged(){}
        }
        
        public class Ritual : PageStatesList
        {
            public Ritual()
            {
                enumId = BelongedSystem.Ritual;
            }
            public override void OnEnter(){}

            public override void OnExit(){}

            public override void OnSceneChanged(){}
        }
        
        public class BackLayer : PageStatesList
        {
            public BackLayer()
            {
                enumId = RenderLayer.BackLayer;
            }
            public override void OnEnter(){}

            public override void OnExit(){}

            public override void OnSceneChanged(){}
        }
        
        public class FrontLayer : PageStatesList
        {
            public FrontLayer()
            {
                enumId = RenderLayer.FrontLayer;
            }

            public override void OnEnter()
            {
                foreach (var item in Parent.CurrentState.relatedObj)
                {
                    if (item.CompareTag("PageObj"))
                    {
                        item.transform.parent.gameObject.SetActive(true);
                    }
                }
            }

            public override void OnExit()
            {
                foreach (var item in Parent.CurrentState.relatedObj)
                {
                    if (item.CompareTag("PageObj"))
                    {
                        CoroutineManager.DoOneFrameDelay(delegate
                        {
                            item.transform.parent.gameObject.SetActive(false);
                        });
                    }
                }
            }

            public override void OnSceneChanged(){}
        }

        public class PlainLoad : PageStatesList
        {
            public PlainLoad()
            {
                enumId = LoadBehavior.Plain;
            }
            public override void OnEnter()
            {
                foreach (var item in Parent.CurrentState.relatedObj)
                {
                    if (item.CompareTag("PageObj"))
                    {
                        var cav =item.GetComponent<CanvasGroup>(); 
                        if (cav) Services.canvasEffect.CanvasOn(cav);
                    }

                    if (item.CompareTag("CameraRenderItem"))
                    {
                        item.SetActive(true);
                    }
                }
            }

            public override void OnExit()
            {
                foreach (var item in Parent.CurrentState.relatedObj)
                {
                    if (item.CompareTag("PageObj"))
                    {
                        var cav =item.GetComponent<CanvasGroup>(); 
                        if (cav) Services.canvasEffect.CanvasOff(cav);
                    }

                    if (item.CompareTag("CameraRenderItem"))
                    {
                        item.SetActive(false);
                    }
                }
            }

            public override void OnSceneChanged()
            {
            }
        }
        
        public class TransparentLoad : PageStatesList
        {
            public TransparentLoad()
            {
                enumId = LoadBehavior.TransparentLoad;
            }
            public override void OnEnter()
            {
                foreach (var item in Parent.CurrentState.relatedObj)
                {
                    if (item.CompareTag("PageObj"))
                    {
                        var cav =item.GetComponent<CanvasGroup>(); 
                        if (cav) Services.canvasEffect.CanvasOn(cav);
                    }

                    if (item.CompareTag("CameraRenderItem"))
                    {
                        item.SetActive(true);
                    }
                }
                
                foreach (var item in Parent.PreviousState.relatedObj)
                {
                    if (item.CompareTag("PageObj"))
                    {
                        var cav =item.GetComponent<CanvasGroup>(); 
                        if (cav) Services.canvasEffect.CanvasShow(cav);
                    }

                    if (item.CompareTag("CameraRenderItem"))
                    {
                        item.SetActive(true);
                    }
                }

                foreach (var item in Parent.GetGameState("Mask_Black").relatedObj)
                {
                    if (item.CompareTag("PageObj"))
                    {
                        var cav =item.GetComponent<CanvasGroup>(); 
                        if (cav) Services.canvasEffect.CanvasShow(cav);
                    }
                }
            }

            public override void OnExit()
            {
                foreach (var item in Parent.CurrentState.relatedObj)
                {
                    if (item.CompareTag("PageObj"))
                    {
                        var cav =item.GetComponent<CanvasGroup>(); 
                        if (cav) Services.canvasEffect.CanvasOff(cav);
                    }

                    if (item.CompareTag("CameraRenderItem"))
                    {
                        item.SetActive(false);
                    }
                }
                
                foreach (var item in Parent.PreviousState.relatedObj)
                {
                    if (item.CompareTag("PageObj"))
                    {
                        var cav =item.GetComponent<CanvasGroup>(); 
                        if (cav) Services.canvasEffect.CanvasHide(cav);
                    }

                    if (item.CompareTag("CameraRenderItem"))
                    {
                        item.SetActive(false);
                    }
                }
                
                foreach (var item in Parent.GetGameState("Mask_Black").relatedObj)
                {
                    if (item.CompareTag("PageObj"))
                    {
                        var cav =item.GetComponent<CanvasGroup>(); 
                        if (cav) Services.canvasEffect.CanvasHide(cav);
                    }
                }
            }

            public override void OnSceneChanged()
            {
            }
        }
        
        public class LoadingLoad : PageStatesList
        {
            public LoadingLoad()
            {
                enumId = LoadBehavior.Loading;
            }
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

