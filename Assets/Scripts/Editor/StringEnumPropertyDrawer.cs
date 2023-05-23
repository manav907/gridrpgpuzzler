using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StringEnum))]
public class StringEnumPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {



        //base.OnGUI(position, property, label);
        SerializedProperty characterOptions = property.FindPropertyRelative("options");
        /* string[] options = new string[characterOptions.arraySize];
        for (int i = 0; i < characterOptions.arraySize; i++)
        {
            options[i] = characterOptions.GetArrayElementAtIndex(i).stringValue;
        } */
        string[] options = new string[3];
        options[0] = "one";
        options[1] = "2one";
        options[2] = "3one";
        int index = property.FindPropertyRelative("selectedIndex").intValue;

        EditorGUI.BeginProperty(position, label, property);
        property.FindPropertyRelative("selectedIndex").intValue = EditorGUI.Popup(position, index, options);
        EditorGUI.EndProperty();
    }
}