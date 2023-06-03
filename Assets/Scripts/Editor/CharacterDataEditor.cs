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
        /* if (GUILayout.Button("Rename abilityNames"))
        {
            // Modify data in the scriptable object

            if (characterData.listOfAbility == null)
            {
                Debug.Log("List of Ability not Initlized");
                return;
            }
            foreach (Ability ability in characterData.listOfAbility)
            {
                //Debug.Log(ability.abilityString + " to " + ability.abilityName.ToString());
                ability.abilityString = ability.abilityName.ToString();
            }
        } */
        newCharacterData = (CharacterData)EditorGUILayout.ObjectField(GUIContent.none, newCharacterData, typeof(CharacterData), false);
        //if (GUILayout.Button("repalcedata"))
        if (newCharacterData != null)
        {
            characterData.ReplaceDataWithPreset(newCharacterData);
            newCharacterData = null;
            Debug.Log("Replaced Character Data with Preset");
        }
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("ListOfAbility"), true);
    }
}
