using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.iOS;
using UnityEngine.EventSystems;
using Button = UnityEngine.Experimental.UIElements.Button;

public class test : MonoBehaviour, IPointerEnterHandler//
{
    public Text PlayerWritten;
    public static string ButtonText;
    

    // Start is called before the first frame update
    void Start()
    {    
        //PlayerWritten.text = "Nothing yet!!!";
        Touch touch = Input.GetTouch(0);
        
    }

    // Update is called once per frame
    void Update()
    {
        //PlayerWritten.text = "Awesome!!!";

       /* Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            PlayerWritten.text = "Began!!!";
        }
        
        if (touch.phase == TouchPhase.Moved)
        {
            PlayerWritten.text = "Moved!!!";
        }
        
      
        if (touch.phase == TouchPhase.Ended)
        {
            PlayerWritten.text = "Ended!!!";
        }*/ 

    }


    public void OnPointerEnter(PointerEventData eventData)
    {
            PlayerWritten.text = ButtonText;
     
        /*
        if (touch.phase == TouchPhase.Stationary)
        {
            PlayerWritten.text = "magicccc!!!";
        }*/
    }
    
 

  
    
    
    
}
