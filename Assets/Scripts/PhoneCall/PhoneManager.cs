using System;
using System.Collections;
using System.Collections.Generic;
using DialADemon.Library;
using DialADemon.Page;
using UnityEngine;
using UnityEngine.UI;

public class PhoneManager
{
    public PlotManager.PhonePlot currrentPhonePlot;
    private PageState ps = Services.pageState;
    private Pages previousPage;
    public Coroutine waitForPickingUp;
    
    //these are for checking phone call states and making events
    private bool _isPhonePlayLastFrame;
    private readonly AudioSource _phoneAudioSource = AudioManager.audioSources[DefaultAudioSource.PhoneCall];
    public DateTime phoneStartTime;

    // Start is called before the first frame update
    public void Init()
    {
        Services.eventManager.AddHandler<PhoneStart>(delegate{ OnPhoneStart();});
        Services.eventManager.AddHandler<PhonePickedUp>(delegate{ OnPhonePickedUp();});
        Services.eventManager.AddHandler<PhoneHangedUp>(delegate{ OnPhoneFinished();});
        Services.eventManager.AddHandler<PhoneFinished>(delegate{ OnPhoneFinished();});
    }

    // Update is called once per frame
    public void Update()
    {
        //this part is for phone call event
/*        if (!_isPhonePlayLastFrame && _phoneAudioSource.isPlaying)
        {
            _phoneStartTime = DateTime.Now;
            Services.eventManager.Fire(new PhonePickedUp());
        }*/
        if (_isPhonePlayLastFrame && !_phoneAudioSource.isPlaying )
        {
            if (currrentPhonePlot != null && currrentPhonePlot.plotState != PlotManager.plotState.isBreak &&
                currrentPhonePlot.plotState != PlotManager.plotState.isFinished)
            {
                Services.eventManager.Fire(new PhoneFinished());
            }
        }
        _isPhonePlayLastFrame = _phoneAudioSource.isPlaying;
    }

    public void Clear()
    {
        if (currrentPhonePlot != null)
        {
            var timeRatio = (DateTime.Now - phoneStartTime).TotalSeconds / currrentPhonePlot.callContent.length;
            if (timeRatio > 0.95)
            {
                Services.eventManager.Fire(new PhoneFinished());
            }
            else
            {
                Services.eventManager.Fire(new PhoneHangedUp());
            }
        }
        Services.eventManager.RemoveHandler<PhoneStart>(delegate{ OnPhoneStart();});
        Services.eventManager.RemoveHandler<PhonePickedUp>(delegate{ OnPhonePickedUp();});
        Services.eventManager.RemoveHandler<PhoneHangedUp>(delegate{ OnPhoneFinished();});
        Services.eventManager.RemoveHandler<PhoneFinished>(delegate{ OnPhoneFinished();});
    }
    
    #region Save
    public void Load(){}

    public void Save()
    {
        
    }
     #endregion
    
    #region Events

    private void OnPhoneStart()
    {
        previousPage = ps.GetCurrentState();
        if(currrentPhonePlot.isDemonCall) ps.ChangeGameState(ps.CSM.stateList.Phone_DemonCall);
        else ps.ChangeGameState(ps.CSM.stateList.Phone_PlayerCall);
    }

    private void OnPhonePickedUp()
    {
        Debug.Assert(currrentPhonePlot!= null,"The current phone plot is not assigned properly");
        ps.ChangeGameState(ps.CSM.stateList.Phone_OnCall);
        phoneStartTime = DateTime.Now;
        AudioManager.PlaySound(DefaultAudioSource.PhoneCall,currrentPhonePlot.callContent);
    }

    private void OnPhoneFinished()
    {
        ps.ChangeGameState(previousPage);
        previousPage = null;
    }


    #endregion
    
    #region Phone Related Buttons

    [RequireComponent(typeof(Button))]
    public class DialButton : MonoBehaviour
    {
        private PhoneManager pm = Services.phoneManager;
        private EventManager em = Services.eventManager;
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
            Debug.Assert(pm.currrentPhonePlot!= null && pm.currrentPhonePlot.GetType().IsSubclassOf(typeof(PlotManager.PlayerPhoneCallPlot)),"the current phone call should be a ");
                        
            em.Fire(new PhoneStart());
            PlotManager.PlayerPhoneCallPlot playerCall = pm.currrentPhonePlot as PlotManager.PlayerPhoneCallPlot;
            pm.waitForPickingUp = CoroutineManager.DoDelayCertainSeconds(
                delegate { em.Fire(new PhonePickedUp()); },
                playerCall.playerWaitTime.Seconds);
        }

    }

    [RequireComponent(typeof(Button))]
     public class PickButton : MonoBehaviour
     {
         private PhoneManager pm = Services.phoneManager;
         private PageState ps = Services.pageState;
         private EventManager em = Services.eventManager;
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
             if (pm.currrentPhonePlot != null)
             {
                 em.Fire(new PhonePickedUp());
             }
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
             StopCoroutine(pm.waitForPickingUp);
             var timeRatio = (DateTime.Now - pm.phoneStartTime).TotalSeconds / pm.currrentPhonePlot.callContent.length;
             if (timeRatio > 0.95)
             {
                 Services.eventManager.Fire(new PhoneFinished());
             }
             else
             {
                 Services.eventManager.Fire(new PhoneHangedUp());
             }
         }
     }
    #endregion

}
