using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HangButton : MonoBehaviour
{
    private PhoneManager pm = Services.phoneManager;
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
        if(pm.waitForPickingUp!=null) StopCoroutine(pm.waitForPickingUp);
        if (pm.currrentPhonePlot != null)
        {
            var timeRatio = (DateTime.Now - pm.phoneStartTime).TotalSeconds / pm.currrentPhonePlot.callContent.length;
            if (timeRatio > 0.95)
            {
                Services.eventManager.Fire(new PhoneFinished());
            }
            else
            {
                Services.eventManager.Fire(new PhoneHangedUp());
            }
        }
    }
}
