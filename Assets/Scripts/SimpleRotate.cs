using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;


//usage: put this on Images in main menu to make them rotate (hopefully)
public class SimpleRotate : MonoBehaviour
{
    public Vector3 spin;

    public float spinAmount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //gameObject.transform.rotation.z = spinAmount;
        transform.Rotate(0f,0f,spinAmount);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f*Time.deltaTime)
    }
}
