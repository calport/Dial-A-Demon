using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPageItem : MonoBehaviour
{
    public string nameOfPage = String.Empty;
    // Start is called before the first frame update
    void Awake()
    {
        //Debug.Assert(String.Compare(nameOfPage, string.Empty) == 0);
        if (!Services.referenceInfo.MenuPage.ContainsValue(gameObject))
        {
            Services.referenceInfo.MenuPage.Add(nameOfPage,gameObject);
        }
    }

    private void OnEnable()
    {
        //Debug.Assert(String.Compare(nameOfPage, string.Empty) == 0);
        if (!Services.referenceInfo.MenuPage.ContainsValue(gameObject))
        {
            Services.referenceInfo.MenuPage.Add(nameOfPage,gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Services.referenceInfo.MenuPage.ContainsValue(gameObject))
        {
            Services.referenceInfo.MenuPage.Remove(nameOfPage);
        }
    }
}
