using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyboardInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // attach this to a keyboard button
    // this button should be able to click
    private float _touchTime = 0f;
    private bool _countTouchTime = false;
    // this need to be capitals
    public string keyCode = String.Empty;
    public string content;
    //private TextInput _textInput;
    
    void Start()
    {
        
        //_textInput = this.transform.parent.GetComponentInChildren<TextInput>();
        TextInputManager.Instance.keyboardArray.Add(keyCode, this);
    }

    // Update is called once per frame
    void Update()
    {
        if (_countTouchTime && TextInputManager.Instance.keyboardOn && content != String.Empty && Input.touchCount ==1)
        {
            _touchTime += Time.deltaTime;
             Touch touch = Input.GetTouch(0);
             if (touch.phase == TouchPhase.Stationary )
             {
                 /*if (TextInputManager.Instance.boxState != TextInputManager.TextBoxState.NoWords)
                 {               
                     TextInputManager.Instance.TextBoxStateChange(TextInputManager.TextBoxState.SelectWords, content);
                 }
                 
                 if (TextInputManager.Instance.boxState == TextInputManager.TextBoxState.ChosenWords)
                 {
                     if (TextInputManager.Instance.textBox.text != content)
                     {
                         TextInputManager.Instance.TextBoxStateChange(TextInputManager.TextBoxState.SelectWords,
                             content);
                     }
                 }*/
                 if (TextInputManager.Instance.textBox.text != content)
                 {
                     TextInputManager.Instance.TextBoxStateChange(TextInputManager.TextBoxState.SelectWords, content);
                 }

                 if (touch.pressure > 1.5f)
                 {
                     //TODO 
                     //vibration
                     TextInputManager.Instance.TextBoxStateChange(TextInputManager.TextBoxState.ChosenWords, content);
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
    
    // start count time
    public void OnPointerEnter(PointerEventData eventData)
    {
        _countTouchTime = true;
    }
    
    // stop count time
    public void OnPointerExit(PointerEventData eventData)
    {
        _countTouchTime = false;
        _touchTime = 0f;
    }
}

