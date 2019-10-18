using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentSizeChange : MonoBehaviour
{
    public float top;
    public float bottom;
    public float space;
    public float minHeight = 700f;

    public void Update()
    {
        ChangeScrollContentSize();
    }

    public void ChangeScrollContentSize()
    {
        float expHeight = 0f;
        int childNum=0;
        foreach (Transform trans in gameObject.transform)
        {
            if (trans.gameObject != gameObject)
            {
                expHeight += trans.gameObject.GetComponent<RectTransform>().sizeDelta.y;
                childNum++;
            }
        }

        expHeight += (childNum - 1) * space + top + bottom;
        
        var height = Mathf.Max(minHeight,expHeight);
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x,height );
    }
}
