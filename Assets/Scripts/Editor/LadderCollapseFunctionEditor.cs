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
        EditorGUILayout.LabelField("Costom Inspect");

        for (int i = 0; i < ladderCollapseFunction.invokeFunction.KeyValuePairs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            ladderCollapseFunction.invokeFunction.KeyValuePairs[i].key = (LadderCollapseFunctionEnums)EditorGUILayout.EnumPopup(ladderCollapseFunction.invokeFunction.KeyValuePairs[i].key);
            ladderCollapseFunction.invokeFunction.KeyValuePairs[i].value = EditorGUILayout.IntField(ladderCollapseFunction.invokeFunction.KeyValuePairs[i].value);
            int ID = ladderCollapseFunction.invokeFunction.KeyValuePairs[i].value;
            EditorGUILayout.EndHorizontal();
            //
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            //
            if (ladderCollapseFunction.invokeFunction.KeyValuePairs[i].key == LadderCollapseFunctionEnums.setDataWithID)
            {

                if (ladderCollapseFunction.SetDataAtIndex.KeyValuePairs.Count < ID + 1)
                {

                    ladderCollapseFunction.SetDataAtIndex.KeyValuePairs.Add(new KeyPair<ActionInputParams, string>(new ActionInputParams(), ""));
                    return;
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("AffectsVariableID");
                ladderCollapseFunction.SetDataAtIndex.KeyValuePairs[ID].value = EditorGUILayout.TextField(ladderCollapseFunction.SetDataAtIndex.KeyValuePairs[ID].value);
                EditorGUILayout.EndHorizontal();
                SerializedProperty SetDataAtIndexKeyPairs = serializedObject.FindProperty("SetDataAtIndex.KeyValuePairs");
                SerializedProperty SetDataAtIndexKeyPair = SetDataAtIndexKeyPairs.GetArrayElementAtIndex(ID);
                SerializedProperty actionInputParams = SetDataAtIndexKeyPair.FindPropertyRelative("key");
                //ladderCollapseFunction.SetDataAtIndex.KeyValuePairs[ID].key = 
                EditorGUILayout.PropertyField(actionInputParams);
            }
            else if (ladderCollapseFunction.invokeFunction.KeyValuePairs[i].key == LadderCollapseFunctionEnums.doActionWithID)
            {
                if (ladderCollapseFunction.DoActionFromDataAtIndex.KeyValuePairs.Count < ID + 1)
                {
                    ladderCollapseFunction.DoActionFromDataAtIndex.KeyValuePairs.Add(new KeyPair<TypeOfAction, string>(TypeOfAction.apply_Damage, ""));
                    return;
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("AffectsVariableID");
                ladderCollapseFunction.DoActionFromDataAtIndex.KeyValuePairs[ID].value = EditorGUILayout.TextField(ladderCollapseFunction.DoActionFromDataAtIndex.KeyValuePairs[ID].value);
                EditorGUILayout.EndHorizontal();
                ladderCollapseFunction.DoActionFromDataAtIndex.KeyValuePairs[ID].key = (TypeOfAction)EditorGUILayout.EnumPopup(ladderCollapseFunction.DoActionFromDataAtIndex.KeyValuePairs[ID].key);
            }
            else
            {
                //do Nothing
            }
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;


        }
        if (GUILayout.Button("Add New ladderCollapseStep"))
        {
            ladderCollapseFunction.invokeFunction.KeyValuePairs.Add
            (new KeyPair<LadderCollapseFunctionEnums, int>(LadderCollapseFunctionEnums.SetDataUsingTherorticalPosAtArrayIndex, 0));
        }
        serializedObject.ApplyModifiedProperties();

    }

}
