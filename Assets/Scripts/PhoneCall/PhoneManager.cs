﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DialADemon.Library;
using DialADemon.Page;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PhoneManager
{
    private readonly PageState _ps = Services.pageState;
    
    private List<Coroutine> _phoneRelatedCoroutine = new List<Coroutine>();
    
    //phone plot related info
    private Pages _previousPage;
    public PlotManager.PhoneCall targetPhoneCall;
    private AudioPiece _targetCallAudioPiece;

    // Start is called before the first frame update
    public void Init()
    {
    }

    // Update is called once per frame
    public void Update()
    {
    }

    public void Clear()
    {
        Hang();
    }
    
    public void DialOut(PlotManager.PhoneCall phone = null)
    {
        targetPhoneCall = phone;
        _previousPage = _ps.GetCurrentState();
        _ps.ChangeGameState("Phone_PlayerCall");
        var waitTime = 0f;
        if (phone == null) waitTime = 15f;
        else waitTime = Random.Range(0f, 13f);
        _phoneRelatedCoroutine.Add(CoroutineManager.DoDelayCertainSeconds(
            PhonePutThrough,
            waitTime));
    }

    public void DialIn(PlotManager.PhoneCall phone)
    {
        targetPhoneCall = phone;
        _previousPage = _ps.GetCurrentState();
        _ps.ChangeGameState("Phone_DemonCall");
        var waitTime = 15f;
        _phoneRelatedCoroutine.Add(CoroutineManager.DoDelayCertainSeconds(
            Hang,
            waitTime));

    }

    public void PhonePutThrough()
    {
        //clear coroutine
        foreach (var coroutine in _phoneRelatedCoroutine)
            CoroutineManager.StopCoroutine(coroutine);
        _phoneRelatedCoroutine.Clear();
        
        //plot mark
        targetPhoneCall.OnCallPutThrough();
        
        //change visual
        _ps.ChangeGameState("Phone_OnCall");
        
        //start phone call audioclip
        if (targetPhoneCall != null)
            _targetCallAudioPiece = new AudioPiece(targetPhoneCall.callContent,-1);
        else
        {
            var audio = Services.audioManager.GetAudioClip("NoResponse","PhoneCall/");
            _targetCallAudioPiece = new AudioPiece(audio,-1);
        }
        
        _targetCallAudioPiece.onAudioFinished += () =>
        {
            _ps.ChangeGameState(_previousPage);
            _previousPage = null;
            targetPhoneCall?.ChangePlotState(PlotManager.PlotState.Finished);

            targetPhoneCall = null;
        };
        _targetCallAudioPiece.Play();
    }

    public void Hang()
    {
        if(targetPhoneCall == null) return;

        //visually return
        
        _ps.ChangeGameState(_previousPage);
        _previousPage = null;

        if (ReferenceEquals(_targetCallAudioPiece, null))
        {
            targetPhoneCall.ChangePlotState(PlotManager.PlotState.Broke);
            return;
        }

        //if(!_targetCallAudioPiece.audioSource.isPlaying)
        _targetCallAudioPiece.Stop();
        var timeRatio = _targetCallAudioPiece.audioSource.time/ targetPhoneCall.callContent.length;
        if (timeRatio > 0.95)
            targetPhoneCall.ChangePlotState(PlotManager.PlotState.Finished);
        else
            targetPhoneCall.ChangePlotState(PlotManager.PlotState.Broke);

        targetPhoneCall = null;
    }
    
    #region Save
    public void Load(JSONNode jsonObject){}

    public JSONObject Save(JSONObject jsonObject)
    {
        return jsonObject;
    }
     #endregion
    
    
}
