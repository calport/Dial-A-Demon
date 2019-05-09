using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    [Range(1, 10000)] public float heightMultiplier=50.0f;
    [Range(64, 8192)] public int numberOfSamples = 1024;
    public FFTWindow fftWindow;
    public Transform[] audioSpectrumObjects;
    public float lerpTime = 1;
    public float largestSound = 0;
    private AudioSource audioSource;
    [Range(1, 100)] public float visualThreshold = 2.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // initialize our float array
        float[] spectrum = new float[numberOfSamples];

        // populate array with fequency spectrum data
        audioSource.GetSpectrumData(spectrum, 0, fftWindow);
        
        // loop over audioSpectrumObjects and modify according to fequency spectrum data
        // this loop matches the Array element to an object on a One-to-One basis.
        for(int i = 0; i < audioSpectrumObjects.Length; i++)
        {


            // apply height multiplier to intensity
            float intensity = spectrum[i] * heightMultiplier;
            if (i == 0) largestSound = 0f;
            if (spectrum[i] > largestSound) largestSound = spectrum[i];

            // calculate object's scale
            float lerpY = Mathf.Lerp(audioSpectrumObjects[i].localScale.y,intensity,lerpTime);
            Vector3 newScale = new Vector3( audioSpectrumObjects[i].localScale.x, lerpY, audioSpectrumObjects[i].localScale.z);

            // appply new scale to object
            if (newScale.y < visualThreshold)
            {
                audioSpectrumObjects[i].localScale = newScale;
            }
            else
            {
                newScale.y = visualThreshold;
                audioSpectrumObjects[i].localScale = newScale;
            }
        }
        
    }
}
