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
    public PlotManager.PhonePlot targetPhonePlot;
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
    
    public void DialOut(PlotManager.PhonePlot phone = null)
    {
        targetPhonePlot = phone;
        _previousPage = _ps.GetCurrentState();
        _ps.ChangeGameState("Phone_PlayerCall");
        var waitTime = 0f;
        if (phone == null) waitTime = 15f;
        else waitTime = Random.Range(0f, 13f);
        _phoneRelatedCoroutine.Add(CoroutineManager.DoDelayCertainSeconds(
            PhonePutThrough,
            waitTime));
    }

    public void DialIn(PlotManager.PhonePlot phone)
    {
        targetPhonePlot = phone;
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
        targetPhonePlot.OnCallPutThrough();
        
        //change visual
        _ps.ChangeGameState("Phone_OnCall");
        
        //start phone call audioclip
        if (targetPhonePlot != null)
            _targetCallAudioPiece = new AudioPiece(targetPhonePlot.callContent,-1);
        else
        {
            var audio = Services.audioManager.GetAudioClip("NoResponse","PhoneCall/");
            _targetCallAudioPiece = new AudioPiece(audio,-1);
        }
        
        _targetCallAudioPiece.onAudioFinished += () =>
        {
            _ps.ChangeGameState(_previousPage);
            _previousPage = null;
            targetPhonePlot?.ChangePlotState(PlotManager.PlotState.Finished);

            targetPhonePlot = null;
        };
        _targetCallAudioPiece.Play();
    }

    public void Hang()
    {
        if(targetPhonePlot == null || !_targetCallAudioPiece.audioSource.isPlaying) return;

        _targetCallAudioPiece.Stop();
        _ps.ChangeGameState(_previousPage);
        _previousPage = null;
        var timeRatio = _targetCallAudioPiece.audioSource.time/ targetPhonePlot.callContent.length;
        if (timeRatio > 0.95)
            targetPhonePlot.ChangePlotState(PlotManager.PlotState.Finished);
        else
            targetPhonePlot.ChangePlotState(PlotManager.PlotState.Broke);

        targetPhonePlot = null;
    }
    
    #region Save
    public void Load(JSONNode jsonObject){}

    public JSONObject Save(JSONObject jsonObject)
    {
        return jsonObject;
    }
     #endregion
    
    
}
