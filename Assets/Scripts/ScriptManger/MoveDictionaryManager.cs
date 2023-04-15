using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MoveDictionaryManager : MonoBehaviour
{
    TurnManager turnManager;
    ReticalManager reticalManager;
    MapManager mapManager;
    MoveDictionaryManager moveDictionaryManager;
    UniversalCalculator universalCalculator;
    ButtonManager buttonManager;
    public void setVariables()
    {
        turnManager = this.GetComponent<TurnManager>();
        reticalManager = this.GetComponent<ReticalManager>();
        mapManager = this.GetComponent<MapManager>();
        universalCalculator = this.GetComponent<UniversalCalculator>();
        buttonManager = this.GetComponent<ButtonManager>();
        SetMoveDictionary();
    }
    GameObject thisCharacter;
    CharacterControllerScript thisCharacterCDH;
    public void getThisCharacterData()
    {
        thisCharacter = turnManager.thisCharacter;
        //Debug.Log(thisCharacter.name);//
        thisCharacterCDH = thisCharacter.GetComponent<CharacterControllerScript>();
    }
    Dictionary<AbilityName, Action> ANtoA;

    void SetMoveDictionary()
    {
        ANtoA = new Dictionary<AbilityName, Action>();
        ANtoA.Add(AbilityName.Move, MoveCharacter);
        ANtoA.Add(AbilityName.Attack, AttackHere);
        ANtoA.Add(AbilityName.EndTurn, endTurn);
        ANtoA.Add(AbilityName.OpenInventory, OpenInventory);
        ANtoA.Add(AbilityName.CloseInventory, CloseInventory);
        ANtoA.Add(AbilityName.FireBall, ThrowFireBall);
        ANtoA.Add(AbilityName.HeartPickup, HeartPickup);
        void MoveCharacter()
        {
            List<IEnumerator> coroutines = new List<IEnumerator>()
            {
                getInput(simpleMoveAction,false,true,true,AbilityName.Move),
                endTurnCoroutine()
            };
            StartCoroutine(StartSequence(coroutines));
        }
        void simpleMoveAction()
        {
            Vector3Int currentPosition = universalCalculator.convertToVector3Int(thisCharacter.transform.position);
            mapManager.cellDataDir[currentPosition].characterAtCell = null;
            mapManager.cellDataDir[tryHere].characterAtCell = thisCharacter;
            thisCharacter.transform.position = tryHere;
        }
        void AttackHere()
        {
            List<IEnumerator> coroutines = new List<IEnumerator>()
            {
                getInput(simpleAttackAction,true,true||false,true,AbilityName.Attack),
                endTurnCoroutine()
            };
            StartCoroutine(StartSequence(coroutines));
        }
        void simpleAttackAction()
        {
            CharacterControllerScript targetCharacter = mapManager.cellDataDir[tryHere].characterAtCell.GetComponent<CharacterControllerScript>();
            CharacterControllerScript attackingCharacter = thisCharacter.GetComponent<CharacterControllerScript>();
            targetCharacter.health -= attackingCharacter.AttackDamage;
            checkCharacters(targetCharacter);
        }
        void endTurn()
        {
            StartCoroutine(endTurnCoroutine());

        }
        IEnumerator endTurnCoroutine()
        {
            yield return null;
            CharacterControllerScript targetCharacter = thisCharacter.gameObject.GetComponent<CharacterControllerScript>();
            targetCharacter.ToggleCharacterTurnAnimation(false); ;
            this.GetComponent<TurnManager>().endTurn();
        }
        void checkCharacters(CharacterControllerScript targetCharacter)
        {
            targetCharacter.CheckIfCharacterIsDead();
        }
        void OpenInventory()
        {
            //Debug.Log("Open Inv");
            buttonManager.InstantiateButtons(thisCharacterCDH.CharacterInventoryList);
        }
        void CloseInventory()
        {
            //Debug.Log("Close Inv");
            buttonManager.InstantiateButtons(thisCharacterCDH.CharacterMoveList);
        }
        void ThrowFireBall()
        {
            Debug.Log("Throw Fire Ball");
        }
        void HeartPickup()
        {
            //Debug.Log(thisCharacterCDH.health);
            thisCharacterCDH.health += 3;
            if (thisCharacterCDH.CheckIfCharacterIsDead() == false)
                endTurn();
        }
    }
    IEnumerator StartSequence(List<IEnumerator> coroutines)
    {
        foreach (IEnumerator coroutine in coroutines)
        {
            yield return StartCoroutine(coroutine);
            yield return null;
        }
    }

    IEnumerator getInput(Action doThisAction, bool gameObjectHere, bool walkableTileHere, bool needsButton, AbilityName abilityName)
    {
        //Debug.Log("Get Input Works");
        listOfValidtargets = getValidTargetList(gameObjectHere, walkableTileHere, thisCharacterCDH.GetAbilityRange(abilityName));
        if (thisCharacterCDH.isPlayerCharacter && needsButton)
            reticalManager.reDrawValidTiles(listOfValidtargets);//this sets the Valid Tiles Overlay

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));//this waits for MB1 to be pressed before processeding
        if (GetDataForActions())
        {
            doThisAction();
        }
        reticalManager.reDrawValidTiles(null);
        reticalManager.reDrawShadows();
    }

    public void doAction(AbilityName abilityName)
    {
        ANtoA[abilityName]();
    }
    Vector3Int tryHere;
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
            tryHere = thisCharacterCDH.getTarget(listOfValidtargets);
            return true;
        }
    }
    List<Vector3Int> getValidTargetList(bool GameObjectHere, bool WalkableTileHere, int rangeOfAction)
    {
        //Debug.Log("Generating List of valid Targets for the character" + thisCharacter.name);
        Vector3Int centerPos = universalCalculator.convertToVector3Int(thisCharacter.transform.position);
        List<Vector3Int> listOfRanges = universalCalculator.generateRangeFromPoint(centerPos, rangeOfAction);
        List<Vector3Int> listOfNonNullTiles = new List<Vector3Int>(mapManager.cellDataDir.Keys);
        listOfRanges = universalCalculator.filterOutList(listOfRanges, listOfNonNullTiles);
        //The Following Removes Invalid Tiles
        for (int i = 0; i < listOfRanges.Count; i++)
        {
            //Normal Checks         
            bool isWalkableHere = mapManager.checkAtPosIfCharacterCanWalk(listOfRanges[i], thisCharacterCDH);
            bool isGameObjectHere = mapManager.cellDataDir[listOfRanges[i]].isCellHoldingCharacer();

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
}
public enum AbilityName
{
    Move,
    Attack,
    EndTurn,
    OpenInventory,
    CloseInventory,
    FireBall,
    HeartPickup
}