using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    Empty,
    AnyFaction,
    Enemies,
    Allies,
    Neutral,
    AnyValidOrInValid
}
