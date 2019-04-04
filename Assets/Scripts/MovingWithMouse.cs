using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovingWithMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Touch _touch;
    private bool _pointerIn;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    private Vector2 _prePos;
    void Update()
    {
        if (Input.touchCount != 0 && _pointerIn)
        {
            _touch = Input.GetTouch(0);
            ChangePos(_touch);
            _prePos = _touch.position;
        }

    }

    void ChangePos(Touch touch)
    {
        var posChange = touch.position - _prePos;
        transform.position += new Vector3(posChange.x, posChange.y, 0);                         
    }
    
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //initialize the touch position
        if (Input.touchCount != 0) _prePos = Input.GetTouch(0).position;
        _pointerIn = true;
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        _pointerIn = false;
    }
}   