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
    
    // Start is called before the first frame update
    void Start()
    {
        RepeatedTime = 0; 
        StartCoroutine(WaitForWords()); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitForWords()
    {
        yield return new WaitForSeconds(3);
            
        switch (RepeatedTime)
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
        }

        //yield break;
    }

    IEnumerator DemonResponds()
    {
        yield return new WaitForSeconds(1); 
        Handheld.Vibrate(); 
        yield return new WaitForSeconds(2);
        Handheld.Vibrate();
        demonResponds.SetActive(true);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Texting");
        //demonResponds.SetActive(false);
        //wordScene.SetActive(false);
    }
    
}
