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
        GameObject parent;
        Services.referenceInfo.BigPage.TryGetValue("MainMenu", out parent);
        parent = parent.transform.Find("PageCanvas").gameObject;
        createdFile = Instantiate(Resources.Load<GameObject>(OpenFilePrefabAddrass), parent.transform);
        var closeButton = createdFile.transform.GetComponentInChildren<CloseFileButton>();
        Debug.Assert(closeButton==null);
        closeButton.File = createdFile;
        if (Services.gameStates.GetCurrentState().GetType() == typeof(GameStates.TextingPage))
        {
            Services.textStates.ChangeGameState<TextStates.OnFileCheck>(new TextStates.OnFileCheck());
        }        
 
    }
}
