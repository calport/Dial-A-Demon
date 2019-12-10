using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(PageChangeButton))]
public class ButtonArtEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<PageChangeButton>().artEffect += Twist;
    }

    private void Twist()
    {
        var pcb = gameObject.GetComponent<PageChangeButton>();
        transform.DORotate(new Vector3(0f, 0f, 180f), 1f).onComplete = delegate
        {
            pcb.OnClick();
            transform.DORotate(new Vector3(0f, 0f, 0f), 0.1f);
        };
    }
}
