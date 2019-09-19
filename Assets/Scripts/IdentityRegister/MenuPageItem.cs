using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPageItem : MonoBehaviour
{
    [SerializeField]private string _nameOfPage = String.Empty;
    public List<GameObject> attachedSpriteItem;
    // Start is called before the first frame update
    void Awake()
    {
        //
        attachedSpriteItem = new List<GameObject>();
        var list = gameObject.GetComponentsInChildren<CameraRenderingItem>();
        foreach (var cri in list)
        {
            attachedSpriteItem.Add(cri.gameObject);
        }
        
        //
        if (!Services.referenceInfo.MenuPage.ContainsValue(gameObject))
        {
            Services.referenceInfo.MenuPage.Add(_nameOfPage,gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Services.referenceInfo.MenuPage.ContainsValue(gameObject))
        {
            Services.referenceInfo.MenuPage.Remove(_nameOfPage);
        }
    }
}
