using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelDataSO))]
public class LevelDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);
        LevelDataSO levelDataSO = target as LevelDataSO;


        if (levelDataSO.posToCharacterData != null)
        {
            levelDataSO.loadDataifNotLoaded();
            EditorGUILayout.LabelField("Position To GameObject Dictionary:");
            foreach (var pair in levelDataSO.posToCharacterData)
            {
                //EditorGUILayout.LabelField($"Key: {pair.Key}, Value: {pair.Value.name}");
                GUILayout.BeginHorizontal();

                GUILayout.Label(pair.Key.ToString());
                if (GUILayout.Button("Edit " + pair.Value.name + pair.Value.InstanceID))
                {
                    Selection.activeObject = pair.Value;
                }
                if (GUILayout.Button("Remove "))
                {
                    levelDataSO.posToCharacterData.Remove(pair.Key);
                    break;
                }
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("CheckAtPos"), GUIContent.none, true);
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("importThisCharData"), GUIContent.none, true);
        serializedObject.ApplyModifiedProperties();


        if (GUILayout.Button("Add To Dictionary"))
        {
            levelDataSO.addToDictionary();
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("Manage Level Data File");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("SaveData"))
        {
            levelDataSO.SaveData();
        }
        if (GUILayout.Button("LoadData"))
        {
            levelDataSO.LoadData();
        }
        if (GUILayout.Button("Clear Data(no Save)"))
        {
            levelDataSO.ClearData();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("tryDiagonose"))
        {
            levelDataSO.tryDiagonose();
        }
    }
}

