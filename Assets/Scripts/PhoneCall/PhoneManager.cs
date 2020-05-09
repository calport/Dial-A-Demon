using System;
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
        _ps.ChangeGameState("Phone_OnCall");
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
        targetPhoneCall?.OnCallPutThrough();
        
        //change visual
        //TODO visual logic
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
        //visually return
        if (!ReferenceEquals(_previousPage, null))
        {
            _ps.ChangeGameState(_previousPage); 
            _previousPage = null;
        }
        
        //if the phone hasn't put through yet, then it's broken
        if (_targetCallAudioPiece==null)
        {
            targetPhoneCall?.ChangePlotState(PlotManager.PlotState.Broke);
            targetPhoneCall = null;
            return;
        }
        else
        {
            //if(!_targetCallAudioPiece.audioSource.isPlaying)
            if (targetPhoneCall != null)
            {
                var timeRatio = _targetCallAudioPiece.audioSource.time / targetPhoneCall.callContent.length;
                if (timeRatio > 0.9f)
                    targetPhoneCall?.ChangePlotState(PlotManager.PlotState.Finished);
                else
                    targetPhoneCall?.ChangePlotState(PlotManager.PlotState.Broke);
                targetPhoneCall = null;
            }
            _targetCallAudioPiece.Stop();
            _targetCallAudioPiece = null;
        }

    }
    
    #region Save
    public void Load(JSONNode jsonObject){}

    public JSONObject Save(JSONObject jsonObject)
    {
        return jsonObject;
    }
     #endregion
    
    
}
