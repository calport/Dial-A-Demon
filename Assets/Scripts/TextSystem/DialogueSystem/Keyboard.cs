using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Yarn.Unity.Example;

[RequireComponent(typeof(AudioSource))]
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
    private GameObject _pointedFont;
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
        //_pointedFont = GetComponent<Transform>().GetChild(0).gameObject;
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
            //_pointedFont.SetActive(true);
            if (isChoice)
            {
                tm.textBox.text = content;
                _sendButton.onClick.AddListener(SendChoice);
    #if UNITY_IOS
                Handheld.Vibrate();
                Handheld.Vibrate();
    #endif
                // neonLight.Play(0);
                optionSound.Play();
            }
            _pointerIn = true;
            
            //testing sound
            
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == _pointer)
        {
            //_pointedFont.SetActive(false);
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
        tm.currentStory?.ChooseChoiceIndex(choiceIdex);
        tm.currentTextPlot?.OnSendText();
        tm.CreatePlayerBubble();

        _sendButton.onClick.RemoveListener(SendChoice);
    }
    
    
}

