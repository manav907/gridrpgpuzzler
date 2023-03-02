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
    void getThisCharacterData()
    {
        thisCharacter = turnManager.thisCharacter;
        thisCharacterCDH = thisCharacter.GetComponent<characterDataHolder>();
    }
    IEnumerator waitUntileButton(Action action, bool needsButton)
    {
        getThisCharacterData();
        if (thisCharacterCDH.isPlayerCharacter && needsButton)
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            action();
        }
        else
            action();
    }
    Vector3Int tryHere;
    Dictionary<Vector3, GameObject> PositionToGameObject;
    bool GetDataForActions(bool GameObjectHere, bool WalkableTileHere)
    {
        PositionToGameObject = mapManager.PositionToGameObject;
        if (thisCharacterCDH.isPlayerCharacter)
        {
            tryHere = reticalManager.getMovePoint();
            bool isWalkableHere = mapManager.getIsWalkable(tryHere);
            bool isGameObjectHere = PositionToGameObject.ContainsKey(tryHere);
            if (isWalkableHere == WalkableTileHere && isGameObjectHere == GameObjectHere)
            {

                return true;
            }
            else
            {
                bool debugThis = true;
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
                return false;
            }
        }
        else
        {
            Debug.Log("get AI Here");
            return false;
        }
    }
    void ThrowFireBall()
    {
        Debug.Log("Throw Fire Ball");
    }
    void MoveCharacter()
    {
        StartCoroutine(waitUntileButton(thisAction, true));//the co routine starts the action not all actions need a co routine     
        void thisAction()
        {
            if (GetDataForActions(false, true))
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
        StartCoroutine(waitUntileButton(thisAction, true));
        void thisAction()
        {
            if (GetDataForActions(true, true || false))
            {
                characterDataHolder targetCharacter = PositionToGameObject[tryHere].gameObject.GetComponent<characterDataHolder>();
                characterDataHolder attackingCharacter = thisCharacter.GetComponent<characterDataHolder>();
                targetCharacter.health -= attackingCharacter.AttackDamage;
                targetCharacter.UpdateCharacterData();
            }
            else
            {
                //problem
                //Debug.Log("AttackHere");
            }
        }

    }
    void endTurn()
    {
        StartCoroutine(waitUntileButton(thisAction, false));
        void thisAction()
        {
            characterDataHolder targetCharacter = thisCharacter.gameObject.GetComponent<characterDataHolder>();
            targetCharacter.ToggleCharacterTurnAnimation(false); ;
            targetCharacter.UpdateCharacterData();
            this.GetComponent<TurnManager>().endTurn();
        }
    }
}
