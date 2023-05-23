using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CharacterData))]
public class CharacterDataPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        if (GUI.Button(position, "Edit Properties"))
        {
            if (property.objectReferenceValue == null)
            {
                property.objectReferenceValue = ScriptableObject.CreateInstance<CharacterData>();
                if (property.objectReferenceValue == null)
                    Debug.Log("this is still null");
            }
            EditorUtility.OpenPropertyEditor(property.objectReferenceValue);
            //EditorUtility.OpenPropertyEditor(ScriptableObject.CreateInstance<CharacterData>());//This works but does not get saved obviously
        }
        EditorGUI.EndProperty();
    }
}
