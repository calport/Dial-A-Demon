﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TextStates
{
    protected GameObject textPage;
    private FSM<TextStates> _fsm;

    public void ChangeGameState<T>(T state) where T : TextStatesList
    {
        _fsm.TransitionTo<T>();
    }

    public void TransitToPreviousState()
    {
        _fsm.TransitionToPreviousState();
    }


    #region Class state dynamic

    public void Init()
    {
        // Initialize the FSM with the context (in this case the critter whose states we're managing)
        _fsm = new FSM<TextStates>(this);

        // Set the initial state. Don't forget to do this!!
        _fsm.TransitionTo<NotInText>();

        Services.referenceInfo.MenuPage.TryGetValue("TextingPage", out textPage);


    }

    public void Start()
    {

    }


    public void Update()
    {
        _fsm.Update();
    }

    public void Clear()
    {
    }

    #endregion

    #region States switch situation

    public class TextStatesList : FSM<TextStates>.State
    {
    }

    #endregion

    #region States detail perform

    public class NotInText : TextStatesList{}

    public class NormalText : TextStatesList
    {
        public override void OnEnter()
        {
            if (Services.gameStates.GetCurrentState().GetType() != typeof(GameStates.TextingPage))
            {
                Services.gameStates.ChangeGameState(new GameStates.TextingPage());
            }
        }
    }

    public class OnFileCheck : TextStatesList
    {
        
        public override void OnEnter()
        {
            if (Services.gameStates.GetCurrentState().GetType() != typeof(GameStates.TextingPage))
            {
                Services.gameStates.ChangeGameState(new GameStates.TextingPage());
            }
            
             Context.CanvasFade(Context.textPage.GetComponent<CanvasGroup>());
             GameObject frontLayer;
             Services.referenceInfo.MenuPage.TryGetValue("FrontLayer", out frontLayer);
             Context.CanvasOn(frontLayer.GetComponent<CanvasGroup>());
             
             foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.TextingPage.GetHashCode()])
             {
                 renderedItem.SetActive(false);
             }
        }
        
        public override void OnExit()
        {
            
            Context.CanvasBack(Context.textPage.GetComponent<CanvasGroup>());
            GameObject frontLayer;
            Services.referenceInfo.MenuPage.TryGetValue("FrontLayer", out frontLayer);
            Context.CanvasOff(frontLayer.GetComponent<CanvasGroup>());
            
            foreach (var renderedItem in Services.referenceInfo.CameraRenderingItem[global::Page.TextingPage.GetHashCode()])
            {
                renderedItem.SetActive(true);
            }
        }
    }
    
    #endregion
    
    void CanvasFade(CanvasGroup canvas)
    {
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
        //TODO
        //someswitching effect can be added here
        //canvas.alpha = 1;
        canvas.DOFade(0.5f, 0.2f);
    }
    
    void CanvasBack(CanvasGroup canvas)
    {
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
        //TODO
        //someswitching effect can be added here
        //canvas.alpha = 1;
        canvas.DOFade(1.0f, 0.2f);
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
}