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
    public float spriteOffsetY;
    public Sprite[] Walk;
    public Sprite[] Idle;
    public Sprite[] RegularAttack;
    // Add other unique data fields as needed
    [SerializeReference] public AnimatorOverrideController GeneratedAnimatorOverrideController;

    public Dictionary<String, Sprite[]> listOfSprites()
    {
        var thisDict = new Dictionary<String, Sprite[]>();
        thisDict.Add(nameof(Walk), Walk);
        thisDict.Add(nameof(Idle), Idle);
        thisDict.Add(nameof(RegularAttack), RegularAttack);
        return thisDict;
    }


}
public enum CharacterName
{
    SkeletonWithSheild,
    PinkHairGuy,
    RedPriestessGirl,
    GreenRedDude,
    OrcBarbarianGreen
}
