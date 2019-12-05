using System;
using System.Collections;
using System.Collections.Generic;
using DialADemon.Page;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PageChangeButton : MonoBehaviour
{
    [SerializeField] string pageName;
    [SerializeField] private bool transferToPrevious = false;
    [SerializeField] private bool isWithEffect = false;
    
    [HideInInspector] public delegate void ArtEffect();
    [HideInInspector] public ArtEffect artEffect = new ArtEffect(() => {});
    
    // Use this for initialization
    void OnEnable ()
    {
        var btn = gameObject.GetComponent<Button>(); 
        btn.onClick.AddListener(delegate() { OnClickEffect(); });

        }

    private void OnDisable()
    {
        var btn = gameObject.GetComponent<Button>();
        btn.onClick.RemoveListener(delegate() { OnClickEffect(); });
    }

    public void OnClickEffect()
    {
        if(isWithEffect) artEffect.Invoke();
        else OnClick();
    }
    
    public void OnClick()
    {
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
  