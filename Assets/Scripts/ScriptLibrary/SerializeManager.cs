using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SerializeManager
{
    public static Dictionary<List<int>, List<T>> Serialize2DArray<T>(T[][] array)
    {
        List<int> lengthInfo = new List<int>();
        List<T> info = new List<T>();
        foreach (var smallArray in array)
        {
            lengthInfo.Add(smallArray.Length);
            foreach (var item in smallArray)
            {
                info.Add(item);
            }
        }

        Dictionary<List<int>, List<T>> dic = new Dictionary<List<int>, List<T>>();
        dic.Add(lengthInfo,info);
        return dic;
    }
    
    public static Dictionary<List<int>, List<T>> Serialize2DList<T>(List<List<T>> list)
    {
        List<int> lengthInfo = new List<int>();
        List<T> info = new List<T>();
        foreach (var smallList in list)
        {
            lengthInfo.Add(smallList.Count);
            foreach (var item in smallList)
            {
                info.Add(item);
            }
        }

        Dictionary<List<int>, List<T>> dic = new Dictionary<List<int>, List<T>>();
        dic.Add(lengthInfo,info);
        return dic;
    }
    
    public static T[][] Deserialize2DArray<T>(List<int> lengthInfo, List<T> info)
    {
        int lengthLabel = 0;
        List<T[]> finalList = new List<T[]>();
        for (int i = 0; i < lengthInfo.Count; i++)
        {
            var length = lengthInfo[i];
            List<T> list = new List<T>();
            for (int x = 0; x < length; x++)
            {
                list.Add(info[lengthLabel+x]);
                
            }
            finalList.Add(list.ToArray());
            lengthLabel += length;
        }

        return finalList.ToArray();
    }
    
    public static List<List<T>> Deserialize2DList<T>(List<int> lengthInfo, List<T> info)
    {
        int lengthLabel = 0;
        List<List<T>> finalList = new List<List<T>>();
        for (int i = 0; i < lengthInfo.Count; i++)
        {
            var length = lengthInfo[i];
            List<T> list = new List<T>();
            for (int x = 0; x < length; x++)
            {
                list.Add(info[lengthLabel+x]);
                
            }
            finalList.Add(list);
            lengthLabel += length;
        }

        return finalList;
    }

    public static T ReadFromJson<T>(string jsonPath)
    {
        //read the file
        StreamReader sr = new StreamReader(jsonPath);
        var json = sr.ReadToEnd();
        T file = JsonUtility.FromJson<T>(json);
        return file;
    }
    
    public static string ReadJsonString(string jsonPath)
    {
        //read the file
        StreamReader sr = new StreamReader(jsonPath);
        var json = sr.ReadToEnd();
        return json;
    }

    public static void SaveToJson<T>(string jsonPath,T fileToSave)
    {
        string json = JsonUtility.ToJson(fileToSave);
        File.WriteAllText(jsonPath, json, Encoding.UTF8);
    }
    
    public static void SaveJson(string jsonPath,string json)
    {
        File.WriteAllText(jsonPath, json, Encoding.UTF8);
    }
}
