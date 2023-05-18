using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[CustomEditor(typeof(LevelDataSO))]
public class LevelDataEditor : Editor
{
    Vector3Int checkAtPos = new Vector3Int();
    string NewName;
    string ChangeName;
    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();
        GUILayout.Space(10);
        LevelDataSO levelDataSO = target as LevelDataSO;

        EditorGUILayout.LabelField("Speical Load Buttons:");
        if (GUILayout.Button("Load"))
        {
            levelDataSO.LoadDataFromDictionary();
        }
        if (GUILayout.Button("Save"))
        {
            levelDataSO.SaveDataInDictionary();
        }
        //ID to Chard DATA
        EditorGUILayout.LabelField("IdToCharacterData:");
        if (levelDataSO.IdToCharacterData != null)
        {
            GUILayout.BeginHorizontal();
            NewName = EditorGUILayout.TextField(GUIContent.none, NewName);
            if (GUILayout.Button("New Character Preset"))
            {
                if (NewName == "")
                {
                    Debug.Log("Invalid name");
                    return;
                }
                try
                { levelDataSO.IdToCharacterData.Add(NewName, ScriptableObject.CreateInstance<CharacterData>()); }
                catch (ArgumentException)
                {
                    Debug.Log("Key alredy Prestn");
                }
            }
            GUILayout.EndHorizontal();

            foreach (var pair in levelDataSO.IdToCharacterData)
            {
                GUILayout.BeginHorizontal();
                //GUILayout.Label(pair.Key.ToString());
                if (GUILayout.Button(pair.Key))
                {
                    EditorUtility.OpenPropertyEditor(pair.Value);//Selection.activeObject = pair.Value;
                }
                //EditorGUILayout.ObjectField(pair.Value, typeof(CharacterData), false);//This Displayes the object
                if (GUILayout.Button("Remove "))
                {
                    levelDataSO.IdToCharacterData.Remove(pair.Key);
                    break;
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.LabelField("V3IntToID:");
            if (levelDataSO.V3IntToID != null)
            {
                GUILayout.BeginHorizontal();
                checkAtPos = EditorGUILayout.Vector3IntField(GUIContent.none, checkAtPos);
                string[] characterOptions = levelDataSO.IdToCharacterData.Keys.ToArray();
                int selectedIndex = EditorGUILayout.Popup(-1, characterOptions);
                if (selectedIndex != -1)
                {
                    try
                    {
                        levelDataSO.V3IntToID.Add(checkAtPos, characterOptions[selectedIndex]);
                    }
                    catch (ArgumentException)
                    {
                        Debug.Log("Key alredy Prestn");
                    }
                }
                GUILayout.EndHorizontal();

                foreach (var pair in levelDataSO.V3IntToID)
                {
                    GUILayout.BeginHorizontal();
                    //GUILayout.Label(pair.Key.ToString());
                    if (GUILayout.Button("Update Position" + pair.Key + " Of Character " + pair.Value))
                    {
                        //EditorUtility.OpenPropertyEditor(pair.Value);//Selection.activeObject = pair.Value;
                        Vector3Int oldPos = pair.Key;
                        string oldValue = pair.Value;
                        levelDataSO.V3IntToID.Remove(oldPos);
                        levelDataSO.V3IntToID.Add(checkAtPos, oldValue);
                        break;

                    }
                    //EditorGUILayout.ObjectField(pair.Value, typeof(CharacterData), false);//This Displayes the object
                    if (GUILayout.Button("Remove "))
                    {
                        levelDataSO.V3IntToID.Remove(pair.Key);
                        break;
                    }
                    GUILayout.EndHorizontal();
                }

            }
            else
            {
                if (GUILayout.Button("Load V3IntToID?"))
                {
                    levelDataSO.V3IntToID = new Dictionary<Vector3Int, string>();//needs to Change;
                }
            }


        }
        else
        {
            if (GUILayout.Button("Load Level Data?"))
            {
                levelDataSO.IdToCharacterData = new Dictionary<string, CharacterData>();
            }
        }
        //Vector3IntTo ID.
















        /////////////////////

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

        //CharacterData characterData = new CharacterData();
        checkAtPos = EditorGUILayout.Vector3IntField(GUIContent.none, checkAtPos);
        //characterData = (CharacterData)EditorGUILayout.ObjectField(GUIContent.none, characterData, typeof(CharacterData), false);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();


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


