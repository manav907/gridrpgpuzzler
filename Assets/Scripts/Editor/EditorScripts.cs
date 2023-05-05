using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;

[CustomEditor(typeof(DataManager))]
public class DataManagerEditor : Editor
{
    Vector3Int checkAtPos;
    CharacterData characterData;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);
        DataManager dataManager = target as DataManager;
        var levelDataSO = dataManager.EditLevel;
        if (levelDataSO.posToCharacterData != null && Application.isPlaying)
        {
            EditorGUILayout.LabelField("Position To Character Data Dictionary:");
            foreach (var pair in levelDataSO.posToCharacterData)
            {
                levelDataSO.loadDataifNotLoaded();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Edit "))
                {
                    Selection.activeObject = pair.Value;
                }
                GUILayout.Label(pair.Key.ToString());
                GUILayout.Label(pair.Value.name + pair.Value.InstanceID);
                if (GUILayout.Button("Remove "))
                {
                    levelDataSO.posToCharacterData.Remove(pair.Key);
                    break;
                }
                if (GUILayout.Button("Get Current Data"))
                {
                    dataManager.mapManager.cellDataDir[pair.Key].characterAtCell.GetComponent<CharacterControllerScript>().saveCharacterDataToCSO();
                }
                GUILayout.EndHorizontal();
            }


            EditorGUILayout.LabelField("ADD Data?:");
            GUILayout.BeginHorizontal();
            checkAtPos = EditorGUILayout.Vector3IntField(GUIContent.none, checkAtPos);
            characterData = (CharacterData)EditorGUILayout.ObjectField(GUIContent.none, characterData, typeof(CharacterData), false);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Try Add This Data To Dictionary"))
            {
                if (levelDataSO.posToCharacterData.ContainsKey(checkAtPos) || characterData == null)
                {
                    Debug.Log("Cannot Add Key as \n" + checkAtPos + characterData);
                    return;
                }
                levelDataSO.posToCharacterData.Add(checkAtPos, characterData);
            }
            GUILayout.Label("Manage Level Data File");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("SaveData"))
            {
                levelDataSO.SaveData();
            }
            if (GUILayout.Button("LoadData"))
            {
                levelDataSO.LoadData();
            }
            if (GUILayout.Button("Clear Data(no Save)"))
            {
                levelDataSO.ClearData();
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("tryDiagonose"))
            {
                levelDataSO.tryDiagonose();
            }
        }


    }
}

[CustomEditor(typeof(LevelDataSO))]
public class LevelDataSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);
        LevelDataSO levelDataSO = target as LevelDataSO;

        if (levelDataSO.posToCharacterData != null)
        {
            levelDataSO.loadDataifNotLoaded();
            EditorGUILayout.LabelField("Position To GameObject Dictionary:");
            foreach (var pair in levelDataSO.posToCharacterData)
            {
                //EditorGUILayout.LabelField($"Key: {pair.Key}, Value: {pair.Value.name}");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Edit "))
                {
                    Selection.activeObject = pair.Value;
                }
                GUILayout.Label(pair.Key.ToString());
                GUILayout.Label(pair.Value.name + pair.Value.InstanceID);
                if (GUILayout.Button("Remove "))
                {
                    levelDataSO.posToCharacterData.Remove(pair.Key);
                    break;
                }
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("CheckAtPos"), GUIContent.none, true);
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("importThisCharData"), GUIContent.none, true);
        serializedObject.ApplyModifiedProperties();


        if (GUILayout.Button("Add To Dictionary"))
        {
            levelDataSO.addToDictionary();
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("Manage Level Data File");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("SaveData"))
        {
            levelDataSO.SaveData();
        }
        if (GUILayout.Button("LoadData"))
        {
            levelDataSO.LoadData();
        }
        if (GUILayout.Button("Clear Data(no Save)"))
        {
            levelDataSO.ClearData();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("tryDiagonose"))
        {
            levelDataSO.tryDiagonose();
        }
    }
}
[CustomEditor(typeof(CharacterData))]
public class CharacterDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Rename abilityNames"))
        {
            // Modify data in the scriptable object
            CharacterData characterData = target as CharacterData;
            foreach (Ability ability in characterData.listOfAbility)
            {
                //Debug.Log(ability.abilityString + " to " + ability.abilityName.ToString());
                ability.abilityString = ability.abilityName.ToString();
            }
        }
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("ListOfAbility"), true);
    }
}
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

            string newNameForFile = characterAnimationData.nameEnum.ToString() + " CharacterAnimationData";
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
            AssetDatabase.CreateAsset(GetanimatorOverrideController(), "Assets/Resources/AnimationClips/" + characterAnimationData.nameEnum.ToString() + ".overrideController");
            AssetDatabase.SaveAssets();
            AnimatorOverrideController GetanimatorOverrideController()
            {
                string sourceAnimatorControllerPath = "Assets/Prefabs/Character.prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath(sourceAnimatorControllerPath, typeof(GameObject)) as GameObject;
                Animator animator = prefab.GetComponentInChildren<Animator>();
                AnimatorController originalController = animator.runtimeAnimatorController as AnimatorController;
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
                        animationClip.name = thisPair.Key + "-" + characterAnimationData.nameEnum.ToString();//setting name for Animation Clip This Help you know which clips
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
                        AnimationClipSettings animationClipSettings = new AnimationClipSettings();
                        animationClipSettings.loopTime = true;
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
[CustomEditor(typeof(ReticalManager))]
public class ReticalManagerEditor : Editor
{
    /*
    public override void OnInspectorGUI()
    {
        ReticalManager reticalManager = target as ReticalManager;
        DrawDefaultInspector();
        //Creating a Syle
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;
        //Drawing said Style
        EditorGUILayout.LabelField("Inspector Stuff", style);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Draw Retical at Point"))
        {
            reticalManager.setReticalToFromPoint();
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fromPoint"), true);
        //serializedObject.ApplyModifiedProperties();
        GUILayout.EndHorizontal();


        if (GUILayout.Button("do Action"))
        {
            reticalManager.doOnClick();
        }

    }
    */
}