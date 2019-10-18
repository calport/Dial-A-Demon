using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Game))]
public class GameEditor : Editor
{
    public override void OnInspectorGUI() {

        DrawDefaultInspector();
        string jsonPath = Application.persistentDataPath + "/save.save";

        if(GUILayout.Button("Delete Saving File"))
        {
            if(File.Exists(jsonPath)) File.Delete(jsonPath);
        }

    }
}
