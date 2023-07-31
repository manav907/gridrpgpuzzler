using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New AbilityData", menuName = "AbilityData")]
public class AbilityData : ScriptableObject
{
    public TypeOfAction Primaryuse;
   // public AreaGenerationParams AreaOfAbility;
    public RangeOfActionEnum rangeOfAbility;
    //public AoeStyle typeOfArea;
    public List<AreaGenerationParams> ValidTileData;
    public List<ActionEffectParams> ApplyEffects;
}

[Serializable]
public class AreaGenerationParams
{
    public int minpoints = 1;
    public int MaxPoints = 1;
    public bool sortFromPoint = true;
    public TileValidityParms tileValidityParms;
    public RangeOfActionEnum areaOfEffectRange;
    public AoeStyle aoeStyle;

    public float getRangeOfAction()
    {
        return (float)areaOfEffectRange / 10;
    }

}
[Serializable]
public class ActionEffectParams
{
    public TypeOfAction typeOfAction;
    public ValidTargets OnlyApplyOn;
    public CharacterAnimationState doActionTillKeyFrameAnimation;
    public AnimationMovementType animationMovementType;
    public AnimationLoopType loopType;
}
public enum TargetType
{
    AnyValid,
    FirstValid,
    LastValid,
}
public enum RangeOfActionEnum
{
    r0 = 0,
    r10 = 10,
    r15 = 15,
    r20 = 20,
    r25 = 25,
    r30 = 30
}
public enum ValidTargets
{
    AnyValidOrInValid = 0,
    Empty = 1,
    AnyFaction = 2,
    Enemies = 3,
    Allies = 4,
    Neutral,

}