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


public class TextFinished : GameEvent{}

public class DemonPhoneCallIn : GameEvent{}

public class PlayerPhoneCallOut : GameEvent{}

public class PhoneStart : GameEvent{}

public class PhoneHangedUp : GameEvent{}

public class PhoneFinished : GameEvent{}

public class GameCleanStarted : GameEvent{}

