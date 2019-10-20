using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTrigger : MonoBehaviour
{
    private void Awake()
    {
        Services.textManager.pointerTrigger = gameObject;
    }
}
