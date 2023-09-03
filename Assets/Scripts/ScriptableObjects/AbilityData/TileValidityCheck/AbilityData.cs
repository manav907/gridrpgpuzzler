using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[CreateAssetMenu(fileName = "New AbilityData", menuName = "AbilityData")]
public class AbilityData : ScriptableObject
{
    public string userFriendlyName;
    public TypeOfAction Primaryuse;
    public AreaGenerationParams rangeOfAbility;
    public List<AreaGenerationParams> ValidTileData;
    public List<ActionEffectParams> ApplyEffects;
}

[Serializable]
public class AreaGenerationParams
{
    public string name;
    public int minpoints = 1;
    //public int MaxPoints = 1;
    [Range(-3, 3)]
    public int adjustAtPointWithDirection = 1;
    [Range(1, 3)]
    public int rangeOfArea = 1;
    public TileValidityParms tileValidityParms;
    public AoeStyle aoeStyle;
}
public enum TargetType
{
    AnyValid,
    FirstValid,
    LastValid,
}
public enum ValidTargets
{
    AnyValidOrInValid = 0,
    Empty = 1,
    AnyFaction = 2,
    Enemies = 3,
    Allies = 4,
    SolidObstruction = 5,
    Neutral,
}
public enum AoeStyle
{
    Taxi = 4,
    Square = 5,
    SSweep = 2,
    Omni,//Star
    Cardinal,//Cross
    Ordinal,//Diagonals
}
//Defining Global NameSapce
public enum GroundFloorType
{
    Invalid = -1,
    NotSet = 0,
    Normal = 1,
    Water = 2,
    Fire = 3,
    StructuresNonWalkable = 4
};