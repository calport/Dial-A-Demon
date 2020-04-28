using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using HedgehogTeam.EasyTouch;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MagicCircle : MonoBehaviour
{
    public GameObject drawCircle;
    public GameObject demonCircle;
    private float fadeTime;
    private float waitTime = 2.0f;
    private bool startFade = false;

    private float changeOpacity;
    //private Color colorOfCircle;

    private SpriteRenderer rend;
    private bool noLongerRubbing; 
    
    public GameObject openingRitual;

    // Start is called before the first frame update
    void Start()
    {
        rend = drawCircle.GetComponent<SpriteRenderer>();
        Color colorOfCircle = rend.material.color;
        colorOfCircle.a = 0f;
        rend.material.color = colorOfCircle; 
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(noLongerRubbing);
        if (noLongerRubbing)
        {
            //if (rend.material.color.a < 0.5f)
            if (rend.material.color.a < 0.4f)
            {
                gameObject.GetComponent<DrawCircle>().speed =
                    Mathf.Lerp(gameObject.GetComponent<DrawCircle>().speed, 3.0f, 2.0f * Time.deltaTime);

                Color c = rend.material.color;
                c.a = Mathf.Lerp(c.a, 0, 2.0f * Time.deltaTime);
                rend.material.color = c;
            }
            else
            {
                gameObject.GetComponent<DrawCircle>().speed =
                    Mathf.Lerp(gameObject.GetComponent<DrawCircle>().speed, 500f, 0.5f*Time.deltaTime);
        
                if(gameObject.GetComponent<DrawCircle>().speed>480f)
                {
                    Color c = rend.material.color;
                    c.a = Mathf.Lerp(c.a,1,0.1f*Time.deltaTime);
                    rend.material.color = c;
                }
            }

        }
        
        //if (rend.material.color.a > 0.8f)
        if (rend.material.color.a > 0.4f)
        {
            Color c = demonCircle.GetComponent<SpriteRenderer>().color;
            c.a = Mathf.Lerp(c.a,1,0.1f*Time.deltaTime);
            demonCircle.GetComponent<SpriteRenderer>().color = c;
        }

        //if (demonCircle.GetComponent<SpriteRenderer>().color.a > 0.4f && !startFade)
        if (demonCircle.GetComponent<SpriteRenderer>().color.a > 0.4f && !startFade)
        {
            startFade = true;
            fadeTime = Time.time;
        }

        if (startFade)
        {
            
            //var openRitualPage = Services.pageState.GetGameState("Ritual_Open");
            //openingRitual = openRitualPage.relatedObj[0];
            //var pic = openingRitual.transform.Find("Background").Find("Pic").gameObject;
            Color c = openingRitual.GetComponent<SpriteRenderer>().color;
            c.a = Mathf.Lerp(c.a,0,0.8f*Time.deltaTime);
            //remember to fade all the pics 
            openingRitual.GetComponent<SpriteRenderer>().color = c;
            rend.material.color = c;
            demonCircle.GetComponent<SpriteRenderer>().color = c;
            gameObject.GetComponent<SpriteRenderer>().color = c;
            if (c.a <0.02f)
            {
                Services.pageState.ChangeGameState("Menu_Main");
            }
        }
       
        
        noLongerRubbing = true;
    }

    public void DrawingMagicCircle()
    {
        gameObject.GetComponent<DrawCircle>().speed =
            Mathf.Lerp(gameObject.GetComponent<DrawCircle>().speed, 500f, 0.5f*Time.deltaTime);
        
        if(gameObject.GetComponent<DrawCircle>().speed>400f)
        {
            Color c = rend.material.color;
            c.a = Mathf.Lerp(c.a,1,0.1f*Time.deltaTime);
            rend.material.color = c;
        }


        noLongerRubbing = false;
    }
    

   

    
  
        
}
