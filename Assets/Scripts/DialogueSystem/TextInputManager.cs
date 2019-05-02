
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInputManager : MonoSingleton<TextInputManager>
{
    public Dictionary<string, KeyboardInput> keyboardArray = new Dictionary<string, KeyboardInput>();
    public Dictionary<string, int> linesChooseDic = new Dictionary<string, int>();
    public Text textBox;
    public Button sendButton;
    public string filePath;
    public bool keyboardOn = false;
    private string[][] _loadFileString;
    private int _line = 0;
    private int _startLine = 0;
    private int _endLine = 0;
    //0 is demon and 1 is player;
    private int _sayingCharactor = 0;
    //wait how long before this line shows on the screen
    private float _waitShowTime = 1f;
    private string _content;
    
    //GameObject for ChatManager 
    public GameObject chatManager;
    private int moveBubbleAmount = 200;
    private List<int> bubbles; 
    
    private void Start()
    {
        loadTextFile();
        chatManager = GameObject.Find("ChatManager");
        
    }

    void Update()
    {

    }

    void loadTextFile()
    {
        _loadFileString = this.GetComponent<TextToArrays>().TextToArray("Text/testFile");
        ReadLineInfo(_line);
    }
    void ReadLineInfo(int line)
    {
        //read info
        keyboardOn = false;
        _startLine = Convert.ToInt32(_loadFileString[line][1]);
        _endLine = Convert.ToInt32(_loadFileString[line][2]);
        _sayingCharactor = Convert.ToInt32(_loadFileString[line][3]);
        _waitShowTime = Convert.ToSingle(_loadFileString[line][4]);
        if (_sayingCharactor == 1)
        {
            _waitShowTime = 0;
        }
        _content = _loadFileString[line][5];
        StartCoroutine(TalkInterfaceInput(_sayingCharactor, _waitShowTime, _content));
    }
    IEnumerator TalkInterfaceInput(int character, float waitTime, string content)
    {
        yield return new WaitForSeconds(waitTime);
        if (character==1)
        {
            //player talk
            //Debug.Log(waitTime);
            Debug.Log(character + ":" + content);
            //Create a bubble box and sent the content(text into the bubble box) 
            PlayerTalks(content);

        }
        else
        {
            //demon talk
            //Debug.Log(waitTime);
            Debug.Log(character + ":" + content);
            DemonTalks(content);
        }
        MoveToNextLine();
    }
    void MoveToNextLine()
    {
        //in the end of the line
        if (_startLine < _loadFileString.GetLength(0))
        {
            Debug.Log(_loadFileString.GetLength(0));
            //find out who says next several things
                    int character = Convert.ToInt32(_loadFileString[_startLine][3]);
                    if (character == 1)
                    {
                        //player
                        if (_startLine > _endLine)
                        {
                            Debug.Log("startLine larger then endLine");
                        }
            
                        for (int i = _startLine; i < _endLine + 1; i++)
                        {
                            string _firstLetter = _loadFileString[i][5];
                            _firstLetter = _firstLetter.Substring(0, 1).ToUpper();
                            KeyboardInput key = keyboardArray[_firstLetter];
                            key.content = _loadFileString[i][5];
                            linesChooseDic.Add(_firstLetter, Convert.ToInt32(_loadFileString[i][0]));
                        }
            
                        keyboardOn = true;
                       
                    }
                    else
                    {
                        _line = _startLine;
                        ReadLineInfo(_line);
                    }
        }
        else
        {
            Debug.Log("end the talk"); 
        }
        
    }

    
    public enum TextBoxState
    {
        NoWords,
        SelectWords,
        ChosenWords,
    }
    public TextBoxState boxState = TextBoxState.NoWords;
    public void TextBoxStateChange(TextBoxState boxStateChange, string words)
    {
        switch (boxStateChange)
        {
            case TextBoxState.NoWords:
                boxState = boxStateChange;
                textBox.text = String.Empty;
                sendButton.interactable = false;
                break;
            case TextBoxState.SelectWords:
                boxState = boxStateChange;
                textBox.text = words;
                Color color = textBox.color;
                color = Color.gray;
                sendButton.interactable = false;
                textBox.color = color;
                break;
            case TextBoxState.ChosenWords:
                boxState = boxStateChange;
                textBox.text = words;
                Color color1 = textBox.color;
                color1 = Color.black;
                textBox.color = color1;
                sendButton.interactable = true;
                break;
        }
    }
    
    
    
    public void SendWords()
    {
        keyboardOn = false;
        //this.GetComponent<Button>().enabled = false;
        Debug.Log(textBox.text);
        TraceLine();
        TextBoxStateChange(TextBoxState.NoWords, " ");
        ResetInfo();
        ReadLineInfo(_line);
    }
    void TraceLine()
    {
        string _firstLetter = textBox.text;
        _firstLetter = _firstLetter.Substring(0, 1).ToUpper();
        _line = linesChooseDic[_firstLetter];
    }
    void ResetInfo()
    {
        Dictionary<string, KeyboardInput>.ValueCollection valueCol = keyboardArray.Values;
                foreach (KeyboardInput key in valueCol)
                {
                    key.content = String.Empty;
                }

                linesChooseDic = null;
    }

    string RearrangeText(string _content)
    {
        int textLength = 15;
        if (_content.Length > textLength)
        {
            
            int x = _content.Length / textLength;
            int i = x;
            while (i > 0)
            {
                _content = _content.Insert(textLength * i - 1, "\n");
                Debug.Log(_content);
                i--;
            }
        }
        return _content;
    }

    private void PlayerTalks(string content)
    {
        
        GameObject newPlayerBox = Instantiate(Resources.Load<GameObject>("Prefabs/MessageBubble_Player"), chatManager.transform);
        //content = RearrangeText(content);
        newPlayerBox.GetComponentInChildren<Text>().text = content;
        
        newPlayerBox.transform.position = new Vector2(transform.position.x,transform.position.y +20 );
        
        //newPlayerBox 
    }

    private void DemonTalks(string content)
    {      
        GameObject newDemonBox = Instantiate(Resources.Load<GameObject>("Prefabs/MessageBubble_Demon"), chatManager.transform);
        //content = RearrangeText(content);
        newDemonBox.GetComponentInChildren<Text>().text = content;
        //bubbles.Add(newDemonBox,);
        chatManager.GetComponent<Transform>().position = new Vector2(transform.position.x,moveBubbleAmount);
        moveBubbleAmount += 100;
        //newDemonBox.GetComponentInParent<Transform>().position =  new Vector2(transform.position.x,transform.position.y+100);
        //newDemonBox.transform.position = new Vector2(transform.position.x,transform.position.y +20 );
    }

    /*void TalkUp()
    {
        foreach (var chatObj in chatManager.GetComponentsInChildren<Image>())
        {
            chatObj.gameObject.transform.position = new Vector2(transform.position.x,transform.position.y +20 );
        }

    }*/
}
