﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovingWithMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    private Touch _touch;
    private bool _pointerIn;
    
    void ChangePos(Touch touch)
    {
        
        Vector3 posChange = touch.position;
        var posLength = posChange.magnitude;
        var plantPos = new Vector3(posChange.x, posChange.y, 0)    ;
        var plantLength = plantPos.magnitude;
        transform.position = plantPos * 0.7f;
    }
    
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        _pointerIn = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        _pointerIn = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        ChangePos(Input.GetTouch(0));
    }
}   