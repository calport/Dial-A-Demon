using System.Collections;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using UnityEngine;

public class CloseFileButton : MonoBehaviour
{
    public GameObject File;

    public void OnSimpleTap()
    {
            //Services.textStates.ChangeGameState<TextStates.NormalText>(new TextStates.NormalText());
            Destroy(File);
    }
}
