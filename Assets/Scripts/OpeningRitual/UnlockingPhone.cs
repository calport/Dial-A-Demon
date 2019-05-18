using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class UnlockingPhone : MonoBehaviour
{
    public GameObject Unlock1, Unlock2, Unlock3;
    public GameObject hint, instructions;
    public GameObject MicrophoneSummon; 
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
    List<int> inputNumber = new List<int>();
    Vector3 correctCode = new Vector3((int)6, (int)6, (int)6);
    Vector3 jumpCode = new Vector3((int)1, (int)2, (int)3);
    
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

    public void UnlockPhoneClick(int number)
    {
        switch (guess)
        {
            case 0:
                Unlock1.SetActive(true);
                break;
            case 1:
                Unlock2.SetActive(true);
                break; 
            case 2:
                Unlock3.SetActive(true);
                break;
        }
        guess++;
        
        if(inputNumber.Count<3) inputNumber.Add(number);
        if (inputNumber.Count == 3)
        {
            var currentCode = new Vector3((int)inputNumber[0], (int)inputNumber[1], (int)inputNumber[2]);
            if (currentCode == correctCode) StartCoroutine(CorrectFade());
            else if (currentCode == jumpCode)
                Services.gameStates.ChangeGameState<GameStates.MenuPage>(new GameStates.MenuPage());
            else StartCoroutine(ScreenShake());

            guess = 0;
            inputNumber.Clear();
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
        //make the screenshake and a vibration play 
        shakeDuration = 2.0f;
        //Handheld.Vibrate(); 
        //set all unlocks back to false 
        Unlock1.SetActive(false);
        Unlock2.SetActive(false);
        Unlock3.SetActive(false);
        
        //make the hint display
        instructions.SetActive(false);
        hint.SetActive(true);
        
        
        guess = 0;
        correct = 0; 

    }

    IEnumerator CorrectFade()
    {
        yield return new WaitForSeconds(1);
        Microphone.Start(null,true,10,44100);
        gameObject.SetActive(false);
        MicrophoneSummon.SetActive(true);
        //SceneManager.LoadScene("Texting");
       
    }

    private void OnEnable()
    {
        initialPosition = transform.localPosition;
    }
}
