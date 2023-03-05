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
    public void setMoveDictionaryManagerVariables()
    {
        turnManager = this.GetComponent<TurnManager>();
        reticalManager = this.GetComponent<ReticalManager>();
        mapManager = this.GetComponent<MapManager>();
        SetMoveDictionary();
    }
    public Dictionary<string, Action> MoveNameToActionDictionary;
    void SetMoveDictionary()
    {
        MoveNameToActionDictionary = new Dictionary<String, Action>();
        MoveNameToActionDictionary.Add("Move", MoveCharacter);
        MoveNameToActionDictionary.Add("Attack", AttackHere);
        MoveNameToActionDictionary.Add("End Turn", endTurn);
        MoveNameToActionDictionary.Add("FireBall", ThrowFireBall);
    }
    GameObject thisCharacter;
    characterDataHolder thisCharacterCDH;
    public void getThisCharacterData()//called from button manager to set character this turn
    {
        thisCharacter = turnManager.thisCharacter;
        thisCharacterCDH = thisCharacter.GetComponent<characterDataHolder>();
        PositionToGameObject = mapManager.PositionToGameObject;
    }
    IEnumerator waitUntileButton(Action action, bool needsButton)
    {
        getThisCharacterData();
        if (thisCharacterCDH.isPlayerCharacter && needsButton)
        {
            reticalManager.reDrawValidTiles(listOfValidtargets);//try this but null
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            action();
            reticalManager.reDrawValidTiles(null);//try this but null
        }
        else
            action();
    }
    Vector3Int tryHere;
    Dictionary<Vector3, GameObject> PositionToGameObject;
    List<Vector3Int> listOfValidtargets;
    bool GetDataForActions()
    {

        if (thisCharacterCDH.isPlayerCharacter)
        {
            reticalManager.reDrawValidTiles(listOfValidtargets);//try this
            tryHere = reticalManager.getMovePoint();
            if (listOfValidtargets.Contains(tryHere))
                return true;
            else
                return false;
        }
        else
        {
            Debug.Log("get AI Here");
            return false;
        }
    }
    List<Vector3Int> getValidTargetList(bool GameObjectHere, bool WalkableTileHere, int rangeOfAction)
    {
        getThisCharacterData();
        Vector3 characterPos = thisCharacter.transform.position;
        Vector3 startRange = characterPos - new Vector3(rangeOfAction, rangeOfAction);
        Vector3 endRange = characterPos + new Vector3(rangeOfAction, rangeOfAction);
        List<Vector3Int> listOfAttackRange = new List<Vector3Int>();
        for (int x = (int)startRange.x; x <= endRange.x; x++)
        {
            for (int y = (int)startRange.y; y <= endRange.y; y++)
            {
                Vector3Int atXY = new Vector3Int(x, y, 0);
                listOfAttackRange.Add(atXY);
                //Debug.Log(atXY + " ");
            }
        }
        for (int i = 0; i < listOfAttackRange.Count; i++)
        {
            bool isWalkableHere = mapManager.getIsWalkable(listOfAttackRange[i]);
            bool isGameObjectHere = PositionToGameObject.ContainsKey(listOfAttackRange[i]);
            if (isWalkableHere == WalkableTileHere && isGameObjectHere == GameObjectHere)
            {
                //Debug.Log(listOfAttackRange[i] + "Valid " + i);
            }
            else
            {
                //Debug.Log(listOfAttackRange[i] + "Invalid ");
                listOfAttackRange.RemoveAt(i);
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
        return listOfAttackRange;
    }
    void ThrowFireBall()
    {
        Debug.Log("Throw Fire Ball");
    }
    void MoveCharacter()
    {
        bool needsButton = true;
        listOfValidtargets = getValidTargetList(false, true, thisCharacterCDH.rangeOfMove);
        StartCoroutine(waitUntileButton(thisAction, needsButton));//the co routine starts the action not all actions need a co routine     
        void thisAction()
        {
            if (GetDataForActions())
            {
                Vector3 currentPosition = thisCharacter.transform.position;
                mapManager.UpdateCharacterPosition(currentPosition, tryHere, thisCharacter);
                thisCharacter.transform.position = tryHere;
            }
            else
            {
                //Action Failed
                //Debug.Log("MoveCharacter");
            }
        }
    }

    void AttackHere()
    {
        bool needsButton = true;
        listOfValidtargets = getValidTargetList(true, true || false, thisCharacterCDH.attackRange);
        StartCoroutine(waitUntileButton(thisAction, needsButton));
        void thisAction()
        {
            if (GetDataForActions())
            {
                characterDataHolder targetCharacter = PositionToGameObject[tryHere].gameObject.GetComponent<characterDataHolder>();
                characterDataHolder attackingCharacter = thisCharacter.GetComponent<characterDataHolder>();
                targetCharacter.health -= attackingCharacter.AttackDamage;
                targetCharacter.UpdateCharacterData();
            }
            //else
            {
                //problem
                //Debug.Log("AttackHere");
            }
        }

    }
    void endTurn()
    {
        bool needsButton = false;
        StartCoroutine(waitUntileButton(thisAction, needsButton));
        void thisAction()
        {
            characterDataHolder targetCharacter = thisCharacter.gameObject.GetComponent<characterDataHolder>();
            targetCharacter.ToggleCharacterTurnAnimation(false); ;
            targetCharacter.UpdateCharacterData();
            this.GetComponent<TurnManager>().endTurn();
        }
    }
}
