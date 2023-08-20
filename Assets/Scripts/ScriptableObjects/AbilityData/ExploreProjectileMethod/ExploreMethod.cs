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
    public AoeParams AreaOfAbility;
    public ExploreParams ProjectilesFired;
}
[Serializable]
public class ExploreParams
{
    [Range(0, 5)]
    public int ExploreRangeMax = 3;
    public ExpansionType expansionType;
    public AoeParams AffectsArea;
    public TypeOfAction Effect;
    public List<ValidTargets> AffectTargets = new List<ValidTargets> { ValidTargets.Enemies };
    public List<ValidTargets> StoppedByTargets = new List<ValidTargets> { ValidTargets.SolidObstruction };
}
[Serializable]
public class AoeParams
{
    [Range(0, 5)]
    public int ExploreRangeMax = 3;
    public DirectionalUseParams TypeOfRange;
    public List<ValidTargets> AffectTargets = new List<ValidTargets> { ValidTargets.Enemies };
}
public enum DirectionalUseParams
{
    Omni,//Star
    Cardinal,//Cross
    Ordinal,//Diagonals
    Taxi,

}
public enum ExpansionType
{
    Taxi,
    CollideableProjectile,//Like a Fireball
    PericingProjectile,//Like a Lazer
    LobbedProjectile,//Like a Grenade
}
