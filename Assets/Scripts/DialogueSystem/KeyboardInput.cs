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
    public string content;
    //private TextInput _textInput;
    
    void Awake()
    {
        
        //_textInput = this.transform.parent.GetComponentInChildren<TextInput>();
        DemonTextDialogueUI.Instance.keyboardArray.Add(keyCode, this);
    }

    // Update is called once per frame
    void Update()
    {
        //change the textbox input when the keyboard is touched 
        if (content != String.Empty && Input.touchCount ==1)
        {
             Touch touch = Input.GetTouch(0);
             if (touch.phase == TouchPhase.Stationary )
             {
                 if (DemonTextDialogueUI.Instance.textBox.text != content)
                 {
                     DemonTextDialogueUI.Instance.TextBoxStateChange(DemonTextDialogueUI.TextBoxState.SelectWords, content, choiceNumber);
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
                 /*if (TextInputManager.Instance.boxState == TextInputManager.TextBoxState.SelectWords)
                 {
                     TextInputManager.Instance.TextBoxStateChange(TextInputManager.TextBoxState.NoWords, content);
                 }*/
             }
        }      
    }

}

