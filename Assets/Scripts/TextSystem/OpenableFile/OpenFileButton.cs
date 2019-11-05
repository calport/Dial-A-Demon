using System.Collections;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using UnityEngine;

public class OpenFileButton : MonoBehaviour
{
    [SerializeField] private string OpenFilePrefabAddrass;
    private GameObject createdFile;

    public void OnSimpleTap()
    {
        //create a new prefab on front layer
        GameObject parent = new GameObject();
        //Services.referenceInfo.MenuPage.TryGetValue("FrontLayer", out parent);
        //TODO get parent
        createdFile = Instantiate(Resources.Load<GameObject>(OpenFilePrefabAddrass), parent.transform);
        //set this gameobject a reference to close button
        var closeButton = createdFile.transform.GetComponentInChildren<CloseFileButton>();
        closeButton.File = createdFile;
        
        //change text state
        /*if (Services.gameStates.GetCurrentState().GetType() == typeof(GameStates.TextingPage))
        { 
            Services.textStates.ChangeGameState<TextStates.OnFileCheck>(new TextStates.OnFileCheck());
        }    */    
       
    }
}
