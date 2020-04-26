using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovingWithMouse : MonoBehaviour, IDragHandler
{
    private Vector3 oriPos;

    private void Start()
    {
        oriPos = gameObject.transform.position;
        Services.eventManager.AddHandler<Reset>(_OnReset);
    }

    private void OnDestroy()
    {
        Services.eventManager.RemoveHandler<Reset>(_OnReset);
    }
    
    public void OnDrag(PointerEventData eventData)
    { 
        if (Input.touchSupported)
        {
            if(Input.touchCount == 0 || Input.GetTouch(0).phase == TouchPhase.Began) return;
            var touch = Input.GetTouch(0);
            Vector3 posChange = touch.position;
            var plantPos = new Vector3(posChange.x, posChange.y, -1);
        }
        else
        {
            if(!Input.GetMouseButton(0) || Input.GetMouseButtonDown(0)) return;
            Vector2 mouseDrag = Camera.main.ScreenToWorldPoint(eventData.position);
            transform.position = new Vector3(mouseDrag.x, mouseDrag.y, -1);
        }
    }

    private void _OnReset(Reset e)
    {
        gameObject.transform.position = oriPos;
    }
}   