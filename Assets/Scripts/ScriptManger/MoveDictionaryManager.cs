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
    Dictionary<AbilityName, Action> abilityNameToAction;
    void SetMoveDictionary()
    {
        abilityNameToAction = new Dictionary<AbilityName, Action>();
        abilityNameToAction.Add(AbilityName.Move, MoveCharacter);
        abilityNameToAction.Add(AbilityName.Attack, AttackHere);
        abilityNameToAction.Add(AbilityName.EndTurn, endTurn);
        abilityNameToAction.Add(AbilityName.OpenInventory, OpenInventory);
        abilityNameToAction.Add(AbilityName.CloseInventory, CloseInventory);
        abilityNameToAction.Add(AbilityName.FireBall, ThrowFireBall);
        abilityNameToAction.Add(AbilityName.HeartPickup, HeartPickup);

        //Co-RoutineStuff
        IEnumerator StartSequence(List<IEnumerator> coroutines)
        {
            coroutines.Add(endTurnCoroutine());
            foreach (IEnumerator coroutine in coroutines)
            {
                yield return StartCoroutine(coroutine);
                //Debug.Log("This Action was with result: " + (bool)coroutine.Current);
                yield return null;
            }
        }
        //ablity Actions
        void MoveCharacter()
        {
            var coroutines = new List<IEnumerator>();
            coroutines.Add(getInput(simpleMoveAction, thisCharacterCDH.GetAbilityRange(AbilityName.Move), false, true));
            StartCoroutine(StartSequence(coroutines));
        }
        void AttackHere()
        {
            var coroutines = new List<IEnumerator>();
            coroutines.Add(getInput(simpleAttackAction, thisCharacterCDH.GetAbilityRange(AbilityName.Attack), true, true || false));
            StartCoroutine(StartSequence(coroutines));
        }
        void endTurn()
        {
            StartCoroutine(endTurnCoroutine());
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
            thisCharacterCDH.health += 3;
            if (thisCharacterCDH.CheckIfCharacterIsDead() == false)
                endTurn();
        }
        //Simple Action and Co-routines that will be used for ablity Actions
        void simpleMoveAction()
        {
            Vector3Int currentPosition = universalCalculator.convertToVector3Int(thisCharacter.transform.position);
            mapManager.cellDataDir[currentPosition].characterAtCell = null;
            mapManager.cellDataDir[tryHere].characterAtCell = thisCharacter;
            thisCharacter.transform.position = tryHere;
        }
        void simpleAttackAction()
        {
            CharacterControllerScript targetCharacter = mapManager.cellDataDir[tryHere].characterAtCell.GetComponent<CharacterControllerScript>();
            CharacterControllerScript attackingCharacter = thisCharacter.GetComponent<CharacterControllerScript>();
            targetCharacter.health -= attackingCharacter.AttackDamage;
            checkCharacters(targetCharacter);
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
    }
    public void doAction(AbilityName abilityName)
    {
        abilityNameToAction[abilityName]();
    }
    Vector3Int tryHere;
    IEnumerator getInput(Action doThisAction, int rangeOfAction, bool requireCharacter, bool requireWalkability)
    {
        bool needsButton = rangeOfAction == 0 ? false : true;
        listOfValidtargets = getValidTargetList(requireCharacter, requireWalkability, rangeOfAction);
        bool succesfullyCompleted = false;
        while (!succesfullyCompleted)
        {
            if (thisCharacterCDH.isPlayerCharacter && needsButton)
                reticalManager.reDrawValidTiles(listOfValidtargets);//this sets the Valid Tiles Overlay

            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));//this waits for MB1 to be pressed before processeding
            if (GetDataForActions())
            {
                doThisAction();
                //doAction(AbilityName.EndTurn);
                succesfullyCompleted = true;
            }
            reticalManager.reDrawValidTiles(null);
            reticalManager.reDrawShadows();
        }
        bool GetDataForActions()
        {
            if (thisCharacterCDH.isPlayerCharacter)
            {
                tryHere = reticalManager.getMovePoint();
                if (listOfValidtargets.Contains(tryHere))
                {

                    return true;
                }
                else if (listOfValidtargets.Count == 0)
                {
                    Debug.Log("No Valid Tiles Exist; Ending GetData");
                    return false;
                }
                else
                    return false;
            }
            else
            {
                tryHere = thisCharacterCDH.getTarget(listOfValidtargets);
                return true;
            }
        }
    }



    List<Vector3Int> listOfValidtargets;
    [SerializeField] bool debugValidActionTiles = false;
    List<Vector3Int> getValidTargetList(bool requireCharacter, bool requireWalkability, int rangeOfAction)
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
            bool hasWalkability = mapManager.checkAtPosIfCharacterCanWalk(listOfRanges[i], thisCharacterCDH);
            bool hasCharacter = mapManager.cellDataDir[listOfRanges[i]].isCellHoldingCharacer();

            if (hasWalkability == requireWalkability && hasCharacter == requireCharacter)
            {
                //Debug.Log(listOfAttackRange[i] + "Valid " + i);
                //Do Nothing since all conditions are fine
            }
            else
            {
                //For Debugging
                if (debugValidActionTiles)
                {
                    bool condtion = false;//Will be reassigned later
                    string needConditon = (condtion) ? "Impossible Condition Occured for " : "Required ";//Used to Concatinate String
                    string debugLine = "Point " + listOfRanges[i] + " was Invalid as Tile ";
                    if (hasWalkability != requireWalkability)
                    {
                        condtion = requireWalkability;
                        debugLine += needConditon + "Ability to Walk ";
                    }
                    if (hasCharacter != requireCharacter)
                    {
                        condtion = requireCharacter;
                        debugLine += needConditon + "Character Here ";
                    }
                    if (rangeOfAction == 0)
                        debugLine += "and The Range of Action was " + rangeOfAction;
                    Debug.Log(debugLine);
                }
                //Actual Code 
                listOfRanges.RemoveAt(i);
                i--;
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