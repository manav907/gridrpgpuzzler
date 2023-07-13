using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LadderCollapseFunction))]
public class LadderCollapseFunctionEditor : Editor
{
    int currentsetDataWithID;
    int currentdoActionWithID;
    //int currentSetDataUsingTherorticalPosAtArrayIndex;
    Dictionary<string, int> VarirableDict;
    public override void OnInspectorGUI()
    {
        LadderCollapseFunction ladderCollapseFunction = target as LadderCollapseFunction;
        DrawDefaultInspector();
        EditorGUILayout.LabelField("Costom Inspect");
        SetUPOptionsDict();

        if (ladderCollapseFunction.invokeFunction.KeyValuePairs.Count == 0)
        {
            ladderCollapseFunction.invokeFunction.KeyValuePairs.Add
            (new KeyPair<LadderCollapseFunctionEnums, string>(LadderCollapseFunctionEnums.SetDataUsingTherorticalPosAtArrayIndex, ""));
            return;
        }
        //
        currentsetDataWithID = 0;
        currentdoActionWithID = 0;
        //currentSetDataUsingTherorticalPosAtArrayIndex = 0;
        //
        for (int i = 0; i < ladderCollapseFunction.invokeFunction.KeyValuePairs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            ladderCollapseFunction.invokeFunction.KeyValuePairs[i].key = (LadderCollapseFunctionEnums)EditorGUILayout.EnumPopup(ladderCollapseFunction.invokeFunction.KeyValuePairs[i].key);
            string currentValeueString = ladderCollapseFunction.invokeFunction.KeyValuePairs[i].value;
            int selectedindex = 0;
            if (VarirableDict.ContainsKey(currentValeueString))
                selectedindex = EditorGUILayout.Popup(VarirableDict[currentValeueString], ladderCollapseFunction.Varirables.ToArray());
            ladderCollapseFunction.invokeFunction.KeyValuePairs[i].value = ladderCollapseFunction.Varirables[selectedindex];
            if (GUILayout.Button("Delete"))
            {
                ladderCollapseFunction.invokeFunction.KeyValuePairs.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
            //
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            //
            if (ladderCollapseFunction.invokeFunction.KeyValuePairs[i].key == LadderCollapseFunctionEnums.setDataWithID)
            {
                if (ladderCollapseFunction.SetDataAtIndex.Count < currentsetDataWithID + 1)
                {
                    ladderCollapseFunction.SetDataAtIndex.Add(new ActionInputParams());
                }
                SerializedProperty SetDataAtIndexKeyPairs = serializedObject.FindProperty("SetDataAtIndex");
                SerializedProperty SetDataAtIndexKeyPair = SetDataAtIndexKeyPairs.GetArrayElementAtIndex(currentsetDataWithID);
                EditorGUILayout.PropertyField(SetDataAtIndexKeyPair, true);
                currentsetDataWithID++;
            }
            else if (ladderCollapseFunction.invokeFunction.KeyValuePairs[i].key == LadderCollapseFunctionEnums.doActionWithID)
            {
                if (ladderCollapseFunction.DoActionFromDataAtIndex.Count < currentdoActionWithID + 1)
                {
                    ladderCollapseFunction.DoActionFromDataAtIndex.Add(new ActionEffectParams());
                }
                SerializedProperty SetDataAtIndexKeyPairs = serializedObject.FindProperty("DoActionFromDataAtIndex");
                SerializedProperty SetDataAtIndexKeyPair = SetDataAtIndexKeyPairs.GetArrayElementAtIndex(currentdoActionWithID);
                EditorGUILayout.PropertyField(SetDataAtIndexKeyPair, true);
                /* EditorGUILayout.BeginHorizontal();
                ladderCollapseFunction.DoActionFromDataAtIndex[currentdoActionWithID].typeOfAction = (TypeOfAction)EditorGUILayout.EnumPopup(ladderCollapseFunction.DoActionFromDataAtIndex[currentdoActionWithID].typeOfAction);
                //ladderCollapseFunction.DoActionFromDataAtIndex[currentdoActionWithID].validTargets = (ValidTargets)EditorGUILayout.EnumPopup(ladderCollapseFunction.DoActionFromDataAtIndex[currentdoActionWithID].validTargets);
                //ladderCollapseFunction.DoActionFromDataAtIndex[currentdoActionWithID].includeSelf = EditorGUILayout.Toggle(ladderCollapseFunction.DoActionFromDataAtIndex[currentdoActionWithID].includeSelf);
                EditorGUILayout.EndHorizontal(); */
                currentdoActionWithID++;
            }
            else if (ladderCollapseFunction.invokeFunction.KeyValuePairs[i].key == LadderCollapseFunctionEnums.SetDataUsingTherorticalPosAtArrayIndex)
            {
                //do Nothing
            }
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;


        }
        if (GUILayout.Button("Add New ladderCollapseStep"))
        {
            ladderCollapseFunction.invokeFunction.KeyValuePairs.Add
            (new KeyPair<LadderCollapseFunctionEnums, string>(LadderCollapseFunctionEnums.SetDataUsingTherorticalPosAtArrayIndex, ""));
        }
        serializedObject.ApplyModifiedProperties();
        void SetUPOptionsDict()
        {
            VarirableDict = new Dictionary<string, int>();
            //Debug.Log(ladderCollapseFunction.Varirables);
            if (ladderCollapseFunction.Varirables == null)
            {
                ladderCollapseFunction.Varirables = new List<string>();
                ladderCollapseFunction.DoActionFromDataAtIndex = new List<ActionEffectParams>();
                ladderCollapseFunction.invokeFunction = new SerializableDictionary<LadderCollapseFunctionEnums, string>();
                ladderCollapseFunction.SetDataAtIndex = new List<ActionInputParams>();
            }
            if (ladderCollapseFunction.Varirables.Count == 0)
            {
                ladderCollapseFunction.Varirables.Add("New Varirable ");
            }
            for (int i = 0; i < ladderCollapseFunction.Varirables.Count; i++)
            {
                VarirableDict.Add(ladderCollapseFunction.Varirables[i], i);
                EditorGUILayout.BeginHorizontal();
                ladderCollapseFunction.Varirables[i] = EditorGUILayout.TextField(ladderCollapseFunction.Varirables[i]);
                if (GUILayout.Button("Delete"))
                {
                    ladderCollapseFunction.Varirables.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add New"))
            {
                string newKeyName = "New Key";
                if (VarirableDict.ContainsKey(newKeyName))
                {
                    Debug.LogError("Key Alredy Present");
                }
                else
                {
                    ladderCollapseFunction.Varirables.Add(newKeyName);
                }
            }

        }
    }

}
