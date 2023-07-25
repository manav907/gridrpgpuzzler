using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterData))]
public class CharacterDataEditor : Editor
{
    [SerializeField] CharacterData newCharacterData;

    public override void OnInspectorGUI()
    {
        CharacterData characterData = target as CharacterData;
        DrawDefaultInspector();
        newCharacterData = (CharacterData)EditorGUILayout.ObjectField(GUIContent.none, newCharacterData, typeof(CharacterData), false);
        //if (GUILayout.Button("repalcedata"))
        if (newCharacterData != null)
        {
            characterData.ReplaceDataWithPreset(newCharacterData);
            newCharacterData = null;
            Debug.Log("Replaced Character Data with Preset");
        }
        if (GUILayout.Button("Save SO"))
        {
            EditorUtility.SetDirty(characterData);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("ListOfAbility"), true);
    }
}
