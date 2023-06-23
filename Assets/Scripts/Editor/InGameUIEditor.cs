using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(InGameUI))]
public class InGameUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InGameUI inGameUI = target as InGameUI;
        DrawDefaultInspector();
        if (GUILayout.Button("Set ToolTip"))
        {
            inGameUI.setTip("Thi Will be Empt ylad");
        }
    }
}
