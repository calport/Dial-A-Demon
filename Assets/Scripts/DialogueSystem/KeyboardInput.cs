using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Yarn.Unity.Example;

public class KeyboardInput : MonoBehaviour
{
    //the number of the options of this keyboard
    public int choiceNumber = 0;
    // this need to be capitals
    public string keyCode = String.Empty;
    public string content = String.Empty;
    //private TextInput _textInput;
    public GameObject Pointer;
    private bool _pointerIn;
    void Awake()
    {
        
        //_textInput = this.transform.parent.GetComponentInChildren<TextInput>();
        DemonTextDialogueUI.Instance.keyboardArray.Add(keyCode, this);
    }

    // Update is called once per frame
    void Update()
    {
        //change the textbox input when the keyboard is touched 
        if (string.Compare(String.Empty,content)!=0)
        {
             //Touch touch = Input.GetTouch(0);
             if (string.Compare(content,DemonTextDialogueUI.Instance.textBox.text)!=0)
             {
                 if (_pointerIn)
                 {
                     //vibration
                     //TODO
                     DemonTextDialogueUI.Instance.TextBoxStateChange(DemonTextDialogueUI.TextBoxState.ChosenWords,
                         content, choiceNumber);
                     Debug.Log(content);
                 }
                 else
                 {
                     DemonTextDialogueUI.Instance.TextBoxStateChange(DemonTextDialogueUI.TextBoxState.NoWords,
                         content, choiceNumber);
                 }
             }

             /*if (touch.phase == TouchPhase.Stationary)
             {
                 if (DemonTextDialogueUI.Instance.textBox.text != content)
                 {
                     DemonTextDialogueUI.Instance.TextBoxStateChange(DemonTextDialogueUI.TextBoxState.SelectWords, content, choiceNumber);
                     Debug.Log(content);
                 }

                 if (touch.pressure > 1.5f)
                 {
                     //TODO 
                     //vibration
                     DemonTextDialogueUI.Instance.TextBoxStateChange(DemonTextDialogueUI.TextBoxState.ChosenWords, content, choiceNumber );
                 }
                 
             }
             else
             {
                 if (TextInputManager.Instance.boxState == TextInputManager.TextBoxState.SelectWords)
                 {
                     TextInputManager.Instance.TextBoxStateChange(TextInputManager.TextBoxState.NoWords, content);
                 }
             }*/
        }      
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject == Pointer)
        {
            _pointerIn = true;
            Debug.Log("Find pointer");
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == Pointer)
        {
            _pointerIn = false;
            Debug.Log("Find pointer");
        }
    }
}

