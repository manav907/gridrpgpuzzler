using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;




[CustomEditor(typeof(CharacterAnimationData))]
public class CharacterAnimationDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CharacterAnimationData characterAnimationData = target as CharacterAnimationData;
        if (GUILayout.Button("Rename FileNames"))
        {
            // Modify data in the scriptable object

            string newNameForFile = characterAnimationData.ToString() + " CharacterAnimationData";
            // Rename the SO asset
            string path = AssetDatabase.GetAssetPath(characterAnimationData);
            string errorMsg = AssetDatabase.RenameAsset(path, newNameForFile);
            if (string.IsNullOrEmpty(errorMsg))
            {
                // Refresh the editor to update the file name
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("Failed to rename asset: " + errorMsg);
            }

        }
        if (GUILayout.Button("Genereate Animation"))
        {
            string assetPath = "Assets/Resources/AnimationClips/" + characterAnimationData.ToString() + ".overrideController";
            AssetDatabase.CreateAsset(GetanimatorOverrideController(), assetPath);
            AssetDatabase.SaveAssets();
            characterAnimationData.GeneratedAnimatorOverrideController = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(assetPath);

            AnimatorOverrideController GetanimatorOverrideController()
            {
                string sourceAnimatorControllerPath = "Assets/Prefabs/Character.prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath(sourceAnimatorControllerPath, typeof(GameObject)) as GameObject;
                Animator animator = prefab.GetComponentInChildren<Animator>();                
                AnimatorController originalController = animator.runtimeAnimatorController as AnimatorController;
                //originalController.animationClips[];
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
                Dictionary<string, AnimationClip> GetAnimationClips()
                {
                    var thisDict = new Dictionary<string, AnimationClip>();
                    foreach (var thisPair in characterAnimationData.listOfSprites())
                    {
                        //getting data
                        var animationClip = CreateAnimation(thisPair.Value);//Getting Clip
                        animationClip.name = thisPair.Key + "-" + characterAnimationData.ToString();//setting name for Animation Clip This Help you know which clips
                        if (thisPair.Key == "RegularAttack")
                        {
                            //animationClip.loop
                        }
                        thisDict.Add(thisPair.Key, animationClip);
                        //Save the animation clip
                        SaveClip(animationClip, true);
                    }
                    return thisDict;
                    void SaveClip(AnimationClip animationClip, bool SaveYN)
                    {
                        if (SaveYN)
                        {
                            AssetDatabase.CreateAsset(animationClip, "Assets/Resources/AnimationClips/" + animationClip.name + ".anim");
                            AssetDatabase.SaveAssets();
                        }
                    }
                    AnimationClip CreateAnimation(Sprite[] frames)
                    {
                        string nameOfAnimation = nameof(frames);
                        //AnimationClip originalClip = AssetDatabase.LoadAssetAtPath("Assets/Resources/AnimationClips/" + nameOfAnimation + ".anim",object) as AnimationClip;
                        AnimationClip clip = new AnimationClip();
                        clip.name = "Unassigned";

                        // Set up the animation clip
                        clip.frameRate = frames.Length; // The animation frame rate
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
                        //AnimationClipSettings originalAnimationClipSetting = 
                        AnimationClipSettings animationClipSettings = new AnimationClipSettings();
                        animationClipSettings.loopTime = true;//Mess with this to refine loops
                        animationClipSettings.stopTime = 1f;
                        //AnimationClipSettings Being Set
                        AnimationUtility.SetAnimationClipSettings(clip, animationClipSettings);
                        return clip;
                    }
                }
            }
        }
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("ListOfAbility"), true);
    }
}