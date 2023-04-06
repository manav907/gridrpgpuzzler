using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

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


    public AnimatorOverrideController GetanimatorOverrideController(AnimatorController originalController)
    {
        AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(originalController);
        animatorOverrideController.name = this.name + " OverideController";
        List<KeyValuePair<AnimationClip, AnimationClip>> overrideList = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        animatorOverrideController.GetOverrides(overrideList);
        List<KeyValuePair<AnimationClip, AnimationClip>> combinedList = matchOverridelist(overrideList);
        animatorOverrideController.ApplyOverrides(combinedList);
        return animatorOverrideController;
    }
    List<KeyValuePair<AnimationClip, AnimationClip>> matchOverridelist(List<KeyValuePair<AnimationClip, AnimationClip>> overrideList)
    {
        var OriginalNametoNewClip = GetAnimationClips();
        List<KeyValuePair<AnimationClip, AnimationClip>> combinedList = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var thisPair in overrideList)
        {
            string thisName = thisPair.Key.name;
            var thisKeyPair = new KeyValuePair<AnimationClip, AnimationClip>(thisPair.Key, OriginalNametoNewClip[thisName]);
            combinedList.Add(thisKeyPair);
        }
        return combinedList;
    }
    Dictionary<String, Sprite[]> listOfSprites()
    {
        var thisDict = new Dictionary<String, Sprite[]>();
        thisDict.Add(nameof(Walk), Walk);
        thisDict.Add(nameof(Idle), Idle);
        return thisDict;
    }
    [SerializeField] Sprite[] Walk;
    [SerializeField] Sprite[] Idle;
    Dictionary<String, AnimationClip> GetAnimationClips()
    {
        var thisDict = new Dictionary<string, AnimationClip>();
        foreach (var thisPair in listOfSprites())
        {
            //getting data
            var animationClip = CreateAnimation(thisPair.Value);//Getting Clip
            animationClip.name = thisPair.Key + "-" + characterName;//setting name for Animation Clip This Help you know which clips
            thisDict.Add(thisPair.Key, animationClip);
            //Save the animation clip
            SaveClip(animationClip, false);
        }
        return thisDict;
        void SaveClip(AnimationClip animationClip, bool SaveYN)
        {
            if (SaveYN)
            {
                AssetDatabase.CreateAsset(animationClip, "Assets/Scripts/ScriptableObjects/CharacterData/AnimationClips/" + animationClip.name + ".anim");
                AssetDatabase.SaveAssets();
            }
        }
    }
    AnimationClip CreateAnimation(Sprite[] frames)
    {
        AnimationClip clip = new AnimationClip();
        clip.name = "Unassigned";

        // Set up the animation clip
        clip.frameRate = frames.Length; // The animation frame rate
        //clip.wrapMode = WrapMode.Loop;// The animation wrap mode
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
        //AnimationClipSettings Being Created
        AnimationClipSettings animationClipSettings = new AnimationClipSettings();
        animationClipSettings.loopTime = true;
        animationClipSettings.stopTime = 1f;
        //AnimationClipSettings Being Set
        AnimationUtility.SetAnimationClipSettings(clip, animationClipSettings);
        return clip;
    }
}

