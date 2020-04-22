using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HangButton : MonoBehaviour
{
    private readonly PhoneManager _pm = Services.phoneManager;
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
        _pm.Hang();
    }
}
