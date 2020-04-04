using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using UnityEngine;
//using UnityEngine.Experimental.UIElements;
using Button = UnityEngine.UI.Button;
//using Color = System.Drawing.Color;
using Image = UnityEngine.UI.Image;

public class MenuPlantChange : MonoSingleton<MenuPlantChange>
{
    //text, sound, general, mode
    public CanvasGroup[] plant = new CanvasGroup[4];
    public UnityEngine.UI.Image[] plantIcon = new UnityEngine.UI.Image[4];
    public string[] iconPath = new string[4];
    public Sprite[] iconSprite = new Sprite[4];
    private int nowPlant = 0;

    private void Start()
    {
    }

    public void StartChange(int i )
    {
        StartCoroutine(OnChange(i));
    }
    private IEnumerator OnChange(int i)
    {

        foreach (var button in plantIcon)
        {
            if (button.gameObject.GetComponent<Button>())
            {
                button.gameObject.GetComponent<Button>().enabled = false;
            }
        }
        yield return StartCoroutine(ShaderDisappear());
        plant[nowPlant].alpha = 0;
        plant[nowPlant].interactable = false;
        plant[nowPlant].blocksRaycasts = false;
        Debug.Log("chenggong");
        
        plant[i].alpha = 0;
        plant[i].interactable = true;
        plant[i].blocksRaycasts = true;
        plantIcon[0].sprite = iconSprite[i];
        /*List<string> plantIconSmallPath = iconPath.ToList();
        plantIconSmallPath.Remove(iconPath[i]);
        plantIconSmallPath.ToArray();
        for(int x =0; x<plantIconSmallPath.Count; x++)
        {
            plantIcon[x + 1].sprite = Resources.Load(plantIconSmallPath[x]) as Sprite;
            plantIcon[x+i].GetComponent<MenuChangeCall>().i
        }*/
        for(int x= 0;x<plantIcon.Length;x++)
        {
            if (x < i)
            {
                plantIcon[x + 1].sprite = iconSprite[x];
                var color = plantIcon[x + 1].color;
                color.a = 0;
                plantIcon[x + 1].color = color;
                plantIcon[x + 1].GetComponent<MenuChangeCall>().i = x;
                
            }
            if(x>i)
            {
                plantIcon[x].sprite = iconSprite[x];
                var color = plantIcon[x].color;
                color.a = 0;
                plantIcon[x].color = color;
                plantIcon[x].GetComponent<MenuChangeCall>().i = x;
            }
        }

        yield return StartCoroutine(ShaderAppear(i));
        
        plant[i].alpha = 1;
        foreach (var icon in plantIcon)
        {
            var color = icon.color;
            color.a = 1;
            icon.color = color;
        }
        nowPlant = i;
        
        foreach (var button in plantIcon)
        {
            if (button.gameObject.GetComponent<Button>())
            {
                button.gameObject.GetComponent<Button>().enabled = true;
            }
        }
    }

    IEnumerator ShaderDisappear()
    {
        var startTime = Time.time;
        while((Time.time- startTime)<1.5f)
        {
            plant[nowPlant].GetComponent<CanvasGroup>().alpha =
                Mathf.Lerp(plant[nowPlant].GetComponent<CanvasGroup>().alpha, 0, 2.0f*Time.deltaTime);
            foreach (var image in plantIcon)
            {
                var color = image.color;
                color.a = Mathf.Lerp(color.a, 0,  2.0f*Time.deltaTime);
                image.color = color;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    IEnumerator ShaderAppear(int i)
    {
        var startTime = Time.time;
        while((Time.time- startTime)<1.5f)
        {
            plant[i].GetComponent<CanvasGroup>().alpha =
                Mathf.Lerp(plant[i].GetComponent<CanvasGroup>().alpha, 1, 2.0f*Time.deltaTime);
            foreach (var image in plantIcon)
            {
                var color = image.color;
                color.a = Mathf.Lerp(color.a, 1, 2.0f*Time.deltaTime);
                image.color = color;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
