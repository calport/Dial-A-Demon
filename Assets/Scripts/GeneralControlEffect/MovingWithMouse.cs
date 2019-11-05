using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovingWithMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    private Touch _touch;
    private bool _pointerIn;
    
    void ChangePosWithTouch(Touch touch)
    {
        Vector3 posChange = touch.position;
        Debug.Log(posChange);
        var posLength = posChange.magnitude;
        var plantPos = new Vector3(posChange.x, posChange.y, -1)    ;
        var plantLength = plantPos.magnitude;
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
        if(Input.touchCount>0) ChangePosWithTouch(Input.GetTouch(0));
        
        Vector2 mouseDrag = Camera.main.ScreenToWorldPoint(eventData.position);
        transform.position = new Vector3(mouseDrag.x, mouseDrag.y,-1);
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("true");
            //var curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            //transform.position = curScreenSpace;
        }
    }
}   