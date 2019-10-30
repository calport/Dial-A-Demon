﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Yarn.Unity.Example;

public class Keyboard : MonoBehaviour
{
    //the number of the options of this keyboard
    public int choiceIdex = 0;
    // this need to be capitals
    public string keyCode = String.Empty;
    public bool isChoice = false;
    
    //[HideInInspector]
    public string content = String.Empty;
    public string Content
    {
        get { return content;}}
    
    
    //private TextInput _textInput;
    private GameObject _pointer;
    private TextManager tm;
    private Button _sendButton;
    private bool _pointerIn;

    //Sound that plays over a "choice"
    public AudioSource optionSound;

    void Awake()
    {
        //_textInput = this.transform.parent.GetComponentInChildren<TextInput>();
        TextRunnerInfo.Instance.keyboard.Add(keyCode,this);
    }

    private void Start()
    {
        tm = Services.textManager;
        _pointer = tm.pointerTrigger;
        _sendButton = tm.sendButton;
    }

    // Update is called once per frame
    void Update()
    {
        //change the textbox input when the keyboard is touched 
        if (string.Compare(String.Empty, content)!= 0)
        {
            if (string.Compare(content, Services.textManager.textBox.text) != 0)
            {
                if (_pointerIn)
                {
                    /*DemonTextDialogueUI.Instance.TextBoxStateChange(DemonTextDialogueUI.TextBoxState.ChosenWords,
                        content, _choiceNumber);
                    Handheld.Vibrate();
                    Handheld.Vibrate();
                    //testing sound
                    optionSound.Play();
*/

                }
            }

        }
    }
    
    public void Clear()
    {
        //save data
    }

    private void OnDestroy()
    {
        Clear();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _pointer)
        {
            if (isChoice)
            {
                tm.textBox.text = content;
                _sendButton.onClick.AddListener(SendChoice);
                Handheld.Vibrate();
                Handheld.Vibrate();
            }
            _pointerIn = true;
            
            //testing sound
            optionSound.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == _pointer)
        {
            _pointerIn = false;
            if (isChoice)
            {
                if(string.Compare(tm.textBox.text,content)==0){
                tm.textBox.text = String.Empty;}
                _sendButton.onClick.RemoveListener(SendChoice);
            }
        }
    }

    void SendChoice()
    {   
        //choice idex goes first
        tm.currentStory.ChooseChoiceIndex(choiceIdex);
        tm.CreatePlayerBubble(content);

        _sendButton.onClick.RemoveListener(SendChoice);
    }
    
    
}

