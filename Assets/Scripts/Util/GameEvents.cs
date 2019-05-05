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