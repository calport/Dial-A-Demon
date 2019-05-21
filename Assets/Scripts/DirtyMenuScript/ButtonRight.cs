using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonRight : MonoBehaviour
{
    public GameObject[] choiceList;
    public ButtonChoicePlace buttonChoice;
    
    public void OnClickSwitchRight()
    {
        Debug.Log("click");
        choiceList[buttonChoice.NowChoice].SetActive(false);
        buttonChoice.NowChoice++;
        if (buttonChoice.NowChoice < choiceList.Length)
        {
        }
        else
        {
            buttonChoice.NowChoice = 0;
        }
        choiceList[buttonChoice.NowChoice].SetActive(true);
    }

    public void OnClickSwitchLeft()
    {
        Debug.Log("click");
        choiceList[buttonChoice.NowChoice].SetActive(false);
        buttonChoice.NowChoice--;
        if (buttonChoice.NowChoice < 0)
        {
            buttonChoice.NowChoice = choiceList.Length - 1;
        }
        choiceList[buttonChoice.NowChoice].SetActive(true);
    }
}
