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
    public void SetVariablesForCSS(CharacterControllerScript characterControllerScript)
    {
        characterControllerScript.CharacterDataSO = this;
        setVariables();
        characterControllerScript.CheckIfCharacterIsDead();
        void setVariables()
        {
            characterControllerScript.characterName = characterName;
            characterControllerScript.isPlayerCharacter = isPlayerCharacter;
            characterControllerScript.health = health;
            characterControllerScript.attackDamage = attackDamage;
            characterControllerScript.speedValue = speedValue;
            characterControllerScript.rangeOfVision = rangeOfVision;
            characterControllerScript.Faction = Faction;
            //ListStuff
            characterControllerScript.canWalkOn = canWalkOn;
            //Rewordk This
            characterControllerScript.maxStamina = maxStamina;
            characterControllerScript.maxFocusPoints = maxFocusPoints;
            characterControllerScript.currentFocusPoints = maxFocusPoints;
            characterControllerScript.listOfAbilities = listOfAbilities;
            //Setting Data

            //Game Event Regersery
            GameEvents.current.AddCharacter(isPlayerCharacter);
            characterControllerScript.animationControllerScript.SetVariables(characterAnimationData);
        }
    }
}
