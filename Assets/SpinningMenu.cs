using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpinningMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    private Touch _touch;
    private bool _pointerIn;
    
    public Vector3 delta  = Vector3.zero;
    private Vector3 lastPos = Vector3.zero;
    
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
       // transform.position = new Vector3(mouseDrag.x, mouseDrag.y,-1);
        //transform.Rotate(0f, 0f, mouseDrag.x + mouseDrag.y);
        //print("x" + mouseDrag.x + "and y" + mouseDrag.y);
        
        if ( Input.GetMouseButtonDown(0) )
        {
            Debug.Log("true");
            lastPos = Input.mousePosition;
        }
        else if ( Input.GetMouseButton(0) )
        {
            delta = (Input.mousePosition - lastPos)/2;

            if (mouseDrag.x >= 0 && mouseDrag.y >= 0)
            {
                transform.Rotate(0f, 0f, (delta.y - delta.x));
            }

            if (mouseDrag.x < 0 && mouseDrag.y >= 0)
            {
                transform.Rotate(0f, 0f, -1 * (delta.x + delta.y));
            }

            if (mouseDrag.x < 0 && mouseDrag.y < 0)
            {
                transform.Rotate(0f, 0f, -1 * (delta.y - delta.x));
            }

            if (mouseDrag.x >= 0 && mouseDrag.y < 0)
            {
                transform.Rotate(0f, 0f, (delta.x + delta.y));
            }
            
         
//            Debug.Log( "delta X : " + delta.x );
//            Debug.Log( "delta Y : " + delta.y );
 
            // End do stuff
 
            lastPos = Input.mousePosition;
        }
 
//        if (Input.GetMouseButtonDown(0))
//        {
//            Debug.Log("true");
//            //var curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
//            //transform.position = curScreenSpace;
//        }
    }
}   

