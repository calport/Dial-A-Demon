using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SerializeManager
{
    public static void Serialize2DArray<T>(T[][] array,out List<int> lengthInfo, out List<T> content)
    {
        lengthInfo = new List<int>();
        content = new List<T>();
        foreach (var smallArray in array)
        {
            lengthInfo.Add(smallArray.Length);
            foreach (var item in smallArray)
                content.Add(item);
        }
    }
    
    public static void Serialize2DList<T>(List<List<T>> list,out List<int> lengthInfo, out List<T> content)
    {
        lengthInfo = new List<int>();
        content = new List<T>();
        foreach (var smallList in list)
        {
            lengthInfo.Add(smallList.Count);
            foreach (var item in smallList)
                content.Add(item);
        }
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

//   public static T ReadFromJson<T>(string jsonPath)
//   {
//       //read the file
//       StreamReader sr = new StreamReader(jsonPath);
//       var json = sr.ReadToEnd();
//       T file = JsonConvert.DeserializeObject<T>(json);
//       return file;
//   }
    
    public static string ReadJsonString(string jsonPath)
    {
        //read the files
        StreamReader sr = new StreamReader(jsonPath);
        var json = sr.ReadToEnd();
        return json;
    }

//    public static void SaveToJson<T>(string jsonPath,T fileToSave)
//    {
//        
//        string json = JsonConvert.SerializeObject(fileToSave);
//        File.WriteAllText(jsonPath, json, Encoding.UTF8);
//    }
    
    public static void SaveJson(string jsonPath,string json)
    {
        File.WriteAllText(jsonPath, json, Encoding.UTF8);
    }
    
    
    public struct JsonDateTime {
        public long value;
        public JsonDateTime(long value)
        {
            this.value = value;
        }
        public static implicit operator DateTime(JsonDateTime jdt) {
            return DateTime.FromFileTime(jdt.value);
        }
        public static implicit operator JsonDateTime(DateTime dt) {
            JsonDateTime jdt = new JsonDateTime();
            jdt.value = dt.ToFileTime();
            return jdt;
        }
    }
    
    public struct JsonTimeSpan {
        public long value;
        public JsonTimeSpan(long value)
        {
            this.value = value;
        }
        public static implicit operator TimeSpan(JsonTimeSpan jts)
        {
            TimeSpan newSpan = TimeSpan.Zero + TimeSpan.FromSeconds(jts.value);
            return newSpan;
        }
        public static implicit operator JsonTimeSpan(TimeSpan ts) {
            JsonTimeSpan jts = new JsonTimeSpan();
            jts.value = ts.Seconds;
            return jts;
        }
    }
    
    
}
