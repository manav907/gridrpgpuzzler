using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerScript : MonoBehaviour
{
    public void setVariables(CharacterAnimationData characterAnimationData)
    {
        //Debug.Log(characterAnimationData);

        animator.runtimeAnimatorController = characterAnimationData.GeneratedAnimatorOverrideController;
        spriteHolder.position = new Vector3(spriteHolder.position.x, spriteHolder.position.y + characterAnimationData.spriteOffsetY, spriteHolder.position.z);
    }
    [SerializeField] Animator animator;
    [SerializeField] Transform spriteHolder;
    public bool checkifCharacterTurn()
    {
        return TurnManager.thisCharacter == this.gameObject;
    }
    public void ToggleCharacterTurnAnimation()
    {
        if (checkifCharacterTurn())
        {
            setCharacterAnimationAndReturnLength(CharacterAnimationState.Walk);
        }
        else
        {
            setCharacterAnimationAndReturnLength(CharacterAnimationState.Idle);
        }
    }
    public CharacterAnimationState currentState;
    public float setCharacterAnimationAndReturnLength(CharacterAnimationState state)
    {

        currentState = state;
        refreshCharacterAnimation();
        return animator.GetCurrentAnimatorStateInfo(0).length;
    }
    public void refreshCharacterAnimation()
    {
        animator.SetTrigger(currentState.ToString());
        Debug.Log(this.gameObject.name + "is now " + currentState.ToString());
    }
}
public enum CharacterAnimationState
{
    Idle,
    Walk,
    RegularAttack,
}
public enum AnimationMovementType
{
    NoMovement,
    ToPoint,
    InDirection,
    ToPointAndReturn
}
public enum AnimationLoopType
{
    forEachAction,
    UntilActionComplete
}
