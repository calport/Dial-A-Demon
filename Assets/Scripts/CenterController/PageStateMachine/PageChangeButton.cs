using System;
using System.Collections;
using System.Collections.Generic;
using DialADemon.Page;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageChangeButton : MonoBehaviour
{
    [SerializeField] string pageName;
    [SerializeField] private bool transferToPrevious = false;
    
    [HideInInspector] public delegate void ArtEffect();
    [HideInInspector]public ArtEffect artEffect;
    
    // Use this for initialization
    void OnEnable ()
    {
        var btn = gameObject.GetComponent<Button>();
        if (btn)
        {
            btn.onClick.AddListener(delegate() { OnClick(); });
        }

    }

    private void OnDisable()
    {
        var btn = gameObject.GetComponent<Button>();
        if (btn)
        {
            btn.onClick.RemoveListener(delegate() { OnClick(); });
        }
    }

    public void OnClick()
    {
        artEffect.Invoke();
        
        if (!transferToPrevious)
        {
            var csm = Services.pageState.CSM;
            IDictionary<string, object> dict = csm.stateList as IDictionary<string, object>;
            object pageObj;
            dict.TryGetValue(pageName, out pageObj);
            Pages page = pageObj as Pages;
            Services.pageState.ChangeGameState(page);
        }
        else
        {
            Services.pageState.TransitToPreviousState();
        }
    }

}
