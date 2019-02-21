using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendPlayerWords : MonoBehaviour
{
    private void Start()
    {
        TextInputManager.Instance.sendButton = this.gameObject.GetComponent<Button>();
        GetComponent<Button>().interactable = false;
    }

    public void SendWords()
    {
        TextInputManager.Instance.SendWords();
    }
}
