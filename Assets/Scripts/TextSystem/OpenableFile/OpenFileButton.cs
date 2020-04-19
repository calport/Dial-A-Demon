using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DialADemon.Page;
using HedgehogTeam.EasyTouch;
using UnityEngine;

public class OpenFileButton : MonoBehaviour
{
    public string fileContentName;
    public GameObject document { get; private set; }
    private PageState ps
    {
        get
        {
            return Services.pageState;
        }
    }

    public void OnSimpleTap()
    {
        ps.ChangeGameState("Front_Main");
        foreach (var item in Services.pageState.GetGameState("Front_Main").relatedObj)
        {
            if (item.CompareTag("PageObj"))
            {
                document = Instantiate(Resources.Load<GameObject>("Prefabs/FileBubble/" + fileContentName),item.transform);
                document.GetComponentInChildren<CloseFileButton>().ofb = this;
                document.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
                document.transform.DOScale(1f, 1f);
                break;
            }
        }
    }
}
