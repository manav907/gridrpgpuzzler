using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelGenerator levelGenerator = target as LevelGenerator;
        DrawDefaultInspector();
        if(GUILayout.Button("try"))
        {
            levelGenerator.GenerateLevelFromList();
        }
    }
}