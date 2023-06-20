using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileData))]
public class TileDataEditor : Editor
{
    Vector2 scroll = new Vector2();
    int breakGrid = 1;
    public override void OnInspectorGUI()
    {
        TileData tileData = target as TileData;
        DrawDefaultInspector();
        breakGrid = EditorGUILayout.IntField(breakGrid);
        scroll = EditorGUILayout.BeginScrollView(scroll);
        if(breakGrid==0)
        {
            breakGrid=1;
        }
        for (int i = 0; i < tileData.tiles.Length; i++)
        {
            if (i % breakGrid == 0)
            {
                GUILayout.BeginHorizontal();
            }
            var texture = AssetPreview.GetAssetPreview(tileData.tiles[i]);
            GUILayout.Label(tileData.tiles[i].name);
            GUILayout.Box(texture, GUILayout.ExpandWidth(false));
            if ((i + 1) % breakGrid == 0 || i == tileData.tiles.Length - 1)
            {
                GUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
    }
}
