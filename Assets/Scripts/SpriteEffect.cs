using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class SpriteEffect : MonoBehaviour
{
    //to use this script, first attach this script to the item that you want it to do the effect,
    //and then assign the numbers to it so it will work just like you want.
    private bool effectStart = true;
    private float degree;
    private float speed;
    private EffectType effect;
    public enum EffectType
    {
        NoEffect,
        FadeImage,
        ShowImage,
        ShowSomeImage,
    }

    public void numberAssignment(float inputSpeed=3.0f, EffectType inputEffect=EffectType.NoEffect, float inputDegree = 0.5f)
    {
        degree = inputDegree;
        speed = inputSpeed;
        effect = inputEffect;
    }

    private void Awake()
    {
        SpriteEffect[] effectNum = GetComponents<SpriteEffect>();
        if (effectNum.Length != 1)
        {
            for (int i = 0; i < effectNum.Length - 1; i++)
            {
                Destroy(effectNum[i]);
            }
        }
    }

    private void Update()
    {
        if (effectStart)
        {
            if (effect == EffectType.FadeImage)
            {
                FadeImage(speed);
            }

            if (effect == EffectType.ShowImage)
            {
                ShowImage(speed);
            }
            
            if (effect == EffectType.ShowSomeImage)
            {
                ShowSomeImage(speed, degree);
            }
        }
    }

    void FadeImage(float speed)
    {
        Color newColor = this.GetComponent<Image>().color;
        newColor.a = Mathf.Lerp(newColor.a, 0f, speed * Time.deltaTime);
        this.GetComponent<Image>().color = newColor;
        if (newColor.a < 0.01f)
        {
            newColor.a = 0f;
            this.GetComponent<Image>().color = newColor;
            effectStart = !effectStart;
            Destroy(this);
        }
    }
    
    void ShowImage(float speed)
    {
        Color newColor = this.GetComponent<Image>().color;
        newColor.a = Mathf.Lerp(newColor.a, 1f, speed * Time.deltaTime);
        this.GetComponent<Image>().color = newColor;
        if (newColor.a >0.99f)
        {
            newColor.a = 1f;
            this.GetComponent<Image>().color = newColor;
            effectStart = !effectStart;
            Destroy(this);
        }
    }
    
    void ShowSomeImage(float speed, float degree)
    {
        Color newColor = this.GetComponent<Image>().color;
        newColor.a = Mathf.Lerp(newColor.a, degree, speed * Time.deltaTime);
        this.GetComponent<Image>().color = newColor;
        if (Mathf.Abs(degree-newColor.a)<0.01f)
        {
            newColor.a = degree;
            this.GetComponent<Image>().color = newColor;
            effectStart = !effectStart;
            Destroy(this);
        }
    }
}
