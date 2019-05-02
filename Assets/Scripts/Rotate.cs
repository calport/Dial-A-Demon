using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    private float startTime;
    private float nowTime;
    public Space rotateSpace;
    private float noiseOffestX = 0f;
    private float noiseOffestY = 0.5f;
    private float noiseOffestZ = 1f;

    public float Speed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        nowTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        nowTime = nowTime + Time.deltaTime;
        var lenghTime = nowTime - startTime;
        var noiseX = Mathf.PerlinNoise(lenghTime/5f, noiseOffestX);
        var noiseY = Mathf.PerlinNoise(lenghTime/5f, noiseOffestY);
        var noiseZ = Mathf.PerlinNoise(lenghTime/5f, noiseOffestZ);
        var newRot = new Vector3(noiseX,noiseY,noiseZ);
        transform.Rotate(newRot *Time.deltaTime * Speed, rotateSpace);
    }
}
