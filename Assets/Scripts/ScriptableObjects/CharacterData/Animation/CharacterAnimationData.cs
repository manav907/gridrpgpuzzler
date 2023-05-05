using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterAnimationData", menuName = "CharacterAnimationData")]
public class CharacterAnimationData : ScriptableObject
{
    [Header("Reffrence Data")]
    public CharacterName nameEnum;
    [Header("Animation Stuff")]
    //Sprite Stats
    public float spriteOffsetY;
    public Sprite[] Walk;
    public Sprite[] Idle;
    // Add other unique data fields as needed
    [SerializeField]
    public AnimatorOverrideController GeneratedAnimatorOverrideController;
   /*  {
        get
        {
            var assetPath = nameEnum.ToString() + ".overrideController";
            AnimatorOverrideController asset = Resources.Load<AnimatorOverrideController>(assetPath);
            //AnimatorOverrideController asset = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(assetPath);
            if (asset == null)
            {
                Debug.Log("Failed to load AnimatorOverrideController asset for " + nameEnum.ToString());
            }
            return asset;
        }
    } */


    public Dictionary<String, Sprite[]> listOfSprites()
    {
        var thisDict = new Dictionary<String, Sprite[]>();
        thisDict.Add(nameof(Walk), Walk);
        thisDict.Add(nameof(Idle), Idle);
        return thisDict;
    }


}
public enum CharacterName
{
    Rapidash,
    ShedNinja
}
