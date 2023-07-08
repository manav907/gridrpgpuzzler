using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New CharacterAbility")]
public class LadderCollapseFunction : ScriptableObject
{
    public string Name;
    public TypeOfAction primaryUseForAction;
    public int ActionPointCost = 1;
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
[System.Serializable]
public class ActionEffectParams
{
    public TypeOfAction typeOfAction;
    //public CharacterAnimationState setUpActionAnimation;
    public CharacterAnimationState doActionTillKeyFrameAnimation;
    public AnimationMovementType animationMovementType;
    //public CharacterAnimationState windupActionAnimation;
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
