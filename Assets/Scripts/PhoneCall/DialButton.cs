using System.Collections;
using System.Collections.Generic;
using DialADemon.Library;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DialButton : MonoBehaviour
{
    private PhoneManager pm = Services.phoneManager;
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
            
        //TODO create new dial plot for plot manager
        Debug.Assert(pm.currrentPhonePlot!= null && pm.currrentPhonePlot.GetType().IsSubclassOf(typeof(PlotManager.PlayerPhoneCallPlot)),"the current phone call should be a ");
                        
        em.Fire(new PhoneStart());
        PlotManager.PlayerPhoneCallPlot playerCall = pm.currrentPhonePlot as PlotManager.PlayerPhoneCallPlot;
        pm.waitForPickingUp = CoroutineManager.DoDelayCertainSeconds(
            delegate { em.Fire(new PhonePickedUp()); },
            playerCall.playerWaitTime.Seconds);
    }

}
