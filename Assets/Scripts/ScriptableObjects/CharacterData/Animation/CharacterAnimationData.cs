using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterAnimationData", menuName = "CharacterAnimationData")]
public class CharacterAnimationData : ScriptableObject
{

    [Header("Animation Stuff")]
    //Sprite Stats
    public float spriteOffsetX;
    public float spriteOffsetY;
    public List<AnimationClipData> animations;
    // Add other unique data fields as needed
    [SerializeReference] public AnimatorOverrideController GeneratedAnimatorOverrideController;

}
public enum CharacterName
{
    SkeletonWithSheild,
    PinkHairGuy,
    RedPriestessGirl,
    GreenRedDude,
    OrcBarbarianGreen
}
[Serializable]
public class AnimationClipData
{
    public string stateName;
    public CharacterAnimationState stateEnum;
    public Sprite[] frames;
    public bool isLooping = true;
    public float speed = 1;
}
