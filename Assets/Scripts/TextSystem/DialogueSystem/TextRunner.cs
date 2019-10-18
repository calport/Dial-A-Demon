using System;
using System.Collections;
using System.Collections.Generic;
using DialADemon.Library;
using UnityEngine;
using Ink.Runtime;
using TMPro;
using UnityEngine.UI;

public class TextRunner : MonoSingleton<TextRunner>
{
	[SerializeField] private TextAsset inkJSONAsset;
 	public Story currentStory;
    public bool isRunning = false;
    
    //A dictionary saves all the keyboard informations
 	public Dictionary<string, Keyboard> keyboard = new Dictionary<string, Keyboard>();
 
 	
 
 	//the text box
 	public Text textBox;
 
 	//the send button with tells system players' choices
 	public Button sendButton;
 
 	// the scroll bar of the text window
 	public ScrollRect msgScroll;
    private GameObject _content;
    
    private AudioSource demonAudio;
    private TextManager _tm;
    
    [HideInInspector]
    public GameObject pointerTrigger;

	public void Awake()
	{
		_tm = Services.textManager;
		_content = msgScroll.content.gameObject;
		
		//finding demon texting audio
		demonAudio = GameObject.Find("DemonTexted").GetComponent<AudioSource>();
		
	}

	public void Start()
	{
	}

	public void StartNewStory(TextAsset newStory)
	{
		currentStory = new Story(newStory.text);
		RefreshView();
	}

	public void ContinueStory()
	{
		//check if the story is finished, if not, check the json saving file and continue the old story
		RefreshView();
	}

	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	private void RefreshView()
	{
		if (isRunning)
		{
			// Read all the content until we can't continue any more
			if (currentStory.canContinue)
			{
				// Continue gets the next line of the story
				string text = currentStory.Continue();
				// This removes any white space from the text.
				text = text.Trim();
				// Display the text on screen!
				StartCoroutine(CreateAIBubble(text));
			}
			// Display all the choices, if there are any!
			else if (currentStory.currentChoices.Count > 0)
			{
				for (int i = 0; i < currentStory.currentChoices.Count; i++)
				{
					Choice choice = currentStory.currentChoices[i];
					CreateChoiceView(choice);
					// Tell the button what to do when we press it
					//button.onClick.AddListener(delegate { OnClickChoiceButton(choice); });
				}
			}
			// If we've read all the content and there's no choices, the story is finished!
			else
			{
				//end story
				Services.eventManager.Fire(new TextFinished());
				//Button choice = CreateChoiceView("End of story.\nRestart?");
				//choice.onClick.AddListener(delegate { StartStory(); });
			}
		}
	}

	// When we click the choice button, tell the story to choose that choice!
	void OnClickChoiceButton(Choice choice)
	{
		currentStory.ChooseChoiceIndex(choice.index);
		RefreshView();
	}

	// Creates a button showing the choice text
	void CreateChoiceView(Choice choice)
	{
		//send all the options to the aiming keyboard
		string text = choice.text.Trim();

		int i = 0;
		string firstLetter = String.Empty;
		while(!System.Text.RegularExpressions.Regex.IsMatch(firstLetter, @"^[a-zA-Z]$+"))
		{
			firstLetter = text.Substring(i, 1).ToUpper();
			i++;
		}

		var key = keyboard[firstLetter.ToUpper()];

		key.isChoice = true;
		key.content = text;
		key.choiceIdex = choice.index;
	}

	// Destroys all the children of this gameobject (all the UI)
	void CleanData()
	{
		int childCount = _content.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i)
		{
			GameObject.Destroy(_content.transform.GetChild(i).gameObject);
		}
	}



	#region DemonDialogue
	
	private IEnumerator CreateAIBubble(string text)
	{
		//the dot dot dot effect
		yield return StartCoroutine(TypingDot());

		//creat the real dialogue
		if(isRunning) _tm.AddNewMessage(MessageBubbleType.Demon,text,DateTime.Now);
		
		//create audio whenever the demon sends a message
		Debug.Log("demon is speaking");
		demonAudio.Play();

		//wait to scroll make the dialogue roll automatically
		RefreshView();
	}

	private IEnumerator TypingDot()
	{
		yield return new WaitForSeconds(0.5f);
		GameObject newDemonBox = Instantiate(Resources.Load<GameObject>(Services.textManager.DemonTextBox), _content.transform);
		newDemonBox.GetComponentInChildren<TextMeshProUGUI>().text = "...";
		yield return new WaitForSeconds(2f);
		Destroy(newDemonBox);
		yield return new WaitForSeconds(0.2f);
	}

	#endregion
	
	// called by the sent button to make a selection
	public void CreatePlayerBubble(string text)
	{
			// show text
			_tm.AddNewMessage(MessageBubbleType.Player,text,DateTime.Now);

			//send the text to text manager as a record
			Services.textManager.FinishedLog.Add(textBox.text);
			Services.textManager.Speaker.Add(1);
		
			// clear the text box
			textBox.text = String.Empty;
			//reset all the keyboard to null
			foreach (var key in keyboard.Values)
			{
				key.isChoice = false;
			}
			RefreshView();
	}

	public void PauseRunner()
	{
		isRunning = false;
	}

	public void PlayRunner()
	{
		isRunning = true;
	}
}
