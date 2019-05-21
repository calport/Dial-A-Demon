using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroPhoneInput : MonoBehaviour
{
    //make sure the gameobject has a audio source
    private AudioSource audioSource;
    
    public int audioSampleRate = 44100;
    public string microphone;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource> ();
        foreach (string device in Microphone.devices) {
            if (microphone == null) {
                //set default mic to first mic found.
                microphone = device;
            }
        }
        UpdateMicrophone();
    }

    void UpdateMicrophone(){
        audioSource.Stop(); 
        //Start recording to audioclip from the mic
        audioSource.clip = Microphone.Start(microphone, true, 10, audioSampleRate);
        audioSource.loop = true; 
        // Mute the sound with an Audio Mixer group becuase we don't want the player to hear it
        Debug.Log(Microphone.IsRecording(microphone).ToString());

        if (Microphone.IsRecording (microphone)) { //check that the mic is recording, otherwise you'll get stuck in an infinite loop waiting for it to start
            while (!(Microphone.GetPosition (microphone) > 0)) {
            } // Wait until the recording has started. 
		
            Debug.Log ("recording started with " + microphone);

            // Start playing the audio source
            audioSource.Play (); 
        } else {
            //microphone doesn't work for some reason

            Debug.Log (microphone + " doesn't work!");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

}
