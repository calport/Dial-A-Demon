using System;
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
    private float[] averageSpectrum;
    public bool adjustmentFinished { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        averageSpectrum = new float[numberOfSamples];
        StartCoroutine(AdjustAverageSound());
    }

    // Update is called once per frame
    void Update()
    {
        if (adjustmentFinished)
        {
            // initialize our float array
            float[] spectrum = new float[numberOfSamples];

            // populate array with fequency spectrum data
            audioSource.GetSpectrumData(spectrum, 0, fftWindow);

            // loop over audioSpectrumObjects and modify according to fequency spectrum data
            // this loop matches the Array element to an object on a One-to-One basis.
            for (int i = 0; i < audioSpectrumObjects.Length; i++)
            {


                // apply height multiplier to intensity
                float intensity = Mathf.Max(spectrum[i] - averageSpectrum[i],0.0f) * heightMultiplier;
                if (i == 0) largestSound = 0f;
                if (spectrum[i] > largestSound) largestSound = spectrum[i];

                // calculate object's scale
                float lerpY = Mathf.Lerp(audioSpectrumObjects[i].localScale.y, intensity, lerpTime);
                Vector3 newScale = new Vector3(audioSpectrumObjects[i].localScale.x, lerpY,
                    audioSpectrumObjects[i].localScale.z);

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
        else
        {
            // initialize our float array
            float[] spectrum = new float[numberOfSamples];

            // populate array with fequency spectrum data
            audioSource.GetSpectrumData(spectrum, 0, fftWindow);

            // loop over audioSpectrumObjects and modify according to fequency spectrum data
            // this loop matches the Array element to an object on a One-to-One basis.
            for (int i = 0; i < audioSpectrumObjects.Length; i++)
            {


                // apply height multiplier to intensity
                float intensity = 0f;
                largestSound = 0f;
                
                float lerpY = Mathf.Lerp(audioSpectrumObjects[i].localScale.y, intensity, lerpTime);
                Vector3 newScale = new Vector3(audioSpectrumObjects[i].localScale.x, lerpY,
                    audioSpectrumObjects[i].localScale.z);
                audioSpectrumObjects[i].localScale = newScale;

            }
        }

    }
    
    IEnumerator AdjustAverageSound()
    {
        int getSampleTime = 100;
        int samplingTime = 0;
        float[] totalSpectrum = new float[numberOfSamples];
        while (samplingTime<getSampleTime)
        {
            float[] spectrum = new float[numberOfSamples];
            audioSource.GetSpectrumData(spectrum, 0, fftWindow);
            for (int i=0; i < numberOfSamples; i++)
            {
                totalSpectrum[i] += spectrum[i];
            }
            samplingTime++;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        for (int i=0; i < numberOfSamples; i++)
        {
            totalSpectrum[i] = totalSpectrum[i] / (float)getSampleTime;
        }

        averageSpectrum = totalSpectrum;
        adjustmentFinished = true;
        Debug.Log("Finished Average Sound Adjusting");
    }
}
