using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterData))]
public class CharacterDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CharacterData characterData = target as CharacterData;
        DrawDefaultInspector();
        if (GUILayout.Button("Save SO"))
        {
            EditorUtility.SetDirty(characterData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
