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
    //public CharacterAnimationState currentState;
    public IEnumerator setAnimationAndWaitForIt(CharacterAnimationState state, bool wait = true)
    {
        var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName(state.ToString()))
        {
            //Debug.Log("not Changing");
        }
        else
        {
            animator.SetTrigger(state.ToString());
            if (true)
            {
                var animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
                Debug.Log(gameObject.name +
                "\n" + " State Info: " + state.ToString() + " " + "Current State Matches?: " + animatorStateInfo.IsName(state.ToString()) +
                "\n" + " Clip Info: " + animatorClipInfo.name);
            }            
        }
        //if (wait && !UserDataManager.skipAnimations)
        if (wait)
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
