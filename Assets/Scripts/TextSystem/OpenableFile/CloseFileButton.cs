using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DialADemon.Page;
using HedgehogTeam.EasyTouch;
using UnityEngine;

public class CloseFileButton : MonoBehaviour
{
    public OpenFileButton ofb;
    private PageState ps
    {
        get
        {
            return Services.pageState;
        }
    }

    public void OnSimpleTap()
    {
        Destroy(ofb.document);
        ps.TransitToPreviousState();
    }
}
