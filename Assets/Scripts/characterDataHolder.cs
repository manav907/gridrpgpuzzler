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
            //Debug.Log("AI Needed");
            moveDictionaryManager.doAction("Move");
        }
    }
    public Vector3Int getCharV3Int()
    {
        return tileCalculator.convertToVector3Int(this.gameObject.transform.position);
    }
    public Vector3Int moveToTarget(List<Vector3Int> validTargets)
    {
        Vector3Int thisCharpos = getCharV3Int();
        var tilesInVision = tileCalculator.generateRangeFromPoint(thisCharpos, rangeOfVision);
        var PTGODIR = thisMapManager.PositionToGameObject;
        PTGODIR.Remove(thisCharpos);
        List<Vector3Int> thisList = new List<Vector3Int>();
        foreach (Vector3Int thisPos in PTGODIR.Keys)
        {
            thisList.Add(thisPos);
        }
        var random = new System.Random();
        Vector3Int thisTarget = thisList[random.Next(thisList.Count)];

        SortedDictionary<float, Vector3Int> sortedListOfDistance = new SortedDictionary<float, Vector3Int>();
        foreach (Vector3Int movablePoints in validTargets)
        {
            float thisDistance = Vector3Int.Distance(movablePoints, thisTarget);
            if (sortedListOfDistance.ContainsKey(thisDistance))
            {
                sortedListOfDistance.Add(thisDistance + 0.001f, movablePoints);
            }
            else
            {
                sortedListOfDistance.Add(thisDistance, movablePoints);
            }

        }
        List<Vector3Int> listOFDistance = new List<Vector3Int>();
        foreach (var thisValie in sortedListOfDistance)
        {

            listOFDistance.Add(thisValie.Value);
           // Debug.Log(thisValie.Key + " " + thisValie.Value);
        }
        return listOFDistance[0];
    }
}
