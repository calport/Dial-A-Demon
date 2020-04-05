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
    private AudioPiece _phoneCallAudioPiece;
    
    //these are for checking phone call states and making events
    private bool _isPhonePlayLastFrame;
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
        /*//this part is for phone call event
/*        if (!_isPhonePlayLastFrame && _phoneAudioSource.isPlaying)
        {
            _phoneStartTime = DateTime.Now;
            Services.eventManager.Fire(new PhonePickedUp());
        }#1#
        if (_isPhonePlayLastFrame && !ReferenceEquals(_phoneAudioSource, null) && !_phoneCallAudioPiece.isPlaying) 
        {
            if (currrentPhonePlot != null && currrentPhonePlot.plotState != PlotManager.plotState.isBreak &&
                currrentPhonePlot.plotState != PlotManager.plotState.isFinished)
            {
                Services.eventManager.Fire(new PhoneFinished());
            }
        }
        _isPhonePlayLastFrame = _phoneCallAudioPiece.isPlaying;*/
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
        if(currrentPhonePlot.isDemonCall) ps.ChangeGameState("Phone_DemonCall");
        else ps.ChangeGameState("Phone_PlayerCall");
    }

    private void OnPhonePickedUp()
    {
        Debug.Assert(currrentPhonePlot!= null,"The current phone plot is not assigned properly");
        ps.ChangeGameState("Phone_OnCall");
        phoneStartTime = DateTime.Now;
        _phoneCallAudioPiece = new AudioPiece(currrentPhonePlot.callContent,-1);
        _phoneCallAudioPiece.onAudioFinished += () =>
        {
            _phoneCallAudioPiece.Stop();
            if (currrentPhonePlot != null && currrentPhonePlot.plotState != PlotManager.plotState.isBreak &&
                currrentPhonePlot.plotState != PlotManager.plotState.isFinished)
            {
                Services.eventManager.Fire(new PhoneFinished());
            }
        };
        _phoneCallAudioPiece.Play();
        //AudioManager.PlaySound(DefaultAudioSource.PhoneCall,currrentPhonePlot.callContent);
    }

    private void OnPhoneFinished()
    {
        if(!ReferenceEquals(_phoneCallAudioPiece,null)) _phoneCallAudioPiece.Stop();
        ps.ChangeGameState(previousPage);
        previousPage = null;
    }


    #endregion
    
}
