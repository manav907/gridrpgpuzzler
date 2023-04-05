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



    [SerializeField] AnimatorOverrideController animatorOverrideController;
    public void GenerateFields()
    {
        List<KeyValuePair<AnimationClip, AnimationClip>> overrideList = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        animatorOverrideController.GetOverrides(overrideList);
        foreach (var overridePair in overrideList)
        {
            Debug.Log("Original clip: " + overridePair.Key.name + ", Override clip: " + overridePair.Value.name);
        }
    }
    public Sprite[] frames;
    //[MenuItem("Assets/Create/Animation Clip")]

    public AnimationClip CreateAnimation()
    {
        AnimationClip clip = new AnimationClip();
        clip.name = "Rapidash-Walk";

        // Set up the animation clip
        clip.frameRate = 10; // The animation frame rate
        //clip.wrapMode = WrapMode.ClampForever; // The animation wrap mode
        clip.wrapMode = WrapMode.Loop;
        // Create a curve for the sprites
        ObjectReferenceKeyframe[] spriteFrames = new ObjectReferenceKeyframe[frames.Length];
        for (int i = 0; i < frames.Length; i++)
        {
            spriteFrames[i] = new ObjectReferenceKeyframe();
            spriteFrames[i].time = i / clip.frameRate;
            spriteFrames[i].value = frames[i];
            //Debug.Log(frames[i]);
        }

        // Add the curve to the animation clip
        AnimationUtility.SetObjectReferenceCurve(clip, EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"), spriteFrames);
        //AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(clip);
        AnimationClipSettings animationClipSettings = new AnimationClipSettings();
        animationClipSettings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, animationClipSettings);

        // Save the animation clip
        //AssetDatabase.CreateAsset(clip, "Assets/Scripts/ScriptableObjects/CharacterData/AnimationClips/NewAnimationClip.anim");
        //AssetDatabase.SaveAssets();
        return clip;
    }
}

