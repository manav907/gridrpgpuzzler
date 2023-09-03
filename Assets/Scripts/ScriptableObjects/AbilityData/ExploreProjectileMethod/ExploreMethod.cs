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
