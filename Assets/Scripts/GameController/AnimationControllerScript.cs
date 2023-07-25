using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimationControllerScript : MonoBehaviour
{
    public void setVariables(CharacterAnimationData characterAnimationData)
    {
        animator.runtimeAnimatorController = characterAnimationData.GeneratedAnimatorOverrideController;
        spriteHolder.position = new Vector3(spriteHolder.position.x + characterAnimationData.spriteOffsetX, spriteHolder.position.y + characterAnimationData.spriteOffsetY, spriteHolder.position.z);
    }
    [SerializeField] Animator animator;
    [SerializeField] Transform spriteHolder;
    public bool checkifCharacterTurn()
    {
        return TurnManager.thisCharacter == this.gameObject;
    }
    CharacterAnimationState lastState;
    public IEnumerator trySetNewAnimation(CharacterAnimationState state)
    {
        var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        animator.ResetTrigger(lastState.ToString());
        if (UserDataManager.skipAnimations)
        {
            StartCoroutine(setAnimation(state));
        }
        else
        {
            yield return StartCoroutine(setAnimation(state));
        }
        IEnumerator setAnimation(CharacterAnimationState state)
        {
            lastState = state;
            animator.SetTrigger(state.ToString());
            yield return StartCoroutine(waitForAnimation(state));

            /* var animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            Debug.Log(gameObject.name +
            "\n" + " State Info: " + state.ToString() + " " + "Current State Matches?: " + animatorStateInfo.IsName(state.ToString()) +
            "\n" + " Clip Info: " + animatorClipInfo.name); */
        }
    }

    public IEnumerator waitForAnimation(CharacterAnimationState state)
    {
        yield return new WaitUntil(() =>
        {
            return isTheCorrectAnimation();
        });
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        bool isTheCorrectAnimation()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(state.ToString()))
                return true;
            //Debug.Log("Waiting for the Wrong animation");
            return false;
        }
    }
}
public enum CharacterAnimationState
{
    Idle,
    Walk,
    RegularAttack,
    SpecialAttack
}
public enum AnimationMovementType
{
    NudgeToPoint,
    NoMovement,
    WalkToPoint,

}
public enum AnimationLoopType
{
    forEachPoint,
    forAction
}
