using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CanvasEffect
{
    public void PageFade(CanvasGroup canvas)
    {
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
        //TODO
        //someswitching effect can be added here
        //canvas.alpha = 1;
        canvas.DOFade(0.5f, 0.2f);
    }

    public void CanvasBack(CanvasGroup canvas)
    {
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
        //TODO
        //someswitching effect can be added here
        //canvas.alpha = 1;
        canvas.DOFade(1.0f, 0.2f);
    }

    public void CanvasOn(CanvasGroup canvas)
    {
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
        //TODO
        //someswitching effect can be added here
        canvas.alpha = 1;
    }

    public void CanvasOff(CanvasGroup canvas)
    {
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
        //TODO
        //someswitching effect can be added here
        canvas.alpha = 0;
    }

    public void CanvasDistroy(GameObject obj)
    {

    }
}
