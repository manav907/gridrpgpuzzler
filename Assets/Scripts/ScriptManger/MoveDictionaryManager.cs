using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public class MoveDictionaryManager : MonoBehaviour
{
    TurnManager turnManager;
    ReticalManager reticalManager;
    MapManager mapManager;
    UniversalCalculator universalCalculator;
    ButtonManager buttonManager;
    [Header("Read Only Data")]
    [Header("Character Data")]
    GameObject thisCharacter;
    [SerializeField] CharacterControllerScript characterCS;
    [Header("Retical And Tile Data")]
    [SerializeField] private Vector3Int tryHere;
    [SerializeField] bool checkValidActionTiles = false;
    [Header("Current Ablity")]
    [SerializeField] Ability currentAblity;
    public void setVariables()
    {
        turnManager = this.GetComponent<TurnManager>();
        reticalManager = this.GetComponent<ReticalManager>();
        mapManager = this.GetComponent<MapManager>();
        universalCalculator = this.GetComponent<UniversalCalculator>();
        buttonManager = this.GetComponent<ButtonManager>();
        SetMoveDictionary();
    }

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
        abilityNameToAction.Add(AbilityName.AxeSweep, AxeSweep);

        //ablity Actions
        void MoveCharacter()
        {
            //Ability ability = new Ability(AbilityName.Move, ValidTargetData.OnOccupiable, DirectionOfAction.complete);
            StartCoroutine(getInput(simpleMoveAction, AbilityName.Move));
        }
        void AttackHere()
        {
            StartCoroutine(getInput(simpleAttackAction, AbilityName.Attack));
        }
        void DoubleAttack()
        {
            StartCoroutine(getInput(simpleAttackAction, AbilityName.Attack));
        }
        void EndTurn()
        {
            if (characterCS.doActionPointsRemainAfterAbility() == false)
            {
                CharacterControllerScript targetCharacter = thisCharacter.gameObject.GetComponent<CharacterControllerScript>();
                targetCharacter.ToggleCharacterTurnAnimation(false); ;
                this.GetComponent<TurnManager>().endTurn();
            }
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
        void AxeSweep()
        {
            StartCoroutine(getInput(simpleAoeAttackAction, AbilityName.AxeSweep));


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
        void simpleAoeAttackAction()
        {
            reticalManager.reticalShapes = ReticalShapes.SSweep;
            List<Vector3Int> aoeTargeted = reticalManager.generateShape(tryHere);
            reticalManager.reticalShapes = ReticalShapes.SSingle;
            foreach (Vector3Int point in aoeTargeted)
            {
                if (mapManager.cellDataDir[point].isCellHoldingCharacer())
                {
                    tryHere = point;
                    simpleAttackAction();
                }
            }
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

    IEnumerator getInput(Action doThisAction, AbilityName forAbilityData)
    {
        //Declaring Variables
        Ability ability = characterCS.AbilityNameToAbilityDataDIR[forAbilityData];
        currentAblity = ability;
        //int rangeOfAction = characterCS.GetAbilityRange(ability.abilityName);
        float rangeOfAction = ability.GetRangeOfAction();
        AbilityName forceNextAbility = ability.forceAbility;
        //Executing Script
        if (rangeOfAction == 0)
            Debug.Log(rangeOfAction);
        List<Vector3Int> listOfValidtargets = getValidTargetList(ability);
        reticalManager.fromPoint = characterCS.getCharV3Int();
        if (!characterCS.isPlayerCharacter)//if Non Player Character
        {
            tryHere = characterCS.getTarget(listOfValidtargets);
        }
        else//if it is the player character
        {
            reticalManager.reDrawValidTiles(listOfValidtargets);//this sets the Valid Tiles Overlay
            reticalManager.reticalShapes = ability.reticalShapes;
            yield return null;
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));//this waits for MB1 to be pressed before processeding
            tryHere = reticalManager.getMovePoint();
            reticalManager.reticalShapes = ReticalShapes.SSingle;
        }
        if (CheckMovePoint())//if Getting tryHere was at a Valid Tile
        {
            doThisAction();
            if (forceNextAbility == AbilityName.EndTurn)
            { doAction(AbilityName.EndTurn); }
            else
            {
                var list = new List<AbilityName>(){
                    forceNextAbility,
                    AbilityName.EndTurn
                };
                buttonManager.InstantiateButtons(list);
            }
        }
        reticalManager.reDrawValidTiles(null);
        reticalManager.reDrawShadows();
        //Methods
        bool CheckMovePoint()
        {
            if (listOfValidtargets.Contains(tryHere))
            {
                return true;
            }
            else if (listOfValidtargets.Count == 0)
            {
                Debug.Log("No Valid Tiles Exist; Ending GetData");
                if (checkValidActionTiles == false)
                {
                    checkValidActionTiles = true;
                    getValidTargetList(ability);
                    checkValidActionTiles = false;
                }
                return false;
            }
            else
                return false;
        }
    }
    public List<Vector3Int> getValidTargetList(Ability ability)
    {
        float rangeOfAction = ability.GetRangeOfAction();
        bool requireCharacter = ability.requireCharacter();
        //Debug.Log("Generating List of valid Targets for the character" + thisCharacter.name);
        Vector3Int centerPos = universalCalculator.convertToVector3Int(thisCharacter.transform.position);
        List<Vector3Int> listOfRanges = universalCalculator.generateTaxiRangeFromPoint(centerPos, rangeOfAction);
        List<Vector3Int> listOfNonNullTiles = new List<Vector3Int>(mapManager.cellDataDir.Keys);
        listOfRanges = universalCalculator.filterOutList(listOfRanges, listOfNonNullTiles);
        //The Following Removes Invalid Tiles
        for (int i = 0; i < listOfRanges.Count; i++)
        {
            //Normal Checks         
            bool hasWalkability = ability.disregardWalkablity ? true : mapManager.checkAtPosIfCharacterCanWalk(listOfRanges[i], characterCS);
            bool hasCharacter = mapManager.cellDataDir[listOfRanges[i]].isCellHoldingCharacer();
            if (hasWalkability && hasCharacter == requireCharacter)
            {/*Do Nothing since all conditions are fine*/}
            else
            {
                //For Debugging
                if (checkValidActionTiles)
                {
                    bool condtion = false;//Will be reassigned later                        
                    string debugLine = "For Action " + ability.abilityName + " Point " + listOfRanges[i] + " was Invalid as Tile ";
                    string needConditon = (condtion) ? "Impossible Condition Occured for " : "Required ";//Used to Concatinate String
                    if (hasCharacter != requireCharacter)
                    {
                        condtion = requireCharacter;
                        debugLine += needConditon + "Character Here; ";
                    }
                    if (rangeOfAction == 0)
                        debugLine += "The Range of Action was " + rangeOfAction;
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
    EndTurn,
    Move,
    Attack,
    DoubleAttack,
    OpenInventory,
    CloseInventory,
    FireBall,
    HeartPickup,
    DoubleTeam,
    AxeSweep
}

[Serializable]
public class Ability
{
    public String abilityString;
    public AbilityName abilityName;
    public AbilityName forceAbility = AbilityName.EndTurn;
    public RangeOfActionEnum rangeOfActionEnum = RangeOfActionEnum.r10;
    public ReticalShapes reticalShapes = ReticalShapes.SSingle;
    public BoolEnum requireCharacterBoolEnum = BoolEnum.TrueOrFalse;
    public bool disregardWalkablity = false;
    public Ability(Ability ability)
    {
        abilityString = ability.abilityString;
        abilityName = ability.abilityName;
        forceAbility = ability.forceAbility;
        rangeOfActionEnum = ability.rangeOfActionEnum;
        reticalShapes = ability.reticalShapes;
        requireCharacterBoolEnum = ability.requireCharacterBoolEnum;
        disregardWalkablity = ability.disregardWalkablity;
    }
    public bool requireCharacter()
    {
        return convertToBool(requireCharacterBoolEnum);
    }
    public float GetRangeOfAction()
    {
        string rangeString = rangeOfActionEnum.ToString();
        rangeString = rangeString.Replace("r", "");
        return float.Parse(rangeString) / 10;
    }
    bool convertToBool(BoolEnum boolEnum)
    {
        if (boolEnum == BoolEnum.TrueOrFalse)
            return true || false;
        if (boolEnum == BoolEnum.True)
            return true;
        if (boolEnum == BoolEnum.False)
            return false;
        return false;
    }

}
public enum BoolEnum
{
    TrueOrFalse,
    True,
    False
}
public enum RangeOfActionEnum
{
    r0, r10, r15, r20, r25, r30
}