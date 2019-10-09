using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DialADemon.Page;
using UnityEngine;

public class PageRegister : MonoBehaviour
{
    [SerializeField] private string _name = nameof(gameObject);
    [SerializeField] private GameObject[] _relatedObj;
    [SerializeField] private BelongedSystem _system;
    [SerializeField] private RenderLayer _layer;
    [SerializeField] private LoadBehavior _load;

    private List<GameObject> cameraRenderItem = new List<GameObject>();
    void Awake()
    {
        //add camera render item to the list and tag them
        var cmi = _relatedObj[0].gameObject.GetComponentsInChildren<CameraRenderingItem>();
        var objList = _relatedObj.ToList();
        foreach (var item in cmi)
        {
            item.gameObject.tag = "CameraRenderItem";
            objList.Add(item.gameObject);
            item.gameObject.SetActive(false);
        }
        _relatedObj = objList.ToArray();
        
        //
        Enum[] prop = new Enum[3];
        prop[0] = _system;
        prop[1] = _layer;
        prop[2] = _load; Services.pageState.AddState( _name, _relatedObj,prop);

        if (gameObject.GetComponent<CanvasGroup>())
        {
            var cp = gameObject.GetComponent<CanvasGroup>();
            Services.canvasEffect.CanvasOff(cp);
        }
    }
}
