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
    DataManager dataManager;
    UniversalCalculator universalCalculator;
    ButtonManager buttonManager;
    [Header("Read Only Data")]
    [SerializeField] private float moveTimeSpeed = 0.12f;
    [Header("Character Data")]
    GameObject thisCharacter;
    [SerializeField] CharacterControllerScript characterCS;
    [Header("Retical And Tile Data")]
    [SerializeField] private Vector3Int tryHere;
    [Header("Current Ablity")]
    [SerializeField] Ability currentAblity;
    [Header("Debug Data")]
    [SerializeField][TextArea] string ValidTargetListDebugInfo;
    bool EditMapMode { get { return dataManager.EditMapMode; } }
    int alternateRange { get { return dataManager.alternateRange; } }
    bool checkValidActionTiles { get { return dataManager.checkValidActionTiles; } }

    bool ShouldContinue;
    public void setVariables()
    {
        turnManager = this.GetComponent<TurnManager>();
        reticalManager = this.GetComponent<ReticalManager>();
        mapManager = this.GetComponent<MapManager>();
        universalCalculator = this.GetComponent<UniversalCalculator>();
        buttonManager = this.GetComponent<ButtonManager>();
        dataManager = GetComponent<DataManager>();
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
            mapManager.UpdateCharacterPosistion(currentPosition, tryHere, thisCharacter);
            var ListOfMovePoints = new List<Vector3>();
            ListOfMovePoints.Add(tryHere);
            universalCalculator.MoveTransFromBetweenPoint(thisCharacter.transform, ListOfMovePoints, moveTimeSpeed);
            //thisCharacter.transform.position = tryHere;
        }
        void simpleAttackAction()
        {
            CharacterControllerScript targetCharacter = mapManager.cellDataDir[tryHere].characterAtCell.GetComponent<CharacterControllerScript>();
            CharacterControllerScript attackingCharacter = thisCharacter.GetComponent<CharacterControllerScript>();
            targetCharacter.health -= attackingCharacter.attackDamage;
            checkCharacters(targetCharacter);
            var ListOfMovePoints = new List<Vector3>();
            ListOfMovePoints.Add(tryHere);
            ListOfMovePoints.Add(attackingCharacter.getCharV3Int());
            universalCalculator.MoveTransFromBetweenPoint(thisCharacter.transform, ListOfMovePoints, moveTimeSpeed);
        }
        void simpleAoeAttackAction()
        {

            reticalManager.reticalShapes = ReticalShapes.SSweep;
            List<Vector3Int> aoeTargeted = reticalManager.generateShape(tryHere);
            reticalManager.reticalShapes = ReticalShapes.SSingle;
            foreach (Vector3Int point in aoeTargeted)
            {
                if (mapManager.cellDataDir.ContainsKey(point))
                    if (mapManager.isCellHoldingCharacer(point))
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
        float rangeOfAction = ability.GetRangeOfAction();
        AbilityName forceNextAbility = ability.forceAbility;
        if (rangeOfAction == 0)
            Debug.Log(rangeOfAction);
        List<Vector3Int> listOfValidtargets = getValidTargetList(ability);
        reticalManager.fromPoint = characterCS.getCharV3Int();
        ShouldContinue = false;

        //Executing Script
        if (!characterCS.controlCharacter)//if Non Player Character
        {
            tryHere = characterCS.getTarget(listOfValidtargets);
            ShouldContinue = true;
        }
        else//if it is the player character
        {
            reticalManager.reDrawValidTiles(listOfValidtargets);//this sets the Valid Tiles Overlay
            reticalManager.reticalShapes = ability.reticalShapes;
            reticalManager.rangeOfAction = rangeOfAction;
            yield return new WaitUntil(() => CheckContinue());//this waits for MB0 or MB1         
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
                var list = new List<AbilityName>()
                {
                    forceNextAbility,
                    AbilityName.EndTurn
                };
                buttonManager.InstantiateButtons(list);
            }
        }
        reticalManager.reDrawValidTiles(null);
        reticalManager.reDrawShadows();
        //Methods
        bool CheckContinue()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ShouldContinue = true;
                return true;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                ShouldContinue = false;
                return true;
            }
            return false;
        }
        bool CheckMovePoint()
        {
            if (ShouldContinue && listOfValidtargets.Contains(tryHere))
            {
                return true;
            }
            else if (listOfValidtargets.Count == 0)
            {
                Debug.Log("No Valid Tiles Exist; Ending GetData; Debugging Just in Case;");
                getValidTargetList(ability);
                return false;
            }
            else
                return false;
        }
    }
    public List<Vector3Int> getValidTargetList(Ability ability)
    {
        float rangeOfAction;
        if (!EditMapMode)
            rangeOfAction = ability.GetRangeOfAction();
        else
            rangeOfAction = alternateRange;

        //Debug.Log("Generating List of valid Targets for the character" + thisCharacter.name);
        Vector3Int centerPos = universalCalculator.convertToVector3Int(thisCharacter.transform.position);
        List<Vector3Int> listOfRanges = universalCalculator.generateTaxiRangeFromPoint(centerPos, rangeOfAction);
        List<Vector3Int> listOfNonNullTiles = new List<Vector3Int>(mapManager.cellDataDir.Keys);
        if (!ability.disregardWalkablity)
            listOfRanges = universalCalculator.filterOutList(listOfRanges, listOfNonNullTiles);
        listOfRanges.Remove(centerPos);
        ValidTargetListDebugInfo = "Data for Invalid Tiles \n";
        //The Following Removes Invalid Tiles
        for (int i = 0; i < listOfRanges.Count; i++)
        {
            //Normal Checks         
            bool hasWalkability = ability.disregardWalkablity ? true : mapManager.checkAtPosIfCharacterCanWalk(listOfRanges[i], characterCS);
            bool requireCharacterCondition = GlobalCal.compareBool(mapManager.isCellHoldingCharacer(listOfRanges[i]), ability.requireCharacterBoolEnum);
            if (hasWalkability && requireCharacterCondition)
            {/*Do Nothing since all conditions are fine*/}
            else
            {
                //For Debugging
                if (checkValidActionTiles)
                {
                    bool condtion = false;//Will be reassigned later                        
                    string debugLine = "For Action " + ability.abilityName + " Point " + listOfRanges[i] + " was Invalid as Tile ";
                    string needConditon = (condtion) ? "Impossible Condition Occured for " : "Required ";//Used to Concatinate String
                    if (!requireCharacterCondition)
                    {
                        condtion = requireCharacterCondition;
                        debugLine += needConditon + "Character Here; ";
                    }
                    if (rangeOfAction == 0)
                        debugLine += "The Range of Action was " + rangeOfAction;
                    ValidTargetListDebugInfo += debugLine + "\n";
                }
                //Actual Code 
                listOfRanges.RemoveAt(i);
                i--;
            }
        }
        if (checkValidActionTiles)
            Debug.Log(ValidTargetListDebugInfo);
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
    public Ability()
    {

    }
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
    public float GetRangeOfAction()
    {
        string rangeString = rangeOfActionEnum.ToString();
        rangeString = rangeString.Replace("r", "");
        return float.Parse(rangeString) / 10;
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
public enum ValidTileType
{
    PointTargeted,
    UnitTargeted,
    EmptyCellTargeted
}