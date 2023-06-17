using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerScript : MonoBehaviour
{
    public void setVariables(CharacterAnimationData characterAnimationData)
    {
        Debug.Log(characterAnimationData);
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
            setCharacterAnimation(CharacterAnimationState.Walk);
        }
        else
        {
            setCharacterAnimation(CharacterAnimationState.Idle);
        }
    }
    public CharacterAnimationState currentState;
    public void setCharacterAnimation(CharacterAnimationState state)
    {

        currentState = state;
        refreshCharacterAnimation();
    }
    public void refreshCharacterAnimation()
    {
        string stateString = currentState.ToString();
        //Debug.Log(this.gameObject.name + "is now " + stateString);
        animator.SetTrigger(stateString);
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
