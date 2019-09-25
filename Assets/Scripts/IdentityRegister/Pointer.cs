using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    private void Awake()
    {
        Services.textManager.textRunner.pointer = gameObject;
    }
}
