using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LadderCollapseFunction))]
public class LadderCollapseFunctionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LadderCollapseFunction ladderCollapseFunction = target as LadderCollapseFunction;
        DrawDefaultInspector();
        //
        SerializedProperty invokeFunctionKeyValuePairs = serializedObject.FindProperty("invokeFunction.KeyValuePairs");
        SerializedProperty SetDataAtIndexKeyValuePairs = serializedObject.FindProperty("SetDataAtIndex.KeyValuePairs");
        SerializedProperty DoActionFromDataAtIndexKeyValuePairs = serializedObject.FindProperty("DoActionFromDataAtIndex.KeyValuePairs");
        //
        foreach (var keyPair in ladderCollapseFunction.invokeFunction.KeyValuePairs)
        {
            EditorGUILayout.BeginHorizontal();
            keyPair.key = (LadderCollapseFunctionEnums)EditorGUILayout.EnumPopup(keyPair.key);
            keyPair.value = (string)EditorGUILayout.TextField(keyPair.value);
            EditorGUILayout.EndHorizontal();
            //SerializedProperty invokeFunctionEnum = invokeFunctionKeyValuePairs.GetArrayElementAtIndex(keyPair.value);
            //EditorGUILayout.PropertyField(invokeFunctionEnum.FindPropertyRelative("value"), GUIContent.none);
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            if (keyPair.key == LadderCollapseFunctionEnums.SetDataAtArrayIndex)
            {
                EditorGUILayout.PropertyField(keyValuePairProperty.FindPropertyRelative("key"), GUIContent.none);
                
                // keyValuePairProperty = SetDataAtIndexKeyValuePairs.GetArrayElementAtIndex(keyPair.value);
                //EditorGUILayout.PropertyField(keyValuePairProperty.FindPropertyRelative("key"), GUIContent.none);

            }
            else if (keyPair.key == LadderCollapseFunctionEnums.PerformActionFromDataAtArrayIndex)
            {
                //ladderCollapseFunction.DoActionFromDataAtIndex.KeyValuePairs[keyPair.value].key = (TypeOfAction)EditorGUILayout.EnumPopup(ladderCollapseFunction.DoActionFromDataAtIndex.KeyValuePairs[keyPair.value].key);
            }
            EditorGUI.indentLevel--; EditorGUI.indentLevel--;
            //EditorGUILayout.EndHorizontal();
        }
        serializedObject.ApplyModifiedProperties();
    }

}
