using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveDictionaryManager : MonoBehaviour
{
    TurnManager turnManager;
    ReticalManager reticalManager;
    MapManager mapManager;
    MoveDictionaryManager moveDictionaryManager;
    UniversalCalculator universalCalculator;
    public void setMoveDictionaryManagerVariables()
    {
        turnManager = this.GetComponent<TurnManager>();
        reticalManager = this.GetComponent<ReticalManager>();
        mapManager = this.GetComponent<MapManager>();
        universalCalculator = this.GetComponent<UniversalCalculator>();
        SetMoveDictionary();
    }

    public Dictionary<string, ActionDataClass> aDCL;
    void SetMoveDictionary()
    {
        aDCL = new Dictionary<String, ActionDataClass>();
        //Debug.Log(thisCharacter.name);
        List<ActionDataClass> actionDataClass = new List<ActionDataClass>() {
            new ActionDataClass("Move", MoveCharacter, true, false, true, 1),
            new ActionDataClass("Attack", AttackHere, true, true, true || false, 2),
            new ActionDataClass("End Turn", endTurn, false, false, false, 0),
            new ActionDataClass("FireBall", ThrowFireBall, false, false, true, 2)
            };
        //This is for Refference (string NameofMove, Action actionOfMove, bool needsButton, bool GameObjectHere, bool WalkableTileHere, int rangeOfAction)    
        foreach (var thisactionData in actionDataClass)
            aDCL.Add(thisactionData.NameofMove, thisactionData);


    }
    public class ActionDataClass
    {
        public string NameofMove;
        public Action actionOfMove;
        public bool needsButton;
        public bool GameObjectHere;
        public bool WalkableTileHere;
        public int rangeOfAction;
        public ActionDataClass()
        {

        }
        public ActionDataClass(string NameofMove, Action actionOfMove, bool needsButton, bool GameObjectHere, bool WalkableTileHere, int rangeOfAction)
        {
            this.NameofMove = NameofMove;
            this.actionOfMove = actionOfMove;
            this.needsButton = needsButton;
            this.GameObjectHere = GameObjectHere;
            this.WalkableTileHere = WalkableTileHere;
            this.rangeOfAction = rangeOfAction;
        }
    }
    GameObject thisCharacter;
    characterDataHolder thisCharacterCDH;
    public void getThisCharacterData()//called from button manager to set character this turn
    {
        thisCharacter = turnManager.thisCharacter;
        thisCharacterCDH = thisCharacter.GetComponent<characterDataHolder>();
        PositionToGameObject = mapManager.PositionToGameObject;
    }

    public void doAction(string thisActionName)
    {
        ActionDataClass thisADL = aDCL[thisActionName];
        Action actionOfMove = thisADL.actionOfMove;
        bool needsButton = thisADL.needsButton;
        bool GameObjectHere = thisADL.GameObjectHere;
        bool WalkableTileHere = thisADL.WalkableTileHere;
        int rangeOfAction = thisADL.rangeOfAction;

        StartCoroutine(waitUntileButton());

        IEnumerator waitUntileButton()
        {
            listOfValidtargets = getValidTargetList(GameObjectHere, WalkableTileHere, rangeOfAction);
            if (thisCharacterCDH.isPlayerCharacter && needsButton)
            {
                reticalManager.reDrawValidTiles(listOfValidtargets);//this sets the Valid Tiles Overlay
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));//this waits for MB1 to be pressed before processeding
                actionOfMove();
                reticalManager.reDrawValidTiles(null);//this clears out the Valid Tiles Overlay
            }
            else
            {
                actionOfMove();
            }

            reticalManager.reDrawShadows();
        }

    }
    Vector3Int tryHere;
    Dictionary<Vector3Int, GameObject> PositionToGameObject;
    List<Vector3Int> listOfValidtargets;
    bool GetDataForActions()
    {
        if (thisCharacterCDH.isPlayerCharacter)
        {
            tryHere = reticalManager.getMovePoint();
            if (listOfValidtargets.Contains(tryHere))
                return true;
            else
                return false;
        }
        else
        {
            tryHere = thisCharacterCDH.moveToTarget(listOfValidtargets);
            //Debug.Log("get AI Here");
            return true;
        }
    }
    List<Vector3Int> getValidTargetList(bool GameObjectHere, bool WalkableTileHere, int rangeOfAction)
    {
        //getThisCharacterData();
        Vector3Int centerPos = universalCalculator.convertToVector3Int(thisCharacter.transform.position);
        List<Vector3Int> listOfRanges = universalCalculator.generateRangeFromPoint(centerPos, rangeOfAction);

        //The Following Removes Invalid Tiles
        for (int i = 0; i < listOfRanges.Count; i++)
        {
            bool isWalkableHere = mapManager.getIsWalkable(listOfRanges[i]);
            bool isGameObjectHere = PositionToGameObject.ContainsKey(listOfRanges[i]);
            if (isWalkableHere == WalkableTileHere && isGameObjectHere == GameObjectHere)
            {
                //Debug.Log(listOfAttackRange[i] + "Valid " + i);

                //Do Nothing since all conditions are fine
            }
            else
            {
                //Debug.Log(listOfAttackRange[i] + "Invalid ");
                listOfRanges.RemoveAt(i);
                i--;
                bool debugThis = false;
                if (debugThis)
                {
                    bool condtion = false;//Will be reassigned later
                    string needConditon = (condtion) ? "Need " : "Does Not Need ";//Used to Concatinate String
                    if (isWalkableHere != WalkableTileHere)
                    {
                        condtion = WalkableTileHere;
                        //Debug.Log(needConditon + "Walkable Tile Here " + " but isWalkableHere is " + isWalkableHere);
                        Debug.Log(needConditon + "Walkable Tile Here ");
                    }
                    if (isGameObjectHere != GameObjectHere)
                    {
                        condtion = GameObjectHere;
                        //Debug.Log(needConditon + "Game Object Here " + " but isGameObjectHere is " + isGameObjectHere);
                        Debug.Log(needConditon + "Game Object Here ");
                    }

                }
            }
        }
        return listOfRanges;
    }
    void ThrowFireBall()
    {
        universalCalculator.doThis();
        //Debug.Log("Throw Fire Ball");
        //endTurn();
    }
    void MoveCharacter()
    {
        if (GetDataForActions())
        {
            Vector3Int currentPosition = universalCalculator.convertToVector3Int(thisCharacter.transform.position);
            mapManager.UpdateCharacterPosition(currentPosition, tryHere, thisCharacter);
            thisCharacter.transform.position = tryHere;
            endTurn();
        }
        else
        {
            //Action Failed
            //Debug.Log("MoveCharacter");
        }
    }

    void AttackHere()
    {
        if (GetDataForActions())
        {
            characterDataHolder targetCharacter = PositionToGameObject[tryHere].gameObject.GetComponent<characterDataHolder>();
            characterDataHolder attackingCharacter = thisCharacter.GetComponent<characterDataHolder>();
            targetCharacter.health -= attackingCharacter.AttackDamage;
            targetCharacter.UpdateCharacterData();
            endTurn();
        }
        //else
        {
            //problem
            //Debug.Log("AttackHere");
        }

    }
    void endTurn()
    {
        characterDataHolder targetCharacter = thisCharacter.gameObject.GetComponent<characterDataHolder>();
        targetCharacter.ToggleCharacterTurnAnimation(false); ;
        targetCharacter.UpdateCharacterData();
        this.GetComponent<TurnManager>().endTurn();

    }
}
