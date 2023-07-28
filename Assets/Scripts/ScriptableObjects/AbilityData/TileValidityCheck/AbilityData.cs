using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New AbilityData", menuName = "AbilityData")]
public class AbilityData : ScriptableObject
{
    public TypeOfAction Primaryuse;
    public int costOfaction;
    public TileToEffectPair AreaOfAbility;
    public List<TileToEffectPair> ValidTileData;
    public List<ActionEffectParams> ApplyEffects;
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


public enum LadderCollapseFunctionEnums
{
    setDataWithID,
    doActionWithID,
    SetDataUsingTherorticalPosAtArrayIndex,
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