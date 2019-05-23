using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageTransButtonItem1 : MonoBehaviour
{
    //this can be deleted after
    [SerializeField] private bool transferToPreviousPage = false;
    [SerializeField] public Page TransToPage;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (transferToPreviousPage)
        {
            if (!Services.referenceInfo.ToSpecifiedPage[Services.referenceInfo.ToSpecifiedPage.Length-1].Contains(gameObject.GetComponent<Button>()))
            {
                Services.referenceInfo.ToSpecifiedPage[Services.referenceInfo.ToSpecifiedPage.Length - 1]
                    .Add(gameObject.GetComponent<Button>());
            }
        }
        else
        {
            if (!Services.referenceInfo.ToSpecifiedPage[TransToPage.GetHashCode()]
                .Contains(gameObject.GetComponent<Button>()))
            {
                Services.referenceInfo.ToSpecifiedPage[TransToPage.GetHashCode()]
                    .Add(gameObject.GetComponent<Button>());
            }
        }
    }

    private void OnEnable()
    {
        if (transferToPreviousPage)
        {
            if (!Services.referenceInfo.ToSpecifiedPage[Services.referenceInfo.ToSpecifiedPage.Length-1].Contains(gameObject.GetComponent<Button>()))
            {
                Services.referenceInfo.ToSpecifiedPage[Services.referenceInfo.ToSpecifiedPage.Length - 1]
                    .Add(gameObject.GetComponent<Button>());
            }
        }
        else
        {
            if (!Services.referenceInfo.ToSpecifiedPage[TransToPage.GetHashCode()]
                .Contains(gameObject.GetComponent<Button>()))
            {
                Services.referenceInfo.ToSpecifiedPage[TransToPage.GetHashCode()]
                    .Add(gameObject.GetComponent<Button>());
            }
        }
    }

    private void OnDestroy()
    {
        if (transferToPreviousPage)
        {
            if (Services.referenceInfo.ToSpecifiedPage[Services.referenceInfo.ToSpecifiedPage.Length-1].Contains(gameObject.GetComponent<Button>()))
            {
                Services.referenceInfo.ToSpecifiedPage[Services.referenceInfo.ToSpecifiedPage.Length - 1]
                    .Remove(gameObject.GetComponent<Button>());
            }
        }
        else
        {
            if (Services.referenceInfo.ToSpecifiedPage[TransToPage.GetHashCode()]
                .Contains(gameObject.GetComponent<Button>()))
            {
                Services.referenceInfo.ToSpecifiedPage[TransToPage.GetHashCode()]
                    .Remove(gameObject.GetComponent<Button>());
            }
        }
    }
}
