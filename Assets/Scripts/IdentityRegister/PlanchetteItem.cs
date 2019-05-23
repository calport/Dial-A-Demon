using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanchetteItem : MonoBehaviour
{
    
    public Vector3 OriginPos;
    void Awake()
    {
        //Debug.Assert(String.Compare(nameOfPage, string.Empty) == 0);
        if (!Services.referenceInfo.planchette)
        {
            OriginPos = transform.position;
            Services.referenceInfo.planchette = gameObject;
        }
    }

    private void OnEnable()
    {
        //Debug.Assert(String.Compare(nameOfPage, string.Empty) == 0);
        if (!Services.referenceInfo.planchette)
        {
            OriginPos = transform.position;
            Services.referenceInfo.planchette = gameObject;
        }
    }

    private void OnDestroy()
    {
        if (!Services.referenceInfo.planchette)
        {
            Services.referenceInfo.planchette = null;
        }
    }
}
