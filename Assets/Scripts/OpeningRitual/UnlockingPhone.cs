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
    private Transform _transform;
    //duration of screenshake
    private float _shakeDuration = 0f; 
    //magnitude for the shake
    private float shakeMagnitude = 2.7f; 
    //how fast it should go away
    private float dampingSpeed = 0.75f; 
    private List<int> _inputNumber = new List<int>();
    private Vector3 _correctCode = new Vector3((int)6, (int)6, (int)6);
    private Vector3 _jumpCode = new Vector3((int)1, (int)2, (int)3);
    
    //initial position of GameObject
    private Vector3 initialPosition;


    private void Awake()
    {
        if (_transform == null)
        {
            _transform = GetComponent(typeof(Transform)) as Transform;
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
            if (_shakeDuration > 0)
            {
                _transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
   
                _shakeDuration -= Time.deltaTime * dampingSpeed;
            }
            else
            {
                _shakeDuration = 0f;
                _transform.localPosition = initialPosition;
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
        
        if(_inputNumber.Count<3) _inputNumber.Add(number);
        if (_inputNumber.Count == 3)
        {
            var currentCode = new Vector3((int)_inputNumber[0], (int)_inputNumber[1], (int)_inputNumber[2]);
            if (currentCode == _correctCode) StartCoroutine(CorrectFade());
            else if (currentCode == _jumpCode)
                Services.pageState.ChangeGameState("Menu_Main");
            else StartCoroutine(ScreenShake());

            guess = 0;
            _inputNumber.Clear();
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
        _shakeDuration = 2.0f;
    #if UNITY_IOS
        Handheld.Vibrate(); 
    #endif
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
        initialPosition = _transform.localPosition;
    }
}
