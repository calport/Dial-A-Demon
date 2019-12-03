using System;
using System.Collections;
using System.Collections.Generic;
using DialADemon.Library;
using UnityEngine;
using Ink.Runtime;
using TMPro;
using UnityEngine.UI;

public class TextRunnerInfo : MonoSingleton<TextRunnerInfo>
{
	public Dictionary<string, Keyboard> keyboard = new Dictionary<string, Keyboard>();
	//the text box
	[SerializeField]
 	private TextMeshProUGUI _textBox;
    [HideInInspector]
    public TextMeshProUGUI textBox
    {
	    get { return _textBox; }
    }
 
 	//the send button with tells system players' choices
    [SerializeField]
 	private Button _sendButton;
    [HideInInspector]
    public Button sendButton
    {
	    get { return _sendButton; }
    }
 
 	// the scroll bar of the text window
    [SerializeField]
 	private ScrollRect _msgScroll;
    [HideInInspector]
    public ScrollRect msgScroll
    {
	    get { return _msgScroll; }
    }
}
