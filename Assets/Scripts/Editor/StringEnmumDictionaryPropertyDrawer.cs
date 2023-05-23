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
                if (KeyValuePairs.arraySize == 0)
                {
                    EditorGUILayout.LabelField("No elements");
                }
                else
                {
                    SerializedProperty childProps = KeyValuePairs;
                    Rect childRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect());
                    EditorGUI.BeginProperty(childRect, GUIContent.none, childProps);

                    ReorderableList list = new ReorderableList(property.serializedObject, childProps, true, true, true, true);
                    list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "List");
                    list.drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        SerializedProperty elementProperty = childProps.GetArrayElementAtIndex(index);
                        EditorGUI.indentLevel++;


                        if (elementProperty.isExpanded)
                        {
                            rect.y -= EditorGUIUtility.singleLineHeight * 1.5f;
                            rect.height += EditorGUIUtility.singleLineHeight * 3f;
                        }
                        elementProperty.isExpanded = EditorGUI.Foldout(rect, elementProperty.isExpanded, "Element " + index);

                        if (elementProperty.isExpanded)
                        {
                            //foldoutRect.y += EditorGUIUtility.singleLineHeight * 3f;
                            rect.height -= EditorGUIUtility.singleLineHeight * 3f;

                            EditorGUI.indentLevel++;
                            Rect minirect = rect;


                            minirect.y += EditorGUIUtility.singleLineHeight * 2.5f;
                            EditorGUI.PropertyField(minirect, elementProperty.FindPropertyRelative("key"));
                            minirect.y += EditorGUIUtility.singleLineHeight * 1.5f;
                            EditorGUI.PropertyField(minirect, elementProperty.FindPropertyRelative("value"));

                            EditorGUI.indentLevel--;
                        }

                        EditorGUI.indentLevel--;
                    };

                    list.DoLayoutList();


                    EditorGUI.EndProperty();
                    EditorGUI.indentLevel--;
                }
            }
        }
    }
}
