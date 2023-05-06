using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
