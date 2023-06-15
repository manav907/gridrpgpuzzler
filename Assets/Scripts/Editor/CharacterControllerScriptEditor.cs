using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterControllerScript))]
public class CharacterControllerScriptEditor : Editor
{
    [SerializeField] CharacterControllerScript newCharacterData;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CharacterControllerScript characterData = target as CharacterControllerScript;
        CharacterAnimationState newState = (CharacterAnimationState)EditorGUILayout.EnumPopup(characterData.currentState);
        if (characterData.currentState != newState)
        {
            characterData.currentState = newState;
            characterData.refreshCharacterAnimation();
        }
    }
}