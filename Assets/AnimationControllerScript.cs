using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        return 1f;
    }
    public IEnumerator setAnimationAndWaitForPreviousToEnd(CharacterAnimationState state)
    {
        currentState = state;
        refreshCharacterAnimation();
        yield return new WaitUntil(() =>
    {
        var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return animatorStateInfo.normalizedTime >= 1;
    });
        //yield return null;
    }
    public void refreshCharacterAnimation()
    {
        animator.SetTrigger(currentState.ToString());
        var animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //float duration = animatorClipInfo.length;
        float duration = animatorStateInfo.length;
        if (currentState == CharacterAnimationState.RegularAttack)
        {
            Debug.Log(this.gameObject.name + " " +
            "\n" + " State Info: " + currentState.ToString() + animatorStateInfo.length + " " + animatorStateInfo.speed + " " + animatorStateInfo.speed +
            "\n" + " Clip Info: " + animatorClipInfo.name + " " + animatorClipInfo.length + " " + animatorClipInfo.frameRate + " " + animatorClipInfo.apparentSpeed);
        }
        //animator.ResetTrigger(currentState.ToString());
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
