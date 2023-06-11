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
        theroticalCurrentPos = characterCS.getCharV3Int();
    }
    Dictionary<TypeOfAction, Action> abilityNameToAction;

    void SetMoveDictionary()
    {
        abilityNameToAction = new Dictionary<TypeOfAction, Action>();
        abilityNameToAction.Add(TypeOfAction.apply_SelfMove, apply_SelfMove);
        abilityNameToAction.Add(TypeOfAction.apply_Damage, apply_Damage);
        abilityNameToAction.Add(TypeOfAction.apply_Heal, apply_Heal);
        abilityNameToAction.Add(TypeOfAction.apply_TryEndTurn, apply_TryEndTurn);
        abilityNameToAction.Add(TypeOfAction.RestartScene, Restart);

        void apply_Damage()
        {
            CharacterControllerScript targetCharacter = mapManager.cellDataDir[tryHere].characterAtCell.GetComponent<CharacterControllerScript>();
            CharacterControllerScript attackingCharacter = thisCharacter.GetComponent<CharacterControllerScript>();
            targetCharacter.health -= attackingCharacter.attackDamage;
            checkCharacters(targetCharacter);
            var ListOfMovePoints = new List<Vector3>();
            ListOfMovePoints.Add(tryHere);
            ListOfMovePoints.Add(attackingCharacter.getCharV3Int());
            universalCalculator.MoveTransFromBetweenPoint(thisCharacter.transform, ListOfMovePoints, moveTimeSpeed);
            GameEvents.current.PlaySound(0);
        }
        void apply_Heal()
        {
            //BasicActionInProgress = false;
        }
        void apply_SelfMove()
        {
            var movePoints = new List<Vector3Int>();
            Vector3Int currentPosition = thisCharacter.GetComponent<CharacterControllerScript>().getCharV3Int();
            mapManager.UpdateCharacterPosistion(currentPosition, tryHere, thisCharacter);
            movePoints.Add(tryHere);
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
        }
        /* void DoubleTeam()
        { characterCS.actionPoints += 1; } */
        void Restart()
        {
            GameEvents.current.reloadScene();
            //Debug.Log("Restart Function not created");
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
    Vector3Int theroticalCurrentPos;
    int stageOfAction;

    string currentVarirable;
    Dictionary<string, List<Vector3Int>> variableNameToData;
    public void doAction(LadderCollapseFunction ladderCollapseFunction)
    {
        theroticalCurrentPos = universalCalculator.castAsV3Int(thisCharacter.transform.position);
        setGetInputCoRoutineState(CoRoutineStateCheck.Proceeding);
        StartCoroutine(SequenceOfEvents());
        IEnumerator SequenceOfEvents()
        {
            variableNameToData = new Dictionary<string, List<Vector3Int>>();


            int currentsetDataWithID = 0;
            int currentdoActionWithID = 0;
            int currentSetDataUsingTherorticalPosAtArrayIndex = 0;



            foreach (var keyPair in ladderCollapseFunction.invokeFunction.KeyValuePairs)
            {
                if (GetInputState != CoRoutineStateCheck.Proceeding)
                {
                    //Debug.Log("Action was" + GetInputState);
                    //thisCharacter.GetComponent<CharacterControllerScript>().OnActionComplete(GetInputState);
                    break;
                }
                currentVarirable = keyPair.value;
                if (keyPair.key == LadderCollapseFunctionEnums.setDataWithID)
                {

                    variableNameToData.Add(currentVarirable, new List<Vector3Int>());
                    BasicActionInProgress = true;
                    StartCoroutine(getInput(ladderCollapseFunction.SetDataAtIndex[currentsetDataWithID]));
                    yield return new WaitUntil(() => !BasicActionInProgress);
                    yield return null;
                    currentsetDataWithID++;

                }
                else if (keyPair.key == LadderCollapseFunctionEnums.doActionWithID)
                {

                    TypeOfAction actiontype = ladderCollapseFunction.DoActionFromDataAtIndex[currentdoActionWithID].typeOfAction;
                    if (!ladderCollapseFunction.DoActionFromDataAtIndex[currentdoActionWithID].includeSelf)
                    {
                        variableNameToData[currentVarirable].Remove(thisCharacter.GetComponent<CharacterControllerScript>().getCharV3Int());
                    }
                    foreach (Vector3Int point in variableNameToData[currentVarirable])
                    {

                        tryHere = point;
                        if (DeterminValidTileTarget())
                        {
                            yield return new WaitForSeconds(0.25f);
                            abilityNameToAction[actiontype]();
                        }
                        bool DeterminValidTileTarget()
                        {

                            Debug.Log("Here");
                            if (mapManager.cellDataDir.ContainsKey(tryHere))
                            {
                                ValidTargets requitedCondtion = ladderCollapseFunction.DoActionFromDataAtIndex[currentdoActionWithID].validTargets;
                                if (requitedCondtion == ValidTargets.Empty)
                                {
                                    return !mapManager.isCellHoldingCharacer(tryHere);
                                }
                                else if (mapManager.isCellHoldingCharacer(tryHere))
                                {
                                    string faction = mapManager.cellDataDir[tryHere].characterAtCell.GetComponent<CharacterControllerScript>().faction;
                                    string factionOfCaster = thisCharacter.GetComponent<CharacterControllerScript>().faction;
                                    switch (requitedCondtion)
                                    {
                                        case ValidTargets.AnyFaction:
                                            {
                                                return true;
                                            }
                                        case ValidTargets.Enemies:
                                            {
                                                if (factionOfCaster != faction)
                                                    return true;
                                                else
                                                    return false;
                                            }
                                        case ValidTargets.Allies:
                                            {
                                                if (factionOfCaster == faction)
                                                    return true;
                                                else
                                                    return false;
                                            }

                                    }
                                }
                            }
                            return false;

                        }
                    }
                    currentdoActionWithID++;
                }
                else if (keyPair.key == LadderCollapseFunctionEnums.SetDataUsingTherorticalPosAtArrayIndex)
                {
                    currentSetDataUsingTherorticalPosAtArrayIndex++;
                    variableNameToData[currentVarirable] = new List<Vector3Int>();
                    variableNameToData[currentVarirable].Add(theroticalCurrentPos);
                }
            }
            if (GetInputState == CoRoutineStateCheck.Proceeding)
            {
                abilityNameToAction[TypeOfAction.apply_TryEndTurn]();
            }
            yield return null;
        }
    }
    List<List<Vector3Int>> listOfPointsForCompundAction;
    CoRoutineStateCheck GetInputState;
    void setGetInputCoRoutineState(CoRoutineStateCheck newState)
    {

        if (newState == CoRoutineStateCheck.Misinput)
        {
            if (GetInputState == CoRoutineStateCheck.Aborting)
            {
                newState = CoRoutineStateCheck.Aborting;
            }
        }
        if (newState != CoRoutineStateCheck.Waiting)
        {
            // Debug.Log(newState);
        }
        GetInputState = newState;
    }
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
            variableNameToData[currentVarirable] = reticalManager.generateShape(tryHere);
            ShouldContinue = true;
            yield return new WaitForSeconds(0.25f);
        }
        else//if it is the player character
        {
            reticalManager.reDrawValidTiles(listOfValidtargets);//this sets the Valid Tiles Overlay
            reticalManager.reticalShapes = basicAction.areaOfEffectType;
            reticalManager.rangeOfAction = rangeOfAction;
            addToolTip("select Purple Tile To Contine with Action " + basicAction + " Or Right Click to Cancel", true);

            setGetInputCoRoutineState(CoRoutineStateCheck.Waiting);
            yield return new WaitUntil(() => CheckContinue());//this waits for MB0 or MB1         

            tryHere = reticalManager.getMovePoint();

            variableNameToData[currentVarirable] = reticalManager.generateShape(tryHere);
            if (basicAction.updateTheroticalPos)
            {
                theroticalCurrentPos = tryHere;
            }


            reticalManager.reticalShapes = ReticalShapes.SSingle;
        }
        if (CheckMovePoint())//if Getting tryHere was at a Valid Tile
        {

            BasicActionInProgress = false;
            setGetInputCoRoutineState(CoRoutineStateCheck.Proceeding);
        }
        else
        {

            setGetInputCoRoutineState(CoRoutineStateCheck.Misinput);
            BasicActionInProgress = false;
        }
        reticalManager.reDrawValidTiles();
        //Methods
        bool CheckContinue()
        {
            if (Input.GetMouseButtonDown(0))
            {
                addToolTip("Select an Button To Perform an Action", true);
                setGetInputCoRoutineState(CoRoutineStateCheck.Waiting);
                ShouldContinue = true;
                return true;

            }
            else if (Input.GetMouseButtonDown(1))
            {
                addToolTip("Action Cancelled; Select an Button To Perform an Action", true);
                setGetInputCoRoutineState(CoRoutineStateCheck.Aborting);
                ShouldContinue = false;
                return true;
            }
            setGetInputCoRoutineState(CoRoutineStateCheck.Waiting);
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
                setGetInputCoRoutineState(CoRoutineStateCheck.Aborting);
                //Debug.Log("No Valid Tiles Exist; Ending GetData; Debugging Just in Case;");
                getValidTargetList(basicAction);
                return false;
            }
            else
            {
                //setGetInputCoRoutineState(CoRoutineStateCheck.Misinput);
                return false;
            }
        }
    }
    public List<Vector3Int> getValidTargetList(ActionInputParams action)
    {
        float rangeOfAction;
        if (!EditMapMode)
            rangeOfAction = action.getRangeOfAction();
        else
            rangeOfAction = alternateRange;

        Vector3Int centerPos = theroticalCurrentPos;
        List<Vector3Int> listOfRanges = universalCalculator.generateTaxiRangeFromPoint(centerPos, rangeOfAction);
        List<Vector3Int> listOfNonNullTiles = new List<Vector3Int>(mapManager.cellDataDir.Keys);
        bool disregardWalkablity = false;
        bool requireCharacter = false;
        bool reverseRequireCharacterCondiditon = false;
        listOfRanges = universalCalculator.filterOutList(listOfRanges, listOfNonNullTiles);
        if (action.validTileType == ValidTileType.PointTargeted)
        {

            disregardWalkablity = true;
        }
        else if (action.validTileType == ValidTileType.EmptyCellTargeted)
        {

            requireCharacter = true;
            reverseRequireCharacterCondiditon = true;
        }
        else if (action.validTileType == ValidTileType.UnitTargeted)
        {
            requireCharacter = true;
        }
        //listOfRanges.Remove(centerPos);
        ValidTargetListDebugInfo = "Data for Invalid Tiles \n";
        //The Following Removes Invalid Tiles
        for (int i = 0; i < listOfRanges.Count; i++)
        {
            //Normal Checks         
            bool hasWalkability = disregardWalkablity ? true : mapManager.checkAtPosIfCharacterCanWalk(listOfRanges[i], characterCS);
            bool requireCharacterCondition = requireCharacter ? mapManager.isCellHoldingCharacer(listOfRanges[i]) : true;
            if (reverseRequireCharacterCondiditon)
            {
                requireCharacterCondition = !requireCharacterCondition;
            }
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
public enum CoRoutineStateCheck
{
    Waiting,
    Proceeding,
    Aborting,
    Misinput
}

public enum LadderCollapseFunctionEnums
{
    setDataWithID,
    doActionWithID,
    SetDataUsingTherorticalPosAtArrayIndex,
}
public enum TypeOfAction
{
    apply_Damage,
    apply_Heal,
    apply_SelfMove,
    apply_TryEndTurn,
    RestartScene
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
    Empty,
    AnyFaction,
    Enemies,
    Allies,
    Neutral
}