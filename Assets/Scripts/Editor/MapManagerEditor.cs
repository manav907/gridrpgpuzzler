using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapManager mapManager = target as MapManager;
        DrawDefaultInspector();
        if (GUILayout.Button("LoadMapDataFromSO"))
        {
            mapManager.LoadMapDataFromSO();
        }
        if (GUILayout.Button("OverWriteMapDataToSO"))
        {
            mapManager.OverWriteMapDataToSO();
        }
    }
}
