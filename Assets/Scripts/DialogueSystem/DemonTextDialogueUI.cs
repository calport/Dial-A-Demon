using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using TMPro;
using Object = UnityEngine.Object;

namespace Yarn.Unity.Example {
    
    
    /// Displays dialogue lines to the player, and sends
    /// user choices back to the dialogue system.

    /** Note that this is just one way of presenting the
     * dialogue to the user. The only hard requirement
     * is that you provide the RunLine, RunOptions, RunCommand
     * and DialogueComplete coroutines; what they do is up to you.
     */
    public class DemonTextDialogueUI : Yarn.Unity.DialogueUIBehaviour
    {
        private static DemonTextDialogueUI instance;
        public static DemonTextDialogueUI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Object.FindObjectOfType<DemonTextDialogueUI>();
                }
                return instance;
            }
        }
       
        
        // path of the massage bubble prefabs
        private string _demonTextBox;
        private string _playerTextBox;
        
        //A dictionary saves all the keyboard informations
        public Dictionary<string, KeyboardInput> keyboardArray = new Dictionary<string, KeyboardInput>();
        //the parent of all the chatlog
        public GameObject content;
        
        //the text box
        public Text textBox;
        //the selected choices number;
        public int selectedOption = 50;
        //the send button with tells system players' choices
        public Button sendButton;
        // the scroll bar of the text window
        public ScrollRect vertScrollbar;
        /// A delegate (ie a function-stored-in-a-variable) that
        /// we call to tell the dialogue system about what option
        /// the user selected
        private Yarn.OptionChooser SetSelectedOption;

        private int OptionsCollectionLength;
        private void Awake()
        {
            _demonTextBox = "Prefabs/MessageBubble_Demon";
            _playerTextBox = "Prefabs/MessageBubble_Player";
        }



        public override IEnumerator RunLine(Yarn.Line line)
        {
            GameObject newPlayerBox = Instantiate(Resources.Load<GameObject>(_demonTextBox), content.transform);
            yield return new WaitForEndOfFrame();        
            vertScrollbar.verticalNormalizedPosition = 0f;
            newPlayerBox.GetComponentInChildren<TextMeshProUGUI>().text = line.text;
            yield return new WaitForSeconds(1.0f);
        }

        public override IEnumerator RunOptions(Yarn.Options optionsCollection, Yarn.OptionChooser optionChooser)
        {
            //send all the options to the aiming keyboard
            int i = 0;
            foreach (var optionString in optionsCollection.options)
            {
                string _firstLetter = optionString;
                _firstLetter = _firstLetter.Substring(0, 1).ToUpper();
                KeyboardInput key = keyboardArray[_firstLetter];
              
                key.content = optionString;
                key.choiceNumber = i;
                i++;
            }

            //record the option chooser
            SetSelectedOption = optionChooser;
            
            //record the option collector
            OptionsCollectionLength = optionsCollection.options.Count;
            // Wait until the chooser has been used and then removed (see SetOption below)
            while (SetSelectedOption != null) {
                yield return null;
            }
            
            //reset the choice number
            foreach (var key in keyboardArray.Values)
            {
                key.choiceNumber = 50;
            }
            selectedOption = 50;
        }

        // called by the sent button to make a selection
        public void SendWords()
        {
            Debug.Log("send options");
            Debug.Log(OptionsCollectionLength);
            if (selectedOption < OptionsCollectionLength && textBox!= null)
            {
                Debug.Log("send options");
                // show text
                GameObject newPlayerBox = Instantiate(Resources.Load<GameObject>(_playerTextBox), content.transform);
                StartCoroutine(waitToScroll());       
                vertScrollbar.verticalNormalizedPosition = 0f;
                newPlayerBox.GetComponentInChildren<TextMeshProUGUI>().text = textBox.text;
                
                // Call the delegate to tell the dialogue system that we've
                // selected an option.
                SetSelectedOption(selectedOption);
                
                // Now remove the delegate so that the loop in RunOptions will exit
                SetSelectedOption = null;
                
                // reset the length of options so no info will be sent until next option
                OptionsCollectionLength = 0;
                
                // clear the text box
                TextBoxStateChange(TextBoxState.NoWords, " ", 50);
                                
                //reset all the keyboard to null
                foreach (var key in keyboardArray.Values)
                {
                    key.content = string.Empty;
                }
                
                Debug.Log("keyboardclean");
            }
        }

        IEnumerator waitToScroll()
        {
            yield return new WaitForEndOfFrame();        
            vertScrollbar.verticalNormalizedPosition = 0f;
        }
        
        public override IEnumerator RunCommand(Yarn.Command command)
        {
            // "Perform" the command
            Debug.Log ("Command: " + command.text);
            yield break;
        }

        public override IEnumerator NodeComplete(string nextNode)
        {
            yield break;
        }
        public override IEnumerator DialogueStarted()
        {
            Debug.Log ("Dialogue starting!");
            yield break;
        }
        public override IEnumerator DialogueComplete()
        {
            Debug.Log ("Complete!");
            yield break;
        }
        
        //the state of textbox
        public enum TextBoxState
        {
            NoWords,
            SelectWords,
            ChosenWords,
        }
        
        public TextBoxState boxState = TextBoxState.NoWords;
        
        public void TextBoxStateChange(TextBoxState boxStateChange, string words, int choiceNumber)
        {
            switch (boxStateChange)
            {
                case TextBoxState.NoWords:
                    boxState = boxStateChange;
                    textBox.text = String.Empty;
                    selectedOption = 50;
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
                    selectedOption = choiceNumber;
                    sendButton.interactable = true;
                    break;
            }
        }
    }

}
