using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using Yarn.Unity;
using Yarn.Unity.Example;

public class TextManager
{
    public TextRunner textRunner;
    // path of the massage bubble prefabs
    private string _demonTextBox;
    private string _playerTextBox;
    private string _demonContract;
    
    public GameObject DialogueSys;
    //this is a place that record all the dialogue lines, so it will be easier to restore the game
    public List<string> FinishedLog = new List<string>();
    //this is a place to record who says the dialogue in finished log, so it will help restore the game, while 0 is demon and 1 is the player
    public List<int> Speaker = new List<int>();

    #region Lifecycle

    public void Init()
    {
        _demonTextBox = "Prefabs/MessageBubble_Demon";
        _playerTextBox = "Prefabs/MessageBubble_Player";
        _demonContract = "Prefabs/DemonContract";

        DialogueSys = DemonTextDialogueUI.Instance.gameObject;
    }

    public void Update()
    {
        
    }
    public void Clear()
    {
        //save here
    }

    #endregion

    #region Static functions

     public void OnSceneLoaded()
    {
        
    }

    public void RecoverDialogue()
    {
        Debug.Assert(DialogueSys.GetComponent<DemonTextDialogueUI>().content);
        var content = DialogueSys.GetComponent<DemonTextDialogueUI>().content;
        
        for (int i = 0; i < FinishedLog.Count; i++)
        {
            switch (Speaker[i])
            {
                case 0:
                    GameObject.Instantiate(Resources.Load<GameObject>(_demonTextBox), content.transform);
                    break;
                case 1:
                    GameObject.Instantiate(Resources.Load<GameObject>(_playerTextBox), content.transform);
                    break;
            }

           
        }
    }

    #endregion
   
    
}
