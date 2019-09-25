using System;
using System.Collections;
using System.Collections.Generic;
using DialADemon.Library;
using UnityEngine;
using Ink.Runtime;
using TMPro;
using UnityEngine.UI;

public class TextRunner : MonoBehaviour
{
	[SerializeField] private TextAsset inkJSONAsset;
 	public Story currentStory;
 
 	[SerializeField] private Canvas canvas;
 
 	// UI Prefabs
 	[SerializeField] private Text textPrefab;
 	[SerializeField] private Button buttonPrefab;
 
 	// path of the massage bubble prefabs
 	private string _demonTextBox;
 	private string _playerTextBox;
 	private string _demonContract;
 
 	//A dictionary saves all the keyboard informations
 	public Dictionary<string, Keyboard> keyboard = new Dictionary<string, Keyboard>();
 
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
 
 	public AudioSource demonAudio;
 
 	private int OptionsCollectionLength;
    
    public GameObject pointer;

	public void Awake()
	{
		Services.textManager.textRunner = this;
		// Remove the default message

		_demonTextBox = "Prefabs/MessageBubble_Demon";
		_playerTextBox = "Prefabs/MessageBubble_Player";
		_demonContract = "Prefabs/DemonContract";

		//finding demon texting audio
		demonAudio = GameObject.Find("DemonTexted").GetComponent<AudioSource>();
	}

	public void StartNewStory(TextAsset newStory)
	{
		_ = new Story(inkJSONAsset.text);
		RefreshView();
	}

	public void ContinueStory()
	{
		//check if the story is finished, if not, check the json saving file and continue the old story
	}

	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	void RefreshView()
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
			//Button choice = CreateChoiceView("End of story.\nRestart?");
			//choice.onClick.AddListener(delegate { StartStory(); });
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
		var firstLetter = text.Substring(0, 1).ToUpper();
		var key = keyboard[firstLetter];

		key.isChoice = true;
		key.content = text;
		key.choiceIdex = choice.index;
	}

	// Destroys all the children of this gameobject (all the UI)
	void CleanData()
	{
		int childCount = canvas.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i)
		{
			GameObject.Destroy(canvas.transform.GetChild(i).gameObject);
		}
	}



	#region DemonDialogue
	
	private IEnumerator CreateAIBubble(string text)
	{
		//the dot dot dot effect
		yield return StartCoroutine(TypingDot());

		//creat the real dialogue
		GameObject newDemonBox = GameObject.Instantiate(Resources.Load<GameObject>(_demonTextBox), content.transform);
		newDemonBox.GetComponentInChildren<TextMeshProUGUI>().text = text;
		
		//create audio whenever the demon sends a message
		Debug.Log("demon is speaking");
		demonAudio.Play();

		//wait to scroll make the dialogue roll automatically
		StartCoroutine(waitToScroll());
		RefreshView();
	}

	private IEnumerator TypingDot()
	{
		yield return new WaitForSeconds(0.5f);
		GameObject newDemonBox = Instantiate(Resources.Load<GameObject>(_demonTextBox), content.transform);
		newDemonBox.GetComponentInChildren<TextMeshProUGUI>().text = "...";
		StartCoroutine(waitToScroll());
		yield return new WaitForSeconds(2f);
		Destroy(newDemonBox);
		yield return new WaitForSeconds(0.2f);
		StartCoroutine(waitToScroll());
	}
	
	IEnumerator waitToScroll()
	{
		yield return new WaitForEndOfFrame();
		vertScrollbar.verticalNormalizedPosition = 0f;
	}
	
	#endregion
	
	// called by the sent button to make a selection
	public void CreatePlayerBubble(string text)
	{
			// show text
			GameObject newPlayerBox = Instantiate(Resources.Load<GameObject>(_playerTextBox), content.transform);
			StartCoroutine(waitToScroll());
			newPlayerBox.GetComponentInChildren<TextMeshProUGUI>().text =text;

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
}
