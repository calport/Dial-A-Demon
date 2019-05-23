using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MicrophoneRepeat : MonoBehaviour
{
    //Times through Coroutine
    private int RepeatedTime;

    public GameObject[] words;
    public GameObject demonResponds;
    public GameObject wordScene;
    public GameObject soundRecieving;
    public GameObject magicCircle;
    private AudioVisualizer audioVisualizer;
    public float threshold;
    public float DurTime = 0.0f;
    public GameObject MicrophoneVisual;
    
    private float startGetSoundTime = 0.0f;
    private float startCoroutineTime = 0.0f;
    public float[] microphoneGetSoundTime = new float[7];
    
    // Start is called before the first frame update
    void Start()
    {
        RepeatedTime = 0; 
        StartCoroutine(WaitForWords());
        audioVisualizer = gameObject.GetComponent<AudioVisualizer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitForWords()
    {   
        while(!gameObject.GetComponent<AudioVisualizer>().adjustmentFinished){yield return new WaitForSeconds(Time.deltaTime);}
        soundRecieving.SetActive(false);
        wordScene.SetActive(true);
        //yield return new WaitForSeconds(3);
        yield return new WaitForSeconds(1);
            
        /*switch (RepeatedTime)
        {
            case 0:
                words[RepeatedTime].SetActive(true);
                RepeatedTime++;
                StartCoroutine(WaitForWords());
                break; 
            case 1:
                words[RepeatedTime].SetActive(true);
                RepeatedTime++;
                StartCoroutine(WaitForWords());
                break;
            case 2:
                words[RepeatedTime].SetActive(true);
                RepeatedTime++;
                StartCoroutine(WaitForWords());
                break;
            case 3:
                words[RepeatedTime].SetActive(true);
                RepeatedTime++;
                StartCoroutine(WaitForWords());
                break; 
            case 4: 
                words[RepeatedTime].SetActive(true);
                RepeatedTime++;
                StartCoroutine(WaitForWords());
                break;
            case 5: 
                words[RepeatedTime].SetActive(true);
                RepeatedTime++;
                StartCoroutine(WaitForWords());
                break; 
            case 6: 
                words[RepeatedTime].SetActive(true);
                RepeatedTime++;
                //last word set up the demon respond
                StartCoroutine(DemonResponds());
                break; 
        }*/
        StartCoroutine(WordsUp(0));

        //yield break;
    }

    IEnumerator DemonResponds()
    {    
        wordScene.SetActive(false);
        words[5].SetActive(false);
        MicrophoneVisual.SetActive(false);
        Handheld.Vibrate();
        yield return new WaitForSeconds(1); 
        Handheld.Vibrate(); 
        yield return new WaitForSeconds(2);
        Handheld.Vibrate();
        
        wordScene.SetActive(false);
        for (int i = 0; i < 6; i++)
        {
            words[i].SetActive(false);
        }
        
        demonResponds.SetActive(true);
        yield return new WaitForSeconds(3);
        magicCircle.SetActive(true);
        demonResponds.SetActive(false);
        yield return new WaitForSeconds(1);
        
    }

    IEnumerator WordsUp(int order)
    {
        if (order == 6)
        {
            StartCoroutine(DemonResponds());
        }
        else
        {
            if (order >0)
            {
                words[order-1].SetActive(false);
            }
            
            yield return new WaitForSeconds(0.5f);
            words[order].SetActive(true);
    
            startCoroutineTime = Time.time;
            Coroutine nextCoroutine = null;
            bool finishCheck = false;
            
            //start check
            while (!finishCheck)
            {
                yield return new WaitForSeconds(Time.deltaTime);
    
                var coroutineLastTime = Time.time - startCoroutineTime;
    
                if (coroutineLastTime > 3.0f)
                {
                    
                    finishCheck = true;
                   
                    startGetSoundTime = 0.0f;
                    order++;
                    nextCoroutine = StartCoroutine(WordsUp(order));
                }
    
                if (audioVisualizer.largestSound > threshold)
                {
                    Debug.Log("RecordTrue");
                    if (startGetSoundTime == 0.0f)
                    {
                        startGetSoundTime = Time.time;
                    }
    
    
    
                    var lastTime = Time.time - startGetSoundTime;
                    DurTime = lastTime;
                    if (lastTime > microphoneGetSoundTime[order])
                    {
                        finishCheck = true;
                        /*f (nextCoroutine == null)
                        {
                            StopCoroutine(nextCoroutine);
                        }*/
    
                        startGetSoundTime = 0.0f;
                        order++;
                        nextCoroutine = StartCoroutine(WordsUp(order));
                    }
                }
                else
                {
                    startGetSoundTime = 0.0f;
                }
                
    
            }
        }

    }

   
    
}
