using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class CharacterData : ScriptableObject
{
    public int InstanceID;
    public string characterName = "GenericCharacter";

    public int health = 5;
    public int rangeOfMove = 1;
    public int rangeOfAttack = 2;
    public int AttackDamage = 2;
    public int speedValue = 3;
    public int rangeOfVision = 5;

    // Add other unique data fields as needed



    public Sprite[] frames;
    //[MenuItem("Assets/Create/Animation Clip")]
    public AnimationClip CreateAnimation()
    {
        AnimationClip clip = new AnimationClip();

        // Set up the animation clip
        //clip.frameRate = 60; // The animation frame rate
        clip.wrapMode = WrapMode.ClampForever; // The animation wrap mode
        // Create a curve for the sprites

        //int TotalFrames = 20;
        //int FrameGap = TotalFrames / frames.Length;
        ObjectReferenceKeyframe[] spriteFrames = new ObjectReferenceKeyframe[frames.Length];
        for (int i = 0; i < frames.Length; i++)
        {
            spriteFrames[i] = new ObjectReferenceKeyframe();
            //[i].time = i / clip.frameRate;
            spriteFrames[i].value = frames[i];
        }

        // Add the curve to the animation clip
        AnimationUtility.SetObjectReferenceCurve(clip, EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"), spriteFrames);

        // Save the animation clip
        //AssetDatabase.CreateAsset(clip, "Assets/Scripts/ScriptableObjects/CharacterData/AnimationClips/NewAnimationClip.anim");
        //AssetDatabase.SaveAssets();
        return clip;
    }
}

