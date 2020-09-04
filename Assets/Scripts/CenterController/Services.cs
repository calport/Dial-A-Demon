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

    private static PlotManager _plotManager;
    public static PlotManager plotManager
    {
        get
        {
            if (_plotManager != null) return _plotManager;
            else
            {
                _plotManager = new PlotManager();
                return _plotManager;
            }
        }
    }
    
    private static PageState _pageState;
    public static PageState pageState
    {
        get
        {
            if (_pageState != null) return _pageState;
            else
            {
                _pageState = new PageState();
                return _pageState;
            }
        }
    }
    
    private static TextManager _textManager;
    public static TextManager textManager
    {
        get
        {
            if (_textManager != null) return _textManager;
            else
            {
                _textManager = new TextManager();
                return _textManager;
            }
        }
    }
    
    private static EventManager _eventManager;
    public static EventManager eventManager
    {
        get
        {
            if (_eventManager != null) return _eventManager;
            else
            {
                _eventManager = new EventManager();
                return _eventManager;
            }
        }
    }

    private static SequenceTaskRunner _textSequenceTaskRunner;
    public static SequenceTaskRunner textSequenceTaskRunner{
        get
        {
            if (_textSequenceTaskRunner != null) return _textSequenceTaskRunner;
            else
            {
                _textSequenceTaskRunner = new SequenceTaskRunner();
                return _textSequenceTaskRunner;
            }
        }
    }

    private static PhoneManager _phoneManager;

    public static PhoneManager phoneManager
    {
        get
        {
            if (_phoneManager != null) return _phoneManager;
            else
            {
                _phoneManager = new PhoneManager();
                return _phoneManager;
            }
        }
    }
//    
//    private static RitualManager _ritualManager;
//
//    public static RitualManager ritualManager
//    {
//        get
//        {
//            if (_ritualManager != null) return _ritualManager;
//            else
//            {
//                _ritualManager = new RitualManager();
//                return _ritualManager;
//            }
//        }
//    }
    
    private static SaveManager _saveManager;
    public static SaveManager saveManager
    {
        get
        {
            if (_saveManager != null) return _saveManager;
            else
            {
                _saveManager = new SaveManager();
                return _saveManager;
            }
        }
    }

    #endregion
    
    #region Util

        private static AudioManager _audioManager;
        public static AudioManager audioManager
        {
            get
            {
                if (_audioManager != null) return _audioManager;
                else
                {
                    _audioManager = new AudioManager();
                    return _audioManager;
                }
            }
        }

    #endregion
    #region Object & Date reference

    public static EasyTouch easyTouch;
    public static Data_Contract dataContract;
    public static Settings settings;

    #endregion

    #region Library reference

    private static CanvasEffect _canvasEffect;
    public static CanvasEffect canvasEffect
    {
        get
        {
            if (_canvasEffect != null) return _canvasEffect;
            else
            {
                _canvasEffect = new CanvasEffect();
                return _canvasEffect;
            }
        }
    }

    #endregion
}
