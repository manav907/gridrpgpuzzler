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
    Dictionary<CompundAbilityPreset, Action> AbilityPresets;
    void SetMoveDictionary()
    {
        abilityNameToAction = new Dictionary<AbilityName, Action>();
        abilityNameToAction.Add(AbilityName.DoubleTeam, DoubleTeam);
        abilityNameToAction.Add(AbilityName.AxeSweep, AxeSweep);
        abilityNameToAction.Add(AbilityName.Restart, Restart);

        AbilityPresets = new Dictionary<CompundAbilityPreset, Action>();

        AbilityPresets.Add(CompundAbilityPreset.Move, Move);
        void Move()
        {
            StartCoroutine(orderOfEvents());
            IEnumerator orderOfEvents()
            {
                foreach (var pointsToMove in listOfPointsForCompundAction)
                {
                    yield return new WaitForSeconds(0.25f);
                    apply_SelfMove(pointsToMove);
                }
            }

        }



        void apply_Damage()
        {
            //BasicActionInProgress = false;
        }
        void apply_Heal()
        {
            //BasicActionInProgress = false;
        }
        void apply_SelfMove(List<Vector3Int> movePoints)
        {
            Vector3Int currentPosition = universalCalculator.castAsV3Int(thisCharacter.transform.position);
            mapManager.UpdateCharacterPosistion(currentPosition, tryHere, thisCharacter);
            universalCalculator.MoveTransFromBetweenPoint(thisCharacter.transform, universalCalculator.castListAsV3(movePoints), moveTimeSpeed);
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
            //BasicActionInProgress = false;
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
    bool CompundActionInProgress = false;
    CompundAbility currentAbility;
    Vector3Int theroticalCurrentPos;

    public void doAction(CompundAbility compoundAbility)
    {
        currentAbility = compoundAbility;
        theroticalCurrentPos = universalCalculator.castAsV3Int(thisCharacter.transform.position);
        StartCoroutine(BeginCompoundAbility(compoundAbility));
        IEnumerator BeginCompoundAbility(CompundAbility compoundAbility)
        {
            stageOfAction = 0;
            listOfPointsForCompundAction = new List<List<Vector3Int>>();
            CompundActionInProgress = true;
            for (int i = 0; i < compoundAbility.actionInputParams.Count; i++)
            {
                if (!CompundActionInProgress)
                {
                    Debug.Log("CA was Broken");
                    break;
                }
                BasicActionInProgress = true;
                stageOfAction = i;

                ActionInputParams actionInputParams = compoundAbility.actionInputParams[i];

                StartCoroutine(getInput(actionInputParams));
                yield return new WaitUntil(() => !BasicActionInProgress);
                if (actionInputParams.updateTheroticalPos)
                {
                    theroticalCurrentPos = tryHere;//Rework This Section
                }
                yield return null;
                Debug.Log("Co loop end");
            }
            if (CompundActionInProgress)
            {
                AbilityPresets[compoundAbility.preset]();
                CompundActionInProgress = false;
            }
            //End Turn Dialog
            //Debug.LogError("Ending Compund Ability but it is not implemented");
        }

    }
    int stageOfAction;
    List<List<Vector3Int>> listOfPointsForCompundAction;

    IEnumerator getInput(ActionInputParams basicAction)
    {
        //Declaring Variables
        float rangeOfAction = basicAction.getRangeOfAction();
        if (rangeOfAction == 0)
            Debug.Log(rangeOfAction + " was zero");
        List<Vector3Int> listOfValidtargets = getValidTargetList(basicAction);
        reticalManager.fromPoint = theroticalCurrentPos;
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
            addToolTip("select Purple Tile To Contine with Action " + basicAction + " Or Right Click to Cancel", true);
            yield return new WaitUntil(() => CheckContinue());//this waits for MB0 or MB1         


            tryHere = reticalManager.getMovePoint();
            listOfPointsForCompundAction.Add(reticalManager.generateShape(tryHere));


            reticalManager.reticalShapes = ReticalShapes.SSingle;
        }
        if (CheckMovePoint() && CompundActionInProgress == true)//if Getting tryHere was at a Valid Tile
        {

            BasicActionInProgress = false;
            //TypeOFActionToAction[basicAction.typeOfAction]();
        }
        else
        {

            CompundActionInProgress = false;
            BasicActionInProgress = false;
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
                CompundActionInProgress = false;
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
    public List<Vector3Int> getValidTargetList(ActionInputParams action)
    {
        float rangeOfAction;
        if (!EditMapMode)
            rangeOfAction = action.getRangeOfAction();
        else
            rangeOfAction = alternateRange;

        //Debug.Log("Generating List of valid Targets for the character" + thisCharacter.name);
        Vector3Int centerPos = theroticalCurrentPos;
        List<Vector3Int> listOfRanges = universalCalculator.generateTaxiRangeFromPoint(centerPos, rangeOfAction);
        List<Vector3Int> listOfNonNullTiles = new List<Vector3Int>(mapManager.cellDataDir.Keys);
        bool disregardWalkablity = false;
        bool requireCharacter = false;
        listOfRanges = universalCalculator.filterOutList(listOfRanges, listOfNonNullTiles);
        if (action.validTileType == ValidTileType.PointTargeted)
        {

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
                    string debugLine = "For Action " + action + " Point " + listOfRanges[i] + " was Invalid as Tile ";
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
    public CompundAbilityPreset preset;
    public List<ActionInputParams> actionInputParams;
    public CompundAbility()
    {

    }
    public CompundAbility(CompundAbility given)
    {
        this.NameOfAbility = given.NameOfAbility;
        actionInputParams = GlobalCal.createCopyListUsingConstructor(given.actionInputParams);
    }

}
[Serializable]
public class ActionInputParams
{
    [SerializeField] RangeOfActionEnum rangeOfActionEnum;
    public ReticalShapes areaOfEffectType;
    public ValidTileType validTileType;
    public ValidTargets validTargets;
    public bool updateTheroticalPos = true;
    public ActionInputParams()
    {

    }
    public ActionInputParams(ActionInputParams given)
    {
        rangeOfActionEnum = given.rangeOfActionEnum;
        areaOfEffectType = given.areaOfEffectType;
        validTileType = given.validTileType;
        validTargets = given.validTargets;
    }
    public float getRangeOfAction()
    {
        return (float)rangeOfActionEnum / 10;
    }

}
public enum CompundAbilityPreset
{
    Move,
    Attack,

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