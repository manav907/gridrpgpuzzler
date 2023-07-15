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
    public CharacterAnimationState currentState;
    public IEnumerator setAnimationAndWaitForIt(CharacterAnimationState state, bool wait = true)
    {
        currentState = state;
        refreshCharacterAnimation();
        if (wait && !UserDataManager.skipAnimations)
            yield return StartCoroutine(waitForAnimation(state));

    }
    public IEnumerator waitForAnimation(CharacterAnimationState state)
    {

        //Debug.Log("Waiting for animation " + state.ToString());
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
    public void refreshCharacterAnimation()
    {

        var animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.ToString() != currentState.ToString())
        {
            animator.SetTrigger(currentState.ToString());
            float duration = animatorStateInfo.length;
            if (true)
            {
                Debug.Log(gameObject.name +
                "\n" + " State Info: " + currentState.ToString() + " " + animatorStateInfo.length + " " + animatorStateInfo.speed + " " + animatorStateInfo.speed +
                "\n" + " Clip Info: " + animatorClipInfo.name + " " + animatorClipInfo.length + " " + animatorClipInfo.frameRate + " " + animatorClipInfo.apparentSpeed);
            }
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
