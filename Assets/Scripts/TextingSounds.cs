using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextingSounds : MonoBehaviour
{

    public AudioSource keyboardSound; 
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("this script is attached");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision detected");
        keyboardSound.Play();
    }
}
