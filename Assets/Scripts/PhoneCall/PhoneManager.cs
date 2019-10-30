using System;
using System.Collections;
using System.Collections.Generic;
using DialADemon.Page;
using UnityEngine;
using UnityEngine.UI;

public class PhoneManager
{
    public bool isWaitingForPickingUp;
    public bool isHavingPhoneCall
    {
        get { return currrentPhonePlot!=null; }
    }
    public PlotManager.PhonePlot currrentPhonePlot;
    private PageState ps = Services.pageState;
    private PlotManager pm = Services.plotManager;
    private Pages currentPage
    {
        get { return ps.GetCurrentState(); }
    }
    
    //these are for checking phone call states and making events
    private bool _isPhonePlayLastFrame;
    private readonly AudioSource _phoneAudioSource = AudioManager.audioSources[DefaultAudioSource.PhoneCall];
    private DateTime _phoneStartTime;

    // Start is called before the first frame update
    public void Init()
    {
        Services.eventManager.AddHandler<DemonPhoneCallIn>(delegate{ OnDemonPhoneCallIn();});
        Services.eventManager.AddHandler<PlayerPhoneCallOut>(delegate{ OnPlayerPhoneCallOut();});
    }

    // Update is called once per frame
    public void Update()
    {
        if (isWaitingForPickingUp)
        {
            if(currrentPhonePlot.isDemonCall) ps.ChangeGameState(Services.pageState.CSM.stateList.Phone_DemonCall);
            else ps.ChangeGameState(Services.pageState.CSM.stateList.Phone_PlayerCall);
        }
        
        //this part is for phone call event
        if (_isPhonePlayLastFrame == false && _phoneAudioSource.isPlaying == true)
        {
            _phoneStartTime = DateTime.Now;
            Services.eventManager.Fire(new PhoneStart());
        }
        if (_isPhonePlayLastFrame == true && _phoneAudioSource.isPlaying == false)
        {
            var timeRatio = (DateTime.Now - _phoneStartTime).TotalSeconds / currrentPhonePlot.callContent.length;
            if (timeRatio > 0.95)
            {
                Services.eventManager.Fire(new PhoneFinished());
            }
            else
            {
                Services.eventManager.Fire(new PhoneHangedUp());
            }
        }
        _isPhonePlayLastFrame = _phoneAudioSource.isPlaying;
    }

    public void Clear()
    {
        Services.eventManager.RemoveHandler<DemonPhoneCallIn>(delegate{ OnDemonPhoneCallIn();});
        Services.eventManager.RemoveHandler<PlayerPhoneCallOut>(delegate{ OnPlayerPhoneCallOut();});
    }

    #region Events

    private void OnPlayerPhoneCallOut()
    {
        if(currrentPhonePlot.isDemonCall) ps.ChangeGameState(Services.pageState.CSM.stateList.Phone_DemonCall);
        else ps.ChangeGameState(Services.pageState.CSM.stateList.Phone_PlayerCall);
    }
    
    private void OnDemonPhoneCallIn()
    {
        if(currrentPhonePlot.isDemonCall) ps.ChangeGameState(Services.pageState.CSM.stateList.Phone_DemonCall);
        else ps.ChangeGameState(Services.pageState.CSM.stateList.Phone_PlayerCall);
    }

    #endregion
    
    #region Phone Related Buttons

    [RequireComponent(typeof(Button))]
    public class DialButton : MonoBehaviour
    {
        private PhoneManager pm = Services.phoneManager;
        void OnEnable ()
        {
            var btn = gameObject.GetComponent<Button>();
            if (btn)
            {
                btn.onClick.AddListener(delegate() { OnClick(); });
            }

        }

        private void OnDisable()
        {
            var btn = gameObject.GetComponent<Button>();
            if (btn)
            {
                btn.onClick.RemoveListener(delegate() { OnClick(); });
            }
        }

        private void OnClick()
        {
            //TODO create new dial plot for plot manager
            pm.isWaitingForPickingUp = true;
        }
    }

    [RequireComponent(typeof(Button))]
     public class PickButton : MonoBehaviour
     {
         private PhoneManager pm = Services.phoneManager;
         void OnEnable ()
         {
             var btn = gameObject.GetComponent<Button>();
             if (btn)
             {
                 btn.onClick.AddListener(delegate() { OnClick(); });
             }
 
         }
 
         private void OnDisable()
         {
             var btn = gameObject.GetComponent<Button>();
             if (btn)
             {
                 btn.onClick.RemoveListener(delegate() { OnClick(); });
             }
         }
 
         private void OnClick()
         {
             pm.isWaitingForPickingUp = false;
         }
     }
     
     [RequireComponent(typeof(Button))]
     public class HangButton : MonoBehaviour
     {
         private PhoneManager pm = Services.phoneManager;
         void OnEnable ()
         {
             var btn = gameObject.GetComponent<Button>();
             if (btn)
             {
                 btn.onClick.AddListener(delegate() { OnClick(); });
             }

         }

         private void OnDisable()
         {
             var btn = gameObject.GetComponent<Button>();
             if (btn)
             {
                 btn.onClick.RemoveListener(delegate() { OnClick(); });
             }
         }

         private void OnClick()
         {
             pm.isWaitingForPickingUp = false;
             Services.eventManager.Fire(new PhoneHangedUp());
         }
     }
    #endregion

}
