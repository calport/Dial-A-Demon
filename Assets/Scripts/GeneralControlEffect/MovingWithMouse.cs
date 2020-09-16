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
        Services.eventManager.AddHandler<ResetForPageChange>(_OnReset);
    }

    private void OnDestroy()
    {
        Services.eventManager.RemoveHandler<ResetForPageChange>(_OnReset);
    }
    
    public void OnDrag(PointerEventData eventData)
    { 
        Debug.Log("On drag is running");
        if (Input.touchSupported)
        {
            Debug.Log("touch supported");
            if(Input.touchCount == 0 || Input.GetTouch(0).phase == TouchPhase.Began) return;
            var touch = Input.GetTouch(0);
            Vector3 posChange = Camera.main.ScreenToWorldPoint(touch.position);
            transform.position =  new Vector3(posChange.x,posChange.y,-1);
            ///var plantPos = new Vector3(posChange.x, posChange.y, -1);
            Debug.Log("we make it to here");
        }
        else
        {
            Debug.Log("mouse is being used");
            if(!Input.GetMouseButton(0) || Input.GetMouseButtonDown(0)) return;
            Vector2 mouseDrag = Camera.main.ScreenToWorldPoint(eventData.position);
            transform.position = new Vector3(mouseDrag.x, mouseDrag.y, -1);
            Debug.Log("we will not make it");
        }
    }

    private void _OnReset(ResetForPageChange e)
    {
        gameObject.transform.position = oriPos;
    }
}   