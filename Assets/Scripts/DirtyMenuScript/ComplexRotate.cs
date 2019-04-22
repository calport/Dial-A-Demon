using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComplexRotate : MonoBehaviour, IDragHandler
{
    public Vector3 spin;
    public float spinSpeed;
    float spinAmount;
    public float speedChangeAdjust = 1f;
    private float x;
    private float y;
    // Start is called before the first frame update
    void Start()
    {
        spinAmount = spinSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //Resistance();
        //gameObject.transform.rotation.z = spinAmount;
        transform.Rotate(0f,0f,spinAmount);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f*Time.deltaTime)
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Mouse Drag");
        var x1 = Input.GetAxis("Mouse X");
        var y1 = Input.GetAxis("Mouse Y");
        var speed = new Vector3((x1-x), (y1-y),0f);
        x = x1;
        y = y1;
        var direction = Mathf.Abs(Vector3.Cross(new Vector3(-1f, -1f, 0f), speed).z)/Vector3.Cross(new Vector3(-1f, -1f, 0f), speed).z;
        /*var speedchange = direction * speed.magnitude;
        spinAmount += speedchange*speedChangeAdjust;
        Convert.ToSingle(spinAmount);*/
        spinAmount += Convert.ToSingle(direction * 0.03);

    }

    void Resistance()
    {
        if (Mathf.Abs(spinSpeed - spinAmount) > 0.001f)
        {
            spinAmount -= (spinAmount-spinSpeed) * Time.deltaTime / 3f;
            Debug.Log(spinAmount);
        }
        Convert.ToSingle(spinAmount);
    }
}
