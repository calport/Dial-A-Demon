using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    private int numberOfBubbles;
    public int moveAmount; 
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
       //Debug.Log("number of Children: " + transform.childCount);

       // numberOfBubbles = transform.childCount; 
   
        
        //Debug.Log("number of children" + numberOfBubbles);
        
    
        
        /*foreach (Transform child in transform)
        {
            //child is your child transform
            //new Vector2(transform.posistion.x + 10, transform.position.y + 10);
          // this.GetComponentInChildren<Transform>().position = new Vector2(transform.position.x + 10, transform.position.y + moveAmount);
          //  moveAmount += 100; 
        }*/

        
    }

    public void MoveChat()
    {
        /*for(int i = 0; i < numberOfBubbles; i++)
        {
            this.GetComponentInChildren<Transform>().position = new Vector2(transform.position.x + 10, transform.position.y + moveAmount);
            moveAmount += 100; 
            //  Debug.Log("I have ran through the loop " + i + " times");
        }*/
    }
}
