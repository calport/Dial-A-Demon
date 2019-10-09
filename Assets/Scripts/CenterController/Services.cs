using System.Collections;
using System.Collections.Generic;
using DialADemon.Library;
using HedgehogTeam.EasyTouch;
using DialADemon.Page;
//using UnityEditorInternal;
using UnityEngine;

public static class Services
{
    #region Game play reference

    public static Game game;
    public static GameSettings gameSettings;
    public static PlotManager plotManager;

    public static PageState pageState;

    public static TextManager textManager;
    public static EventManager eventManager;

    #endregion

    #region Object & Date reference

    public static ReferenceInfo referenceInfo;
    public static EasyTouch easyTouch;
    public static Data_Contract dataContract;

    #endregion

    #region Library reference

    public static CanvasEffect canvasEffect;

    #endregion
}
