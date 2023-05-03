using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    private MapManager mapManager;

    private void OnEnable()
    {
        mapManager = target as MapManager;
    }
    /*
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);

            if (mapManager.PositionToGameObject != null)
            {
                EditorGUILayout.LabelField("Position To GameObject Dictionary:");
                foreach (var pair in mapManager.PositionToGameObject)
                {
                    //EditorGUILayout.LabelField($"Key: {pair.Key}, Value: {pair.Value.name}");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(pair.Key.ToString());
                    GUILayout.Label(pair.Value.name);
                    GUILayout.EndHorizontal();
                }
            }
        }
    */
}
[CustomEditor(typeof(DataManager))]
public class DataManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);
        DataManager dataManager = target as DataManager;
        var levelDataSO = dataManager.EditLevel;
        if (levelDataSO.posToCharacterData != null&& Application.isPlaying)
        {
            foreach (var pair in levelDataSO.posToCharacterData)
            {
                levelDataSO.loadDataifNotLoaded();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Edit "))
                {
                    Selection.activeObject = pair.Value;
                }
                GUILayout.Label(pair.Key.ToString());
                GUILayout.Label(pair.Value.name + pair.Value.InstanceID);
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
}

[CustomEditor(typeof(LevelDataSO))]
public class LevelDataSOEditor : Editor
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
                if (GUILayout.Button("Edit "))
                {
                    Selection.activeObject = pair.Value;
                }
                GUILayout.Label(pair.Key.ToString());
                GUILayout.Label(pair.Value.name + pair.Value.InstanceID);
                if (GUILayout.Button("Remove "))
                {
                    levelDataSO.posToCharacterData.Remove(pair.Key);
                    break;
                }
                /* if (GUILayout.Button("Try Change Pos"))
                {
                    if (levelDataSO.posToCharacterData.ContainsKey(levelDataSO.CheckAtPos))
                    {
                        Debug.Log("The new Posisiton is alredy in dictonary cannot update posistion of this character");
                    }
                    else
                    {
                        levelDataSO.posToCharacterData.Remove(pair.Key);
                        levelDataSO.posToCharacterData.Add(levelDataSO.CheckAtPos, pair.Value);
                        break;
                    }
                } */
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
[CustomEditor(typeof(CharacterData))]
public class CharacterDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Rename abilityNames"))
        {
            // Modify data in the scriptable object
            CharacterData characterData = target as CharacterData;
            foreach (Ability ability in characterData.listOfAbility)
            {
                //Debug.Log(ability.abilityString + " to " + ability.abilityName.ToString());
                ability.abilityString = ability.abilityName.ToString();
            }
        }
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("ListOfAbility"), true);
    }
}
[CustomEditor(typeof(CharacterAnimationData))]
public class CharacterAnimationDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Rename FileNames"))
        {
            // Modify data in the scriptable object
            CharacterAnimationData characterAnimationData = target as CharacterAnimationData;
            string newNameForFile = characterAnimationData.nameEnum.ToString() + " CharacterAnimationData";
            // Rename the SO asset
            string path = AssetDatabase.GetAssetPath(characterAnimationData);
            string errorMsg = AssetDatabase.RenameAsset(path, newNameForFile);
            if (string.IsNullOrEmpty(errorMsg))
            {
                // Refresh the editor to update the file name
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("Failed to rename asset: " + errorMsg);
            }

        }
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("ListOfAbility"), true);
    }
}
[CustomEditor(typeof(ReticalManager))]
public class ReticalManagerEditor : Editor
{
    /*
    public override void OnInspectorGUI()
    {
        ReticalManager reticalManager = target as ReticalManager;
        DrawDefaultInspector();
        //Creating a Syle
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;
        //Drawing said Style
        EditorGUILayout.LabelField("Inspector Stuff", style);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Draw Retical at Point"))
        {
            reticalManager.setReticalToFromPoint();
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fromPoint"), true);
        //serializedObject.ApplyModifiedProperties();
        GUILayout.EndHorizontal();


        if (GUILayout.Button("do Action"))
        {
            reticalManager.doOnClick();
        }

    }
    */
}