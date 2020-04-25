using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DialADemon.Library;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DialButton : MonoBehaviour
{
    private readonly PhoneManager _pm = Services.phoneManager;
    private EventManager em = Services.eventManager;
    private void OnEnable ()
    {
        var btn = gameObject.GetComponent<Button>();
        if (btn)
            btn.onClick.AddListener(_OnClick);

    }

    private void OnDisable()
    {
        var btn = gameObject.GetComponent<Button>();
        if (btn)
            btn.onClick.RemoveListener(_OnClick);
    }

    private void _OnClick()
    {
        //if targeet phone plot exists, that means a call is already started
        if(_pm.targetPhoneCall!=null) return;

        //TODO: check what the phone number is
        
        //start dialing the active player call
        var playerCalls = Services.plotManager.playingPlot.Where(plot => plot is PlotManager.PlayerCall).ToList();
        foreach (var call in playerCalls)
        {
            var phonePlotCall = call as PlotManager.PlayerCall;
            if (!phonePlotCall.isPutThrough)
            {
                _pm.DialOut(phonePlotCall);
                return;
            }
        }
    }

}
