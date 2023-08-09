using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimationControllerScript : MonoBehaviour
{
    public void SetVariables(CharacterAnimationData characterAnimationData)
    {
        characterAnimator.runtimeAnimatorController = characterAnimationData.GeneratedAnimatorOverrideController;
        spriteHolder.position = new Vector3(spriteHolder.position.x + characterAnimationData.spriteOffsetX, spriteHolder.position.y + characterAnimationData.spriteOffsetY, spriteHolder.position.z);
    }
    public Animator characterAnimator;
    [SerializeField] Transform spriteHolder;
    CharacterAnimationState lastCharacterAnimationState;
    public IEnumerator TrySetNewAnimation(CharacterAnimationState state)
    {
        //var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        characterAnimator.ResetTrigger(lastCharacterAnimationState.ToString());
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
            lastCharacterAnimationState = state;
            characterAnimator.SetTrigger(state.ToString());
            yield return StartCoroutine(WaitForAnimation(state.ToString()));

            /* var animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            Debug.Log(gameObject.name +
            "\n" + " State Info: " + state.ToString() + " " + "Current State Matches?: " + animatorStateInfo.IsName(state.ToString()) +
            "\n" + " Clip Info: " + animatorClipInfo.name); */
        }
    }

    public IEnumerator WaitForAnimation(string state)
    {
        yield return new WaitUntil(() =>
        {
            return isTheCorrectAnimation();
        });
        yield return new WaitUntil(() => characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        bool isTheCorrectAnimation()
        {
            if (characterAnimator.GetCurrentAnimatorStateInfo(0).IsName(state))
                return true;
            //Debug.Log("Waiting for the Wrong animation");
            return false;
        }
    }
    public Animator statusAnimator;
    public StatusEffect lastStatusEffect;
    public IEnumerator SetStatusEffect(StatusEffect statusEffect)
    {
        statusAnimator.ResetTrigger(lastStatusEffect.ToString());
        if (UserDataManager.skipAnimations)
        {
            StartCoroutine(setAnimation(statusEffect));
        }
        else
        {
            yield return StartCoroutine(setAnimation(statusEffect));
        }
        IEnumerator setAnimation(StatusEffect statusEffect)
        {
            lastStatusEffect = statusEffect;
            statusAnimator.SetTrigger(statusEffect.ToString());
            yield return StartCoroutine(WaitForAnimation(statusEffect.ToString()));
        }
    }
}
public enum StatusEffect
{
    Normal,
    Stun,
}
public enum CharacterAnimationState
{
    Idle,
    Walk,
    RegularAttack,
    SpecialAttack
}
/* public enum AnimationMovementType
{
    NudgeToPoint,
    NoMovement,
    WalkToPoint,

} */
public enum AnimationLoopType
{
    forEachPoint,
    forAction
}
