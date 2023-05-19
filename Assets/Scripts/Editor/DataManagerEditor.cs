using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[CustomEditor(typeof(DataManager))]
public class DataManagerEditor : Editor
{
    /* Vector3Int checkAtPos;
    CharacterData characterData;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);
        DataManager dataManager = target as DataManager;
        var levelDataSO = dataManager.EditLevel;
        if (levelDataSO.posToCharacterData != null && Application.isPlaying)
        {
            if (GUILayout.Button("View Current Character"))
            {
                dataManager.viewCurrentCharacter();
            }
            EditorGUILayout.LabelField("Position To Character Data Dictionary:");
            foreach (var pair in levelDataSO.posToCharacterData)
            {
                levelDataSO.loadDataifNotLoaded();
                GUILayout.BeginHorizontal();

                if (GUILayout.Button(pair.Key.ToString()))
                {
                    dataManager.setCameraPos(pair.Key);
                }
                if (GUILayout.Button("Edit " + pair.Value.name + pair.Value.InstanceID))
                {
                    Selection.activeObject = pair.Value;
                }
                if (GUILayout.Button("Remove "))
                {
                    levelDataSO.posToCharacterData.Remove(pair.Key);
                    break;
                }
                if (GUILayout.Button("Get Current Data"))
                {
                    dataManager.mapManager.cellDataDir[pair.Key].characterAtCell.GetComponent<CharacterControllerScript>().saveCharacterDataToCSO();
                }
                GUILayout.EndHorizontal();
            }


            EditorGUILayout.LabelField("ADD Data?:");
            GUILayout.BeginHorizontal();
            checkAtPos = EditorGUILayout.Vector3IntField(GUIContent.none, checkAtPos);
            characterData = (CharacterData)EditorGUILayout.ObjectField(GUIContent.none, characterData, typeof(CharacterData), false);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Try Add This Data To Dictionary"))
            {
                if (levelDataSO.posToCharacterData.ContainsKey(checkAtPos) || characterData == null)
                {
                    Debug.Log("Cannot Add Key as \n" + checkAtPos + characterData);
                    return;
                }
                levelDataSO.posToCharacterData.Add(checkAtPos, characterData);
            }
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


    } */
}
