using System;
using System.Collections;
using System.Collections.Generic;
using DialADemon.Page;
using TimeUtil;
using UnityEngine;

public class SceneChanged : GameEvent
{
    public string ChangeToScene = string.Empty;

    public SceneChanged(string changeToScene)
    {
        ChangeToScene = changeToScene;
    }
}

public class GameCleanStarted : GameEvent{}

public class ResetForPageChange : GameEvent
{
    public Pages toPage;

    public ResetForPageChange(Pages toPage)
    {
        this.toPage = toPage;
    }
}

#region Util Events

public class UtilEvents : GameEvent{}

public class DownCounterStart : UtilEvents
{
    public DownCounter downCounter;

    public DownCounterStart(DownCounter dc)
    {
        downCounter = dc;
    }
}

public class DownCounterStop : UtilEvents
{
    public DownCounter downCounter;

    public DownCounterStop(DownCounter dc)
    {
        downCounter = dc;
    }
}

public class DownCounterBreak : UtilEvents
{
    public DownCounter downCounter;

    public DownCounterBreak(DownCounter dc)
    {
        downCounter = dc;
    }
}

public class PointerOut : UtilEvents
{
    public GameObject obj;

    public PointerOut(GameObject obj)
    {
        this.obj = obj;
    }
}

public class PointerIn : UtilEvents
{
    public GameObject obj;

    public PointerIn(GameObject obj)
    {
        this.obj = obj;
    }
}
#endregion


