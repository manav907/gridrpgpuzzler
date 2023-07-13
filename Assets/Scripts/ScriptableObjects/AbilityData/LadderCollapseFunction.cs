using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New CharacterAbility")]
public class LadderCollapseFunction : ScriptableObject
{
    public string Name;
    public TypeOfAction primaryUseForAction;
    public List<string> Varirables;
    public SerializableDictionary<LadderCollapseFunctionEnums, string> invokeFunction;
    public List<ActionInputParams> SetDataAtIndex;
    public List<ActionEffectParams> DoActionFromDataAtIndex;
    public LadderCollapseFunction()
    {

    }
    public LadderCollapseFunction(LadderCollapseFunction given)
    {
        this.name = given.name;
        //Rework This
        invokeFunction = (given.invokeFunction);
        SetDataAtIndex = (given.SetDataAtIndex);
        DoActionFromDataAtIndex = (given.DoActionFromDataAtIndex);
    }
}
[Serializable]
public class ActionInputParams
{
    //public TargetType targetType;
    [SerializeField] RangeOfActionEnum rangeOfActionEnum;
    [SerializeField] RangeOfActionEnum magnititudeOfActionEnum;
    public ReticalShapes areaOfEffectType;
    //public OptimalTargetTip optimalTargetTip;
    public bool ignoreValidTargetsCheck = false;
    public ValidTargets validTargets;
    public bool includeSelf;
    public bool updateTheroticalPos = true;
    public ActionInputParams()
    {

    }
    public float getRangeOfAction()
    {
        return (float)rangeOfActionEnum / 10;
    }
    public float getMagnititudeOfAction()
    {
        return (float)magnititudeOfActionEnum / 10;

    }
}
[Serializable]
public class ActionEffectParams
{
    public TypeOfAction typeOfAction;
    public CharacterAnimationState doActionTillKeyFrameAnimation;
    public AnimationMovementType animationMovementType;
    public AnimationLoopType loopType;
}
public class AnimationParameter
{

}
public enum OptimalTargetTip
{
    PrefferEnemies,
    PrefferAllies
}
public enum TargetType
{
    CellTargeted,
    CellNearest,
    VectorFirstValid,
    VectorLastValid,
    VectorOptimal,
    VectorAll
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
    Neutral
}
