using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class CharacterData : ScriptableObject
{
    public int InstanceID;
    public string characterName = "GenericCharacter";
    public CharacterName NameEnum;
    public bool isPlayerCharacter = true;
    //Stats
    public int health = 5;
    public int attackDamage = 2;
    public int speedValue = 3;
    public int rangeOfVision = 2;
    //Custom Data Types
    public List<GroundFloorType> canWalkOn;
    public List<CompundAbility> abilities;

    public CharacterData()
    {

    }
    public CharacterData(CharacterData characterData)
    {
        InstanceID = characterData.InstanceID;
        characterName = characterData.characterName;
        NameEnum = characterData.NameEnum;
        isPlayerCharacter = characterData.isPlayerCharacter;
        health = characterData.health;
        attackDamage = characterData.attackDamage;
        speedValue = characterData.speedValue;
        rangeOfVision = characterData.rangeOfVision;
        canWalkOn = new List<GroundFloorType>(characterData.canWalkOn);
        abilities = new List<CompundAbility>(characterData.abilities);
    }
    public void ReplaceDataWithPreset(CharacterData characterData)
    {
        if (characterData == null)
        {
            Debug.Log("Data was Null");
            return;
        }
        InstanceID = characterData.InstanceID;
        characterName = characterData.characterName;
        NameEnum = characterData.NameEnum;
        isPlayerCharacter = characterData.isPlayerCharacter;
        health = characterData.health;
        attackDamage = characterData.attackDamage;
        speedValue = characterData.speedValue;
        rangeOfVision = characterData.rangeOfVision;
        canWalkOn = new List<GroundFloorType>(characterData.canWalkOn);
        abilities = new List<CompundAbility>(characterData.abilities);
    }

}