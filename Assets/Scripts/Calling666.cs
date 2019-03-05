using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calling666 : MonoBehaviour
{
    //button
    private Button numberButton;
    private Button callButton;
    
    //Textbox
    //private TextBox dialTextBox;
    private Text dialText;
    
    // Start is called before the first frame update
    void Start()
    {
        numberButton = GameObject.Find("666_button").GetComponent<Button>();
        numberButton.onClick.AddListener(Dial6);
        
        //textbox
        dialText = GameObject.Find("DialText").GetComponent<Text>();
        dialText.text = "";


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Dial6()
    {
        dialText.text = "666";
    }

}
