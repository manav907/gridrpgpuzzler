using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterDataHolder : MonoBehaviour
{
    public int health = 3;
    public int AttackDamage = 2;
    public int speedValue = 3;
    public int rangeOfVision = 5;
    [SerializeField] private TextMesh Heatlh;

    private ButtonManager thisButtonManager;
    private MapManager thisMapManager;
    private TurnManager thisTurnManager;
    TileCalculator tileCalculator;
    public void InitilizeCharacter(GameObject gameController)
    {
        thisButtonManager = gameController.GetComponent<ButtonManager>();
        thisMapManager = gameController.GetComponent<MapManager>();
        thisTurnManager = gameController.GetComponent<TurnManager>();
        moveDictionaryManager = gameController.GetComponent<MoveDictionaryManager>();
        tileCalculator = gameController.GetComponent<TileCalculator>();
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

        Vector3Int thisCharPos = tileCalculator.convertToVector3Int(this.gameObject.transform.position);
        thisMapManager.PositionToGameObject.Remove(thisCharPos);
        //thisMapManager.UpdateCharacterPosition(thisCharPos, null, null);
        Destroy(this.gameObject);
        if (thisTurnManager.thisCharacter == this.gameObject)
        {
            thisTurnManager.endTurn();
        }
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
    MoveDictionaryManager moveDictionaryManager;
    public void BeginThisCharacterTurn()
    {
        ToggleCharacterTurnAnimation(true);
        moveDictionaryManager.getThisCharacterData();
        thisButtonManager.clearButtons();
        if (isPlayerCharacter)
            thisButtonManager.InstantiateButtons(GetCharacterMoveList());
        else
        {
            Debug.Log("AI Needed");
        }
    }
    public Vector3Int getCharV3Int()
    {
        return tileCalculator.convertToVector3Int(this.gameObject.transform.position);
    }
}
