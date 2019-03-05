using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyboardVibration : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        
        if (transform.parent.gameObject.GetComponent<KeyboardInput>().content != String.Empty)
        {
            //vibration
            Handheld.Vibrate();
        }
    }
}
