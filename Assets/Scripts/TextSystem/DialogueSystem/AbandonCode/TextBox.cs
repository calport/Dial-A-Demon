using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TextInputManager.Instance.textBox = gameObject.GetComponent<Text>();
        Debug.Log("Test box attached");
    }

}
