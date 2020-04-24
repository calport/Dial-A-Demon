using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileImporter : MonoBehaviour
{
    void Start()
       { // Start is called before the first frame update
   
        
    }

    // Update is called once per frame
    void GetText()
    { 
        var textAsset = Resources.Load<TextAsset>("InkText/" + name);
    }
}
