using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : ScriptableObject
{
    public int InstanceID;
    public string characterName = "GenericCharacter";
    //public CharacterName NameEnum;
    [SerializeReference] public CharacterAnimationData characterAnimationData;
    public bool isPlayerCharacter = true;
    public string Faction;
    //Stats
    public int health = 5;
    public int attackDamage = 2;
    public int speedValue = 3;
    public int rangeOfVision = 2;
    public int maxStamina = 1;
    public int maxFocusPoints = 2;
    public List<GroundFloorType> canWalkOn;
    public List<AbilityData> listOfAbilities;
    public void ReplaceDataWithPreset(CharacterData characterData)
    {
        if (characterData == null)
        {
            Debug.Log("Data was Null");
            return;
        }
        InstanceID = characterData.InstanceID;
        characterName = characterData.characterName;
        isPlayerCharacter = characterData.isPlayerCharacter;
        health = characterData.health;
        attackDamage = characterData.attackDamage;
        speedValue = characterData.speedValue;
        rangeOfVision = characterData.rangeOfVision;
        canWalkOn = new List<GroundFloorType>(characterData.canWalkOn);
        maxStamina = characterData.maxStamina;
        maxFocusPoints = characterData.maxFocusPoints;
        listOfAbilities = characterData.listOfAbilities;
        Faction = characterData.Faction;
        characterAnimationData = characterData.characterAnimationData;
    }
}
