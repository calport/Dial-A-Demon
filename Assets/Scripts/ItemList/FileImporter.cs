using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FileImporter
{
    // Update is called once per frame
    public static void GetText(string name)
    { 
        var textAsset = Resources.Load<TextAsset>("InkText/" + name);
    }

    public static GameObject GetRitual(string name)
    {
        return Resources.Load<GameObject>("Prefabs/Rituals/" + name);
    }
}
