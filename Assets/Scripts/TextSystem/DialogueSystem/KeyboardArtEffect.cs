using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardArtEffect : MonoBehaviour
{
    private GameObject _pointer;
    private Image _img;
    
    private void Start()
    {
        _pointer = Services.textManager.pointerTrigger;
        _img = transform.parent.GetChild(1).gameObject.GetComponent<Image>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _pointer)
        {
            var color = _img.color;
            color.a = 1;
            _img.color = color;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == _pointer)
        {var color = _img.color;
            color.a = 0;
            _img.color = color;
        }
    }
}
