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
[CustomEditor(typeof(LevelDataSO))]
public class LevelDataSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);
        LevelDataSO levelDataSO = target as LevelDataSO;
        if (levelDataSO.levelData != null)
        {
            EditorGUILayout.LabelField("Position To GameObject Dictionary:");
            foreach (var pair in levelDataSO.levelData.posToCharacterData)
            {
                //EditorGUILayout.LabelField($"Key: {pair.Key}, Value: {pair.Value.name}");
                GUILayout.BeginHorizontal();
                GUILayout.Label(pair.Key.ToString());
                GUILayout.Label(pair.Value.name);
                GUILayout.EndHorizontal();
            }
        }
        if (GUILayout.Button("addToDictionary"))
        {
            levelDataSO.addToDictionary();
        }
        if (GUILayout.Button("SaveData"))
        {
            levelDataSO.SaveData();
        }
        if (GUILayout.Button("LoadData"))
        {
            levelDataSO.LoadData();
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
[CustomEditor(typeof(ReticalManager))]
public class ReticalManagerEditor : Editor
{
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
        serializedObject.ApplyModifiedProperties();
        GUILayout.EndHorizontal();


        if (GUILayout.Button("do Action"))
        {
            reticalManager.doOnClick();
        }

    }
}
