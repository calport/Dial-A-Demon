using System;
using System.Collections;
using System.Collections.Generic;
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

