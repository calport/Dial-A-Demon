using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraRenderingItem : MonoBehaviour
{
    public Page pageBelongedTo;

    private void Awake()
    {
        if (!Services.referenceInfo.CameraRenderingItem[Convert.ToInt32(pageBelongedTo)].Contains(gameObject))
        {
            Services.referenceInfo.CameraRenderingItem[Convert.ToInt32(pageBelongedTo)].Add(gameObject);
        } 
    }

    private void OnEnable()
    {
        if (!Services.referenceInfo.CameraRenderingItem[Convert.ToInt32(pageBelongedTo)].Contains(gameObject))
        {
            Services.referenceInfo.CameraRenderingItem[Convert.ToInt32(pageBelongedTo)].Add(gameObject);
        }
    }

}
