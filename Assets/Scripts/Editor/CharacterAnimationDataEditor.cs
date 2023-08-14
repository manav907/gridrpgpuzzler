using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using System;

[CustomEditor(typeof(CharacterAnimationData))]
public class CharacterAnimationDataEditor : Editor
{
    //Dictionary<string, bool> isChecked;
    float sizeOfView = 0f;
    Vector2 scroll = new Vector2();
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CharacterAnimationData characterAnimationData = target as CharacterAnimationData;
        GUILayout.Label("Preview Animation Here");
        sizeOfView = EditorGUILayout.FloatField(sizeOfView);
        if (sizeOfView != 0)
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (var pair in characterAnimationData.animations)
            {
                GUILayout.Label(pair.stateName);
                GUILayout.BeginHorizontal();
                foreach (var sprite in pair.frames)
                {
                    var texture = AssetPreview.GetAssetPreview(sprite);
                    //                    GUILayout.Box(texture, GUILayout.ExpandWidth(false), GUILayout.Height(sizeOfView), GUILayout.Width(sizeOfView));
                    GUILayout.Box(texture, GUILayout.ExpandWidth(false));
                }
                GUILayout.EndHorizontal();

            }
            EditorGUILayout.EndScrollView();
        }
        GUILayout.Label("Make Animation Here");
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
                    if (OriginalNametoNewClip.ContainsKey(thisName))
                    {
                        var thisKeyPair = new KeyValuePair<AnimationClip, AnimationClip>(thisPair.Key, OriginalNametoNewClip[thisName]);
                        combinedList.Add(thisKeyPair);
                    }
                    else
                    {
                        Debug.LogWarning(thisName + " Was not in the Dictionary");
                    }
                }
                return combinedList;
                Dictionary<string, AnimationClip> GetAnimationClips()
                {
                    var thisDict = new Dictionary<string, AnimationClip>();
                    foreach (var thisPair in characterAnimationData.animations)
                    {
                        thisPair.stateName = thisPair.stateEnum.ToString();
                        //getting data
                        var animationClip = CreateAnimation(thisPair.frames, thisPair);//Getting Clip
                        animationClip.name = thisPair.stateName + "-" + characterAnimationData.ToString();//setting name for Animation Clip This Help you know which clips                        
                        thisDict.Add(thisPair.stateName, animationClip);
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
                    AnimationClip CreateAnimation(Sprite[] frames, AnimationClipData animationClipData)
                    {
                        string nameOfAnimation = animationClipData.stateName;
                        //AnimationClip originalClip = AssetDatabase.LoadAssetAtPath("Assets/Animation/" + nameOfAnimation + ".anim", typeof(AnimationClip)) as AnimationClip;
                        Debug.Log(nameOfAnimation);

                        AnimationClip clip = new AnimationClip();
                        clip.name = characterAnimationData.name + " " + nameOfAnimation + " Animation";

                        // Set up the animation clip
                        clip.frameRate = frames.Length; // The animation frame rate

                        ObjectReferenceKeyframe[] spriteFrames = new ObjectReferenceKeyframe[frames.Length];
                        for (int i = 0; i < frames.Length; i++)
                        {
                            spriteFrames[i] = new ObjectReferenceKeyframe();
                            float speedMultipler = ((clip.frameRate) * (animationClipData.speed));
                            if (speedMultipler <= 0)
                            {
                                Debug.LogError("Incorrect Speed Multipler of " + speedMultipler + " for " + animationClipData.stateName + " Will change it to 1f");
                                speedMultipler = 1f;
                                animationClipData.speed = speedMultipler;
                            }
                            spriteFrames[i].time = i / speedMultipler;
                            spriteFrames[i].value = frames[i];
                            //Debug.Log(frames[i]);
                        }

                        // Add the curve to the animation clip
                        AnimationUtility.SetObjectReferenceCurve(clip, EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"), spriteFrames);
                        //AnimationClipSettings Being Created
                        AnimationClipSettings animationClipSettings = new AnimationClipSettings
                        {
                            //animationClipSettings.loopTime = originalClip.isLooping;//Mess with this to refine loops
                            loopTime = animationClipData.isLooping,//Mess with this to refine loops
                            stopTime = clip.length
                        };
                        //animationClipSettings.stopTime = 1f;
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