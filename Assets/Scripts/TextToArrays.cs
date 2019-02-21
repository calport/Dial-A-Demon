using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class TextToArrays: MonoBehaviour
{
    /*private void Start()
    {
        string[][] text = TextToArray(
            "Text/testFile");
    }*/

    public string[][] TextToArray(string filePath)
    {
        //int property = 5;
        string text = Resources.Load<TextAsset>(filePath).text;
        if (Resources.Load<TextAsset>(filePath) != null)
        {
            List<string>eachLine = new List<string>();
            eachLine.AddRange(text.Split('\n'));
            List<List<string>> textList = new List<List<string>>();
            string[][] textArray = new string[eachLine.Count][];
            for (int i = 0; i < eachLine.Count; i++) {	
                //eachLine[i] = System.Text.RegularExpressions.Regex.Replace(eachLine[i], @"\b\s+\b", " ");
                textArray[i] = eachLine[i].Split('|').ToArray();   
            }

            /*for (int i = 0; i < textArray.Length; i++)
            {
                            foreach (var item in textArray[i])
                            {
                                Debug.Log(","+item + ",");
                            }
                            Debug.Log(" ");
            }*/

            return textArray;
        }
        else
        {
            //Debug.Log("CANTFIND");
            return null;
        }
        
    }
}
