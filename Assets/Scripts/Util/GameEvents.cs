using System;
using System.Collections;
using System.Collections.Generic;
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


public class TextFinished : GameEvent
{
    public DateTime ShootTime = DateTime.MinValue;

    public TextFinished(DateTime shootTime)
    {
        ShootTime = shootTime;
    }
}

public class PhoneStart : GameEvent{}

public class PhonePickedUp : GameEvent{}

public class PhoneHangedUp : GameEvent{}

public class PhoneFinished : GameEvent{}

public class GameCleanStarted : GameEvent{}

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

#endregion


