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
    DataManager dataManager;
    UniversalCalculator universalCalculator;
    ButtonManager buttonManager;
    [Header("Read Only Data")]
    private float moveTimeSpeed = 0.12f;
    [Header("Character Data")]
    GameObject thisCharacter;
    CharacterControllerScript characterCS;
    [Header("Retical And Tile Data")]
    private Vector3Int tryHere;
    [Header("Current Ablity")]
    //[SerializeField] Ability currentAblity;
    [Header("Debug Data")]
    [SerializeField][TextArea] string ValidTargetListDebugInfo;
    bool EditMapMode { get { return dataManager.EditMapMode; } }
    int alternateRange { get { return dataManager.alternateRange; } }
    bool checkValidActionTiles { get { return dataManager.checkValidActionTiles; } }

    bool ShouldContinue;
    [Header("Tool Tips")]
    [SerializeField] private TMPro.TextMeshProUGUI toolTip;
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
    Dictionary<TypeOfAction, Action> TypeOFActionToAction;
    void SetMoveDictionary()
    {
        abilityNameToAction = new Dictionary<AbilityName, Action>();
        abilityNameToAction.Add(AbilityName.DoubleTeam, DoubleTeam);
        abilityNameToAction.Add(AbilityName.AxeSweep, AxeSweep);
        abilityNameToAction.Add(AbilityName.Restart, Restart);

        TypeOFActionToAction = new Dictionary<TypeOfAction, Action>();

        TypeOFActionToAction.Add(TypeOfAction.apply_Damage, apply_Damage);
        TypeOFActionToAction.Add(TypeOfAction.apply_Heal, apply_Heal);
        TypeOFActionToAction.Add(TypeOfAction.apply_SelfMove, apply_SelfMove);
        TypeOFActionToAction.Add(TypeOfAction.apply_TryEndTurn, apply_TryEndTurn);


        void apply_Damage()
        {
            BasicActionInProgress = false;
        }
        void apply_Heal()
        {
            BasicActionInProgress = false;
        }
        void apply_SelfMove()
        {
            Vector3Int currentPosition = universalCalculator.castAsV3Int(thisCharacter.transform.position);
            mapManager.UpdateCharacterPosistion(currentPosition, tryHere, thisCharacter);
            var ListOfMovePoints = new List<Vector3>();
            ListOfMovePoints.Add(tryHere);
            universalCalculator.MoveTransFromBetweenPoint(thisCharacter.transform, ListOfMovePoints, moveTimeSpeed);



            BasicActionInProgress = false;
        }
        void apply_TryEndTurn()
        {
            if (characterCS.doActionPointsRemainAfterAbility() == false)
            {
                CharacterControllerScript targetCharacter = thisCharacter.gameObject.GetComponent<CharacterControllerScript>();
                targetCharacter.ToggleCharacterTurnAnimation(false);
                buttonManager.clearButtons();
                this.GetComponent<TurnManager>().endTurn();
            }
            BasicActionInProgress = false;
        }
        void DoubleTeam()
        { characterCS.actionPoints += 1; }
        void AxeSweep()
        {
            //StartCoroutine(getInput(simpleAoeAttackAction, AbilityName.AxeSweep));
        }
        void Restart()
        {
            GameEvents.current.reloadScene();
            //Debug.Log("Restart Function not created");
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
            GameEvents.current.PlaySound(0);//This is For attacking
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
            GameEvents.current.PlaySound(1);//This is For attacking bigg
        }
        void checkCharacters(CharacterControllerScript targetCharacter)
        {
            targetCharacter.CheckIfCharacterIsDead();
        }

    }
    void addToolTip(string Tip, bool resetTip = false)
    {
        if (resetTip == true)
            toolTip.text = "";
        toolTip.text += Tip;
    }
    bool BasicActionInProgress = false;

    public void doAction(CompundAbility compoundAbility)
    {
        StartCoroutine(BeginCompoundAbility(compoundAbility));
        IEnumerator BeginCompoundAbility(CompundAbility compoundAbility)
        {
            Debug.Log("Starting Compund Ability");
            foreach (BasicAction basicAction in compoundAbility.componentActions)
            {
                BasicActionInProgress = true;
                StartCoroutine(getInput(basicAction));
                yield return new WaitUntil(() => !BasicActionInProgress);
            }
            //End Turn Dialog
            Debug.LogError("Ending Compund Ability but it is not implemented");
        }

    }

    IEnumerator getInput(BasicAction basicAction)
    {
        //Declaring Variables
        float rangeOfAction = ((int)basicAction.rangeOfActionEnum);
        if (rangeOfAction == 0)
            Debug.Log(rangeOfAction + " was zero");
        List<Vector3Int> listOfValidtargets = getValidTargetList(basicAction);
        reticalManager.fromPoint = characterCS.getCharV3Int();
        ShouldContinue = false;

        //Executing Script
        if (!characterCS.controlCharacter)//if Non Player Character
        {
            tryHere = characterCS.getTarget(listOfValidtargets);
            ShouldContinue = true;
            yield return new WaitForSeconds(0.25f);
        }
        else//if it is the player character
        {
            reticalManager.reDrawValidTiles(listOfValidtargets);//this sets the Valid Tiles Overlay
            reticalManager.reticalShapes = basicAction.areaOfEffectType;
            reticalManager.rangeOfAction = rangeOfAction;
            addToolTip("select Purple Tile To Contine with Action " + basicAction.typeOfAction + " Or Right Click to Cancel", true);
            yield return new WaitUntil(() => CheckContinue());//this waits for MB0 or MB1         
            tryHere = reticalManager.getMovePoint();
            reticalManager.reticalShapes = ReticalShapes.SSingle;
        }
        if (CheckMovePoint())//if Getting tryHere was at a Valid Tile
        {
            GameObject objectCharacter = thisCharacter;
            if (mapManager.cellDataDir[tryHere].characterAtCell != null)
                objectCharacter = mapManager.cellDataDir[tryHere].characterAtCell;
            GameEvents.current.sendChoice(thisCharacter, basicAction.typeOfAction, objectCharacter);
            //Send Events Regarding Choice
            yield return new WaitUntil(() => !GameEvents.current.EventInMotion);

            TypeOFActionToAction[basicAction.typeOfAction]();
        }
        reticalManager.reDrawValidTiles();

        //reticalManager.reDrawShadows();
        //Methods
        bool CheckContinue()
        {
            if (Input.GetMouseButtonDown(0))
            {
                addToolTip("Select an Button To Perform an Action", true);
                ShouldContinue = true;
                return true;

            }
            else if (Input.GetMouseButtonDown(1))
            {
                addToolTip("Action Cancelled; Select an Button To Perform an Action", true);
                ShouldContinue = false;
                return true;
            }
            return false;
        }
        bool CheckMovePoint()
        {
            if (ShouldContinue && listOfValidtargets.Contains(tryHere) && listOfValidtargets.Count != 0)
            {

                return true;
            }
            else if (listOfValidtargets.Count == 0)
            {
                addToolTip("No Valid Tiles for This Action; Select an Button To Perform an Action", true);
                //Debug.Log("No Valid Tiles Exist; Ending GetData; Debugging Just in Case;");
                getValidTargetList(basicAction);
                return false;
            }
            else
                return false;
        }
    }
    public List<Vector3Int> getValidTargetList(BasicAction action)
    {
        float rangeOfAction;
        if (!EditMapMode)
            rangeOfAction = ((int)action.rangeOfActionEnum) / 10;
        else
            rangeOfAction = alternateRange;

        //Debug.Log("Generating List of valid Targets for the character" + thisCharacter.name);
        Vector3Int centerPos = universalCalculator.castAsV3Int(thisCharacter.transform.position);
        List<Vector3Int> listOfRanges = universalCalculator.generateTaxiRangeFromPoint(centerPos, rangeOfAction);
        List<Vector3Int> listOfNonNullTiles = new List<Vector3Int>(mapManager.cellDataDir.Keys);
        bool disregardWalkablity = false;
        bool requireCharacter = false;
        if (action.validTileType == ValidTileType.PointTargeted)
        {
            listOfRanges = universalCalculator.filterOutList(listOfRanges, listOfNonNullTiles);
            disregardWalkablity = true;
        }
        if (action.validTargets == ValidTargets.LivingEntities)
        {
            requireCharacter = true;
        }
        listOfRanges.Remove(centerPos);
        ValidTargetListDebugInfo = "Data for Invalid Tiles \n";
        //The Following Removes Invalid Tiles
        for (int i = 0; i < listOfRanges.Count; i++)
        {
            //Normal Checks         
            bool hasWalkability = disregardWalkablity ? true : mapManager.checkAtPosIfCharacterCanWalk(listOfRanges[i], characterCS);
            bool requireCharacterCondition = requireCharacter ? mapManager.isCellHoldingCharacer(listOfRanges[i]) : true;
            //bool requireCharacterCondition = GlobalCal.compareBool(mapManager.isCellHoldingCharacer(listOfRanges[i]), action.requireCharacterBoolEnum);
            if (hasWalkability && requireCharacterCondition)
            {/*Do Nothing since all conditions are fine*/}
            else
            {
                //For Debugging
                if (checkValidActionTiles)
                {
                    bool condtion = false;//Will be reassigned later                        
                    string debugLine = "For Action " + action.typeOfAction + " Point " + listOfRanges[i] + " was Invalid as Tile ";
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
    AxeSweep,
    Restart
}
[Serializable]
public class CompundAbility
{
    public string NameOfAbility;
    public List<BasicAction> componentActions;
    public CompundAbility()
    {

    }
    public CompundAbility(CompundAbility given)
    {
        this.NameOfAbility = given.NameOfAbility;
        componentActions = GlobalCal.createCopyListUsingConstructor(given.componentActions);
    }

}
[Serializable]
public class BasicAction
{
    public TypeOfAction typeOfAction;
    public RangeOfActionEnum rangeOfActionEnum;
    public ReticalShapes areaOfEffectType;
    public ValidTileType validTileType;
    public ValidTargets validTargets;
    public BasicAction()
    {

    }
    public BasicAction(BasicAction given)
    {
        typeOfAction = given.typeOfAction;
        rangeOfActionEnum = given.rangeOfActionEnum;
        areaOfEffectType = given.areaOfEffectType;
        validTileType = given.validTileType;
        validTargets = given.validTargets;
    }

}
public enum TypeOfAction
{
    apply_Damage,
    apply_Heal,
    apply_SelfMove,
    apply_TryEndTurn
}
public enum BoolEnum
{
    TrueOrFalse,
    True,
    False
}
public enum RangeOfActionEnum
{
    r0 = 0,
    r10 = 10,
    r15 = 15,
    r20 = 20,
    r25 = 25,
    r30 = 30
}
public enum ValidTileType
{
    PointTargeted,
    UnitTargeted,
    EmptyCellTargeted
}
public enum ValidTargets
{
    All,
    Enemies,
    Allies,
    LivingEntities,
    NonLivingEntities
}