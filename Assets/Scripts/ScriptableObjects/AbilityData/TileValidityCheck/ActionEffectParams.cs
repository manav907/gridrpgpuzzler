using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New ActionEffectParams", menuName = "ActionEffectParams")]
public class ActionEffectParams : ScriptableObject
{
    public TypeOfAction typeOfAction;
    public ValidTargets OnlyApplyOn;
    public CharacterAnimationState doActionTillKeyFrameAnimation;
    public AnimationMovementType animationMovementType;
    public AnimationLoopType loopType;
}
