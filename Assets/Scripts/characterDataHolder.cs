using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterDataHolder : MonoBehaviour
{
    public int health = 3;
    public int rangeOfMove = 5;
    public int attackRange = 2;
    public int AttackDamage = 2;
    public int speedValue = 3;
    [SerializeField] private TextMesh Heatlh;

    private ButtonManager thisButtonManager;
    private MapManager thisMapManager;
    private TurnManager thisTurnManager;
    public void InitilizeCharacter(GameObject gameController)
    {
        thisButtonManager = gameController.GetComponent<ButtonManager>();
        thisMapManager = gameController.GetComponent<MapManager>();
        thisTurnManager = gameController.GetComponent<TurnManager>();
        UpdateCharacterData();
    }
    List<string> GetCharacterMoveList()
    {
        List<string> defaultMovesAvaliable;
        defaultMovesAvaliable = new List<string>();
        defaultMovesAvaliable.AddRange(new List<string>
        {
            "Move",
            "Attack",
            "FireBall",
            "End Turn"
        });
        return defaultMovesAvaliable;
    }
    public void UpdateCharacterData()
    {
        Heatlh.text = health + "";
        if (health <= 0)
        {
            Debug.Log("I am dying");
            KillCharacter();
            //Destroy(this.gameObject);
        }
    }
    void KillCharacter()
    {
        if (thisTurnManager.thisCharacter == this.gameObject)
        {
            
            thisTurnManager.endTurn();
        }
        thisMapManager.PositionToGameObject.Remove(this.gameObject.transform.position);
        Destroy(this.gameObject);
    }
    [SerializeField] Animator thisAnimation;
    public void ToggleCharacterTurnAnimation(bool isCharacterTurn)
    {
        if (!isCharacterTurn)
            thisAnimation.SetFloat("BlendSpeed", 0f);
        else
            thisAnimation.SetFloat("BlendSpeed", 1f);
    }
    public bool isPlayerCharacter = true;
    public void BeginThisCharacterTurn()
    {
        ToggleCharacterTurnAnimation(true);

        thisButtonManager.clearButtons();
        if (isPlayerCharacter)
            thisButtonManager.InstantiateButtons(GetCharacterMoveList());
        else
        {
            Debug.Log("AI Needed");
        }
    }
}