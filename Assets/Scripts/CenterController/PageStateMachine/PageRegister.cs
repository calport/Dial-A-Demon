using System;
using System.Collections;
using System.Collections.Generic;
using DialADemon.Page;
using UnityEngine;

public class PageRegister : MonoBehaviour
{
    [SerializeField] private string _note = "Id starts at 1";
    [SerializeField] private int _id;
    [SerializeField] private string _name = nameof(gameObject);
    [SerializeField] private GameObject[] _relatedObj;
    [SerializeField] private BelongedSystem _system;
    [SerializeField] private RenderLayer _layer;
    [SerializeField] private LoadBehavior _load;

    void Awake()
    {
        _relatedObj = new[] {gameObject};
        Enum[] prop = new Enum[3];
        prop[0] = _system;
        prop[1] = _layer;
        prop[3] = _load;
        Services.pageState.AddState(_id, _name, _relatedObj,prop);

        if (gameObject.GetComponent<CanvasGroup>())
        {
            var cp = gameObject.GetComponent<CanvasGroup>();
            Services.canvasEffect.CanvasOff(cp);
        }
    }
}
