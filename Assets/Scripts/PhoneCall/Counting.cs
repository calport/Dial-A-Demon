using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Counting : MonoBehaviour
{
    public TextMeshProUGUI countText;
    private bool _toggle;
    private DateTime _startTime;
    // Start is called before the first frame update
    void Start()
    {
        Services.eventManager.AddHandler<ResetForPageChange>(_OnReset);
    }

    // Update is called once per frame
    void Update()
    {
        if (_toggle)
        {
            var span = DateTime.Now - _startTime;
            if(span.Hours<1) 
                countText.text = string.Format("{0:00}:{1:00}", span.Minutes, span.Seconds);
            else
                countText.text = string.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
        }
    }

    private void OnDestroy()
    {
        Services.eventManager.RemoveHandler<ResetForPageChange>(_OnReset);
    }

    private void _OnReset(ResetForPageChange e)
    {
        if (string.Equals(e.toPage.name, "Phone_OnCall"))
        {
            _startTime = DateTime.Now;
            _toggle = true;
        }
        else
            _toggle = false;
    }
}
