using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterDataHolder : MonoBehaviour
{
    public int health = 3;
    public int rangeOfMove = 1;
    public int rangeOfAttack = 2;
    public int AttackDamage = 2;
    public int speedValue = 3;
    public int rangeOfVision = 5;
    [SerializeField] private TextMesh Heatlh;

    private ButtonManager thisButtonManager;
    private MapManager thisMapManager;
    private TurnManager thisTurnManager;
    UniversalCalculator universalCalculator;
    public void InitilizeCharacter(GameObject gameController)
    {
        thisButtonManager = gameController.GetComponent<ButtonManager>();
        thisMapManager = gameController.GetComponent<MapManager>();
        thisTurnManager = gameController.GetComponent<TurnManager>();
        moveDictionaryManager = gameController.GetComponent<MoveDictionaryManager>();
        universalCalculator = gameController.GetComponent<UniversalCalculator>();
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

        Vector3Int thisCharPos = universalCalculator.convertToVector3Int(this.gameObject.transform.position);
        thisMapManager.PositionToGameObject.Remove(thisCharPos);
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
        thisButtonManager.clearButtons();
        if (isPlayerCharacter)
            thisButtonManager.InstantiateButtons(GetCharacterMoveList());
        else
        {
            determineAction();
        }
    }
    Vector3Int currentTarget;
    int GhostVision = 1;
    void determineAction()
    {
        Vector3Int thisCharpos = getCharV3Int();
        var VisionList = universalCalculator.generateRangeFromPoint(thisCharpos, rangeOfVision + GhostVision);
        var targetList = listOfPossibleTargets(VisionList);
        if (targetList.Count == 0)
        {
            Debug.Log("Ideling");
            moveDictionaryManager.doAction("End Turn");
            return;
        }
        else
        {
            currentTarget = targetList[universalCalculator.SelectRandomBetweenZeroAndInt(targetList.Count)];
            if (false)
            {
                //Attack Character
            }
            else if (true)//if character not in attack range
            {
                moveDictionaryManager.doAction("Move");
            }

        }
        List<Vector3Int> listOfPossibleTargets(List<Vector3Int> visionList)
        {
            var PTGODIR = thisMapManager.PositionToGameObject;
            List<Vector3Int> thisList = new List<Vector3Int>();
            foreach (Vector3Int thisPos in PTGODIR.Keys)
            {
                if (visionList.Contains(thisPos))
                    thisList.Add(thisPos);
            }
            thisList.Remove(thisCharpos);
            return thisList;
        }
    }

    public Vector3Int getCharV3Int()
    {
        return universalCalculator.convertToVector3Int(this.gameObject.transform.position);
    }
    public Vector3Int moveToTarget(List<Vector3Int> validTargets)
    {
        return universalCalculator.SortListAccordingtoDistanceFromPoint(validTargets, currentTarget)[0];
    }


}