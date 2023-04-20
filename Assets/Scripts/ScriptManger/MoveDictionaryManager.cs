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
    CharacterControllerScript characterCS;
    public void getThisCharacterData()
    {
        thisCharacter = turnManager.thisCharacter;
        //Debug.Log(thisCharacter.name);//
        characterCS = thisCharacter.GetComponent<CharacterControllerScript>();
    }
    Dictionary<AbilityName, Action> abilityNameToAction;
    void SetMoveDictionary()
    {
        abilityNameToAction = new Dictionary<AbilityName, Action>();
        abilityNameToAction.Add(AbilityName.Move, MoveCharacter);
        abilityNameToAction.Add(AbilityName.Attack, AttackHere);
        abilityNameToAction.Add(AbilityName.DoubleAttack, DoubleAttack);
        abilityNameToAction.Add(AbilityName.EndTurn, EndTurn);
        abilityNameToAction.Add(AbilityName.OpenInventory, OpenInventory);
        abilityNameToAction.Add(AbilityName.CloseInventory, CloseInventory);
        abilityNameToAction.Add(AbilityName.FireBall, ThrowFireBall);
        abilityNameToAction.Add(AbilityName.HeartPickup, HeartPickup);
        abilityNameToAction.Add(AbilityName.DoubleTeam, DoubleTeam);


        //ablity Actions
        void MoveCharacter()
        {
            //Ability ability = new Ability(AbilityName.Move, ValidTargetData.OnOccupiable, DirectionOfAction.complete);
            StartCoroutine(getInput
            (simpleMoveAction, characterCS.GetAbilityRange(AbilityName.Move), AbilityName.Move));
        }
        void AttackHere()
        {
            StartCoroutine(getInput
            (simpleAttackAction, characterCS.GetAbilityRange(AbilityName.Attack), AbilityName.Attack));
        }
        void DoubleAttack()
        {
            StartCoroutine(getInput
            (simpleAttackAction, characterCS.GetAbilityRange(AbilityName.Attack), AbilityName.Attack, AbilityName.Attack));
        }
        void EndTurn()
        {
            CharacterControllerScript targetCharacter = thisCharacter.gameObject.GetComponent<CharacterControllerScript>();
            targetCharacter.ToggleCharacterTurnAnimation(false); ;
            this.GetComponent<TurnManager>().endTurn();
        }
        void OpenInventory()
        {
            buttonManager.InstantiateButtons(characterCS.CharacterInventoryList);
        }
        void CloseInventory()
        {
            buttonManager.InstantiateButtons(characterCS.CharacterMoveList);
        }
        void ThrowFireBall()
        {
            Debug.Log("Throw Fire Ball");
            CloseInventory();
        }
        void HeartPickup()
        {
            characterCS.health += 3;
            CloseInventory();
            if (characterCS.CheckIfCharacterIsDead() == false)
                EndTurn();

        }
        void DoubleTeam()
        { characterCS.actionPoints += 1; }
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
    [SerializeField] bool checkValidActionTiles = false;
    IEnumerator getInput
    (Action doThisAction, int rangeOfAction, AbilityName forAbilityData, AbilityName? forceNextAbility = null)
    {
        Ability ability = characterCS.AbilityNameToAbilityDataDIR[forAbilityData];
        bool requireCharacter = ability.requireCharacter();
        bool requireWalkability = ability.requireWalkability();
        if (rangeOfAction == 0)
            Debug.Log(rangeOfAction);
        List<Vector3Int> listOfValidtargets = getValidTargetList();
        if (characterCS.isPlayerCharacter)
            reticalManager.reDrawValidTiles(listOfValidtargets);//this sets the Valid Tiles Overlay
        yield return null;
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));//this waits for MB1 to be pressed before processeding
        if (GetDataForActions())
        {
            doThisAction();
            if (forceNextAbility.HasValue)
            {
                var list = new List<AbilityName>(){
                    forceNextAbility.Value,
                    AbilityName.EndTurn
                };
                buttonManager.InstantiateButtons(list);
            }
            else if (characterCS.doActionPointsRemainAfterAbility() == false)
            {
                doAction(AbilityName.EndTurn);
            }
        }
        reticalManager.reDrawValidTiles(null);
        reticalManager.reDrawShadows();
        //Methods
        bool GetDataForActions()
        {
            if (characterCS.isPlayerCharacter)
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
                tryHere = characterCS.getTarget(listOfValidtargets);
                return true;
            }
        }
        List<Vector3Int> getValidTargetList()
        {
            //Debug.Log("Generating List of valid Targets for the character" + thisCharacter.name);
            Vector3Int centerPos = universalCalculator.convertToVector3Int(thisCharacter.transform.position);
            List<Vector3Int> listOfRanges = universalCalculator.generateRangeFromPoint(centerPos, rangeOfAction);
            if (ability.directionOfAction == DirectionOfAction.Taxi)
            {
                //listOfRanges = universalCalculator.generateWay4RangeFromPoint(centerPos, rangeOfAction);
                listOfRanges = universalCalculator.generateTaxiRangeFromPoint(centerPos, rangeOfAction);
            }
            List<Vector3Int> listOfNonNullTiles = new List<Vector3Int>(mapManager.cellDataDir.Keys);
            listOfRanges = universalCalculator.filterOutList(listOfRanges, listOfNonNullTiles);
            //The Following Removes Invalid Tiles
            for (int i = 0; i < listOfRanges.Count; i++)
            {
                //Normal Checks         
                bool hasWalkability = mapManager.checkAtPosIfCharacterCanWalk(listOfRanges[i], characterCS);
                bool hasCharacter = mapManager.cellDataDir[listOfRanges[i]].isCellHoldingCharacer();

                if (hasWalkability == requireWalkability && hasCharacter == requireCharacter)
                {/*Do Nothing since all conditions are fine*/}
                else
                {
                    //For Debugging
                    if (checkValidActionTiles)
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
}
public enum AbilityName
{
    Move,
    Attack,
    DoubleAttack,
    EndTurn,
    OpenInventory,
    CloseInventory,
    FireBall,
    HeartPickup,
    DoubleTeam
}
public enum DirectionOfAction
{
    complete,
    Taxi
}
[Serializable]
public class Ability
{
    public AbilityName abilityName;
    public BoolEnum requireCharacterBoolEnum = BoolEnum.False;
    public BoolEnum requireWalkabilityBoolEnum = BoolEnum.False;
    public bool requireCharacter()
    {
        return convertToBool(requireCharacterBoolEnum);
    }
    public bool requireWalkability()
    {
        return convertToBool(requireWalkabilityBoolEnum);
    }
    public DirectionOfAction directionOfAction;
    bool convertToBool(BoolEnum boolEnum)
    {
        if (boolEnum == BoolEnum.True)
            return true;
        if (boolEnum == BoolEnum.False)
            return false;
        if (boolEnum == BoolEnum.TrueOrFalse)
            return true || false;
        if (boolEnum == BoolEnum.TrueAndFalse)
            return true && false;
        return false;
    }

}
public enum BoolEnum
{
    True,
    False,
    TrueOrFalse,
    TrueAndFalse


}