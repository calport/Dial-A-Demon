using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        transform.position = plantPos* 0.001f ;
    }
    
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        _pointerIn = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        _pointerIn = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("true");
            //var curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            //transform.position = curScreenSpace;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(Input.touchCount>0) ChangePos(Input.GetTouch(0));
        
        Vector2 mouseDrag = eventData.position;
        transform.position = mouseDrag;
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("true");
            //var curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            //transform.position = curScreenSpace;
        }
    }
}   