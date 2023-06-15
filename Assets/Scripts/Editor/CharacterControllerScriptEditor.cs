using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimationControllerScript))]
public class AnimationControllerScriptEditor : Editor
{
    [SerializeField] AnimationControllerScript newCharacterData;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AnimationControllerScript characterData = target as AnimationControllerScript;
        CharacterAnimationState newState = (CharacterAnimationState)EditorGUILayout.EnumPopup(characterData.currentState);
        if (characterData.currentState != newState)
        {
            characterData.currentState = newState;
            characterData.refreshCharacterAnimation();
        }
    }
}