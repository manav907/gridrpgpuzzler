using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterData : ScriptableObject
{
    public int InstanceID;
    public string characterName = "GenericCharacter";
    public Sprite[] spriteSheet;
    public int health = 5;
    public int rangeOfMove = 1;
    public int rangeOfAttack = 2;
    public int AttackDamage = 2;
    public int speedValue = 3;
    public int rangeOfVision = 5;

    // Add other unique data fields as needed
}
