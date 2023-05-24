using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomPropertyDrawer(typeof(StringEnumDictionary<,>))]
public class StringEnmumDictionaryPropertyDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        SerializedProperty IDDir = property.FindPropertyRelative("IDDir");
        SerializedProperty ValueDir = property.FindPropertyRelative("ValueDir");
        EditorGUILayout.PropertyField(IDDir);
        EditorGUI.PropertyField(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), ValueDir);
        if (ValueDir.isExpanded)
        {
            EditorGUI.indentLevel++;
            SerializedProperty KeyValuePairs = ValueDir.FindPropertyRelative("KeyValuePairs");
            //EditorGUI.PropertyField(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), KeyValuePairs);

            //if (KeyValuePairs.isExpanded)
            {
                SerializedProperty childProps = KeyValuePairs;
                Rect childRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect());
                ReorderableList thislist = new ReorderableList(childProps.serializedObject, childProps, true, true, true, true);

                thislist.drawElementCallback = (Rect rect, int index, bool isActive, bool ifFocuesd) =>

                {

                    SerializedProperty element = childProps.GetArrayElementAtIndex(index);
                    EditorGUI.indentLevel++;
                    Rect foldOutRect = rect;
                    element.isExpanded = EditorGUI.Foldout(foldOutRect, element.isExpanded, "");
                    if (element.isExpanded)
                    {
                        Rect elementRect = rect;
                        elementRect.height = EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(elementRect, element.FindPropertyRelative("key"));
                        elementRect.y += EditorGUIUtility.singleLineHeight * 1.5f;
                        EditorGUI.PropertyField(elementRect, element.FindPropertyRelative("value"));
                    }
                    EditorGUI.indentLevel--;
                };
                //thislist.elementHeight = 0f;
                thislist.elementHeightCallback = (int index) =>
                    {
                        if (childProps.GetArrayElementAtIndex(index).isExpanded)
                        {
                            //float hight = childProps.GetArrayElementAtIndex(index).rectValue.h
                            return 3f * EditorGUIUtility.singleLineHeight;
                        }
                        return EditorGUIUtility.singleLineHeight;
                    };
                thislist.DoLayoutList();
            }
            EditorGUI.indentLevel--;
        }
    }
}
