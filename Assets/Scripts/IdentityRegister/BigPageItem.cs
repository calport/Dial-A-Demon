using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPageItem : MonoBehaviour
{
    public string nameOfPage;
    // Start is called before the first frame update
    void Awake()
    {
        //Debug.Assert(String.Compare(nameOfPage, string.Empty) == 0);
        if (!Services.referenceInfo.BigPage.ContainsValue(gameObject))
        {
            Services.referenceInfo.BigPage.Add(nameOfPage,gameObject);
        }
    }

    private void OnEnable()
    {
        //Debug.Assert(String.Compare(nameOfPage, string.Empty) == 0);
        if (!Services.referenceInfo.BigPage.ContainsValue(gameObject))
        {
            Services.referenceInfo.BigPage.Add(nameOfPage,gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Services.referenceInfo.BigPage.ContainsValue(gameObject))
        {
            Services.referenceInfo.BigPage.Remove(nameOfPage);
        }
    }
}
