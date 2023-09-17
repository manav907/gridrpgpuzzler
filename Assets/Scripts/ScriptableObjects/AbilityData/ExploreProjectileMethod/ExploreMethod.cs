using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
[CreateAssetMenu(fileName = "New ExploreMethod", menuName = "ExploreMethod")]
public class ExploreMethod : ScriptableObject
{
    public string userFriendlyName;
    public TypeOfAction Primaryuse;
    public AoeParams AreaOfAbility;//Valid Selectable Tiles will be filtered by ExploreParams
    public AoeParams HelpUiArea;//Shown as Selectable Area
    public ExploreParams ProjectilesFired;
}
public class AbilityCheckData
{
    [Range(0, 5)]
    public int ExploreRangeMax = 3;
    public AoeParams StartSeachNodes;//this makes start seach nodes for projectiles
    public AoeParams CheckAreaInNode;//This Creates and Checks for Conditions in an Area
    public bool isPearcing;//This Checks wether or not the Node Check should be canceled upon Inconnect Collision.
    public bool calculateNewDirection;//this calculates new direction from a given origin point
}
[Serializable]
public class ExploreParams
{
    [Range(0, 5)]
    public int ExploreRangeMax = 3;
    public AoeParams AffectsArea;
    public bool perices = false;
    public List<ValidTargets> AffectTargets = new List<ValidTargets> { ValidTargets.Enemies };
    public List<ValidTargets> StoppedByTargets = new List<ValidTargets> { ValidTargets.SolidObstruction };
}
public enum TargetStatus
{
    Valid,
    ConditionallyValid,
    Invalid,
}
[Serializable]
public class AoeParams
{
    [Range(0, 5)]
    public int ExploreRangeMax = 3;
    public AoeStyle TypeOfRange;
    public List<ValidTargets> AffectTargets = new List<ValidTargets> { ValidTargets.Enemies };
}
