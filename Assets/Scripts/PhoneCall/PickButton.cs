using System.Collections;
using System.Collections.Generic;
using DialADemon.Page;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PickButton : MonoBehaviour
{
    private PhoneManager pm = Services.phoneManager;
    private PageState ps = Services.pageState;
    private EventManager em = Services.eventManager;
    private void OnEnable ()
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
 
    private void OnClick()
    {
        if (pm.currrentPhonePlot != null)
        {
            em.Fire(new PhonePickedUp());
        }
    }
    
    
         
}
