using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    private MapManager mapManager;

    private void OnEnable()
    {
        mapManager = target as MapManager;
    }
    /*
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);

            if (mapManager.PositionToGameObject != null)
            {
                EditorGUILayout.LabelField("Position To GameObject Dictionary:");
                foreach (var pair in mapManager.PositionToGameObject)
                {
                    //EditorGUILayout.LabelField($"Key: {pair.Key}, Value: {pair.Value.name}");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(pair.Key.ToString());
                    GUILayout.Label(pair.Value.name);
                    GUILayout.EndHorizontal();
                }
            }
        }
    */
}