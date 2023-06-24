using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[CustomEditor(typeof(LevelDataSO))]
public class LevelDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelDataSO levelDataSO = target as LevelDataSO;
        DrawDefaultInspector();
        if (GUILayout.Button("CreateNewLevel"))
        {
            if (levelDataSO.inputLevel != null)
                levelDataSO.createLevel();

        }
        //EditorGUILayout.pop
        /* if (GUILayout.Button("Check Dictionary"))
        {
            levelDataSO.GenerateV3IntToCharacterDataDir();
        } */
    }
}


