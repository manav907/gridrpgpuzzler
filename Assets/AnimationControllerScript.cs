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
        spriteHolder.position = new Vector3(spriteHolder.position.x + characterAnimationData.spriteOffsetX, spriteHolder.position.y + characterAnimationData.spriteOffsetY, spriteHolder.position.z);
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
            //setCharacterAnimationAndReturnLength(CharacterAnimationState.Walk);
            StartCoroutine(setAnimationAndWaitForIt(CharacterAnimationState.Walk));
        }
        else
        {
            //setCharacterAnimationAndReturnLength(CharacterAnimationState.Idle);
            StartCoroutine(setAnimationAndWaitForIt(CharacterAnimationState.Idle));
        }
    }
    public CharacterAnimationState currentState;
    public IEnumerator setAnimationAndWaitForIt(CharacterAnimationState state, bool wait = true)
    {
        currentState = state;
        refreshCharacterAnimation();
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
    public void refreshCharacterAnimation()
    {
        animator.SetTrigger(currentState.ToString());
        DebugRefresh(false);
        void DebugRefresh(bool DebugData)
        {
            if (DebugData)
            {
                var animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
                var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                //float duration = animatorClipInfo.length;
                float duration = animatorStateInfo.length;
                if (currentState == CharacterAnimationState.RegularAttack)
                {
                    Debug.Log(gameObject.name + " " +
                    "\n" + " State Info: " + currentState.ToString() + animatorStateInfo.length + " " + animatorStateInfo.speed + " " + animatorStateInfo.speed +
                    "\n" + " Clip Info: " + animatorClipInfo.name + " " + animatorClipInfo.length + " " + animatorClipInfo.frameRate + " " + animatorClipInfo.apparentSpeed);

                }
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
