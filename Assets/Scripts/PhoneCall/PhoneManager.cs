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
    private AudioSource _phoneCallAudio;
    
    //these are for checking phone call states and making events
    private bool _isPhonePlayLastFrame;
    private AudioSource _phoneAudioSource;
    public DateTime phoneStartTime;

    // Start is called before the first frame update
    public void Init()
    {
        _phoneAudioSource = AudioManager.audioSources[DefaultAudioSource.PhoneCall];
        Services.eventManager.AddHandler<PhoneStart>(delegate{ OnPhoneStart();});
        Services.eventManager.AddHandler<PhonePickedUp>(delegate{ OnPhonePickedUp();});
        Services.eventManager.AddHandler<PhoneHangedUp>(delegate{ OnPhoneFinished();});
        Services.eventManager.AddHandler<PhoneFinished>(delegate{ OnPhoneFinished();});
    }

    public void Start()
    {
        _phoneCallAudio = ps.GetGameState("Phone_OnCall").relatedObj[1].GetComponent<AudioSource>();
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
        if (_isPhonePlayLastFrame && !_phoneCallAudio.isPlaying )
        {
            if (currrentPhonePlot != null && currrentPhonePlot.plotState != PlotManager.plotState.isBreak &&
                currrentPhonePlot.plotState != PlotManager.plotState.isFinished)
            {
                Services.eventManager.Fire(new PhoneFinished());
            }
        }
        _isPhonePlayLastFrame = _phoneCallAudio.isPlaying;
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
        _phoneCallAudio.clip = currrentPhonePlot.callContent;
        _phoneCallAudio.Play();
        //AudioManager.PlaySound(DefaultAudioSource.PhoneCall,currrentPhonePlot.callContent);
    }

    private void OnPhoneFinished()
    {
        _phoneCallAudio.Stop();
        ps.ChangeGameState(previousPage);
        previousPage = null;
    }


    #endregion
    
}
