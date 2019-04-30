using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnlockingPhone : MonoBehaviour
{
    public GameObject Unlock1, Unlock2, Unlock3;
    private int guess;
    private int correct;
    
    //screenshake for if incorrect guess
    private Transform transform;
    //duration of screenshake
    private float shakeDuration = 0f; 
    //magnitude for the shake
    private float shakeMagnitude = 2.7f; 
    //how fast it should go away
    private float dampingSpeed = 0.75f; 
    
    //initial position of GameObject
    private Vector3 initialPosition;


    private void Awake()
    {
        if (transform == null)
        {
            transform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        guess = 0;
        correct = 0;
    }

    // Update is called once per frame
    void Update()
    {
            if (shakeDuration > 0)
            {
                transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
   
                shakeDuration -= Time.deltaTime * dampingSpeed;
            }
            else
            {
                shakeDuration = 0f;
                transform.localPosition = initialPosition;
            }
    }

    public void UnlockPhoneCorrect()
    {
        switch (guess)
        {
            case 0:
                Unlock1.SetActive(true);
                guess++;
                correct++;
                return;
            case 1:
                Unlock2.SetActive(true);
                guess++;
                correct++;
                return; 
            case 2:
                Unlock3.SetActive(true);
                guess++;
                correct++;
                if (correct == 3)
                {
                    StartCoroutine(CorrectFade());
                }
                else
                {
                    StartCoroutine(ScreenShake());
                }
                return;
        } 
    }

    public void UnlockPhoneIncorrect()
    {
        Debug.Log("wrong guess");
        switch (guess)
        {
           case 0:
               Unlock1.SetActive(true);
               guess++;
               return;
           case 1:
               Unlock2.SetActive(true);
               guess++;
               return; 
           case 2:
               Unlock3.SetActive(true);
               guess++;
               //coroutine a screenshake and reset the guess numbers and unlocks to empty
               StartCoroutine(ScreenShake());
               return;
        } 
       
    }

    IEnumerator ScreenShake()
    {
        yield return new WaitForSeconds(1);
        shakeDuration = 2.0f; 
        Unlock1.SetActive(false);
        Unlock2.SetActive(false);
        Unlock3.SetActive(false);
        guess = 0;
        correct = 0; 

    }

    IEnumerator CorrectFade()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        initialPosition = transform.localPosition;
    }
}
