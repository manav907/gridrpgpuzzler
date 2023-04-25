using UnityEditor;
using UnityEngine;
using System.Linq;

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

[CustomEditor(typeof(CharacterData))]
public class CharacterDataEditor : Editor
{
    CharacterData characterData;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Rename abilityNames"))
        {
            // Modify data in the scriptable object

            characterData = target as CharacterData;
            Debug.Log(characterData);
            foreach (Ability ability in characterData.listOfAbility)
            { ability.abilityString = ability.abilityName.ToString(); }
        }
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("ListOfAbility"), true);
    }
}