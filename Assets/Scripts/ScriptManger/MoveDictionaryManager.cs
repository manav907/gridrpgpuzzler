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
    public bool ControlAI = false;

    bool ShouldContinue;
    public void setVariables()
    {
        turnManager = this.GetComponent<TurnManager>();
        reticalManager = this.GetComponent<ReticalManager>();
        mapManager = this.GetComponent<MapManager>();
        universalCalculator = this.GetComponent<UniversalCalculator>();

        SetMoveDictionary();
    }
    public void getThisCharacterData()
    {
        thisCharacter = TurnManager.thisCharacter;
        characterCS = thisCharacter.GetComponent<CharacterControllerScript>();
        theroticalCurrentPos = characterCS.getCharV3Int();
        reticalManager.fromPoint = theroticalCurrentPos;
    }
    Dictionary<TypeOfAction, Action> abilityNameToAction;

    void SetMoveDictionary()
    {
        abilityNameToAction = new Dictionary<TypeOfAction, Action>();
        abilityNameToAction.Add(TypeOfAction.apply_SelfMove, apply_SelfMove);
        abilityNameToAction.Add(TypeOfAction.apply_Damage, apply_Damage);
        abilityNameToAction.Add(TypeOfAction.apply_Heal, apply_Heal);

        void apply_Damage()
        {
            CharacterControllerScript targetCharacter = mapManager.cellDataDir[tryHere].characterAtCell.GetComponent<CharacterControllerScript>();
            CharacterControllerScript attackingCharacter = thisCharacter.GetComponent<CharacterControllerScript>();
            targetCharacter.health -= attackingCharacter.attackDamage;
            checkCharacters(targetCharacter);
            GameEvents.current.PlaySound(0);
        }
        void apply_Heal()
        {
            //BasicActionInProgress = false;
        }
        void apply_SelfMove()
        {
            Vector3Int currentPosition = thisCharacter.GetComponent<CharacterControllerScript>().getCharV3Int();
            mapManager.UpdateCharacterPosistion(currentPosition, tryHere, thisCharacter);
        }
        /* void DoubleTeam())
        { characterCS.actionPoints += 1; } */
        void checkCharacters(CharacterControllerScript targetCharacter)
        {
            targetCharacter.CheckIfCharacterIsDead();
        }
    }

    void addToolTip(string Tip)
    {
        GameEvents.current.inGameUI.setTip(Tip);
    }
    [SerializeField] Vector3Int theroticalCurrentPos;
    int stageOfAction;

    string currentVarirable;
    Dictionary<string, List<Vector3Int>> variableNameToData;
    public void doAction(LadderCollapseFunction ladderCollapseFunction)
    {
        int costOfaction = 0;
        costOfaction = characterCS.abilityToCost.returnDict()[ladderCollapseFunction];
        if (characterCS.actionPoints < costOfaction)
        {
            addToolTip("The Ability " + ladderCollapseFunction.name + " cannot be used as you dont have action Points Remaining " + characterCS.actionPoints);
            return;
        }
        theroticalCurrentPos = characterCS.getCharV3Int();
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
                    yield return StartCoroutine(getInput(ladderCollapseFunction.SetDataAtIndex[currentsetDataWithID]));
                    currentsetDataWithID++;
                }
                else if (keyPair.key == LadderCollapseFunctionEnums.doActionWithID)
                {
                    float startTime = Time.time;
                    float lastTime = Time.time;
                    debugTime(false);
                    void debugTime(bool debugit = true)
                    {
                        if (!debugit)
                            return;
                        float currentTime = Time.time;
                        float deltaTime = currentTime - lastTime;

                        Debug.Log("Delta Time: " + deltaTime);
                        lastTime = currentTime;
                    }
                    //
                    GameEvents.current.inGameUI.ClearButtons();//Clearing Buttons while action is in progress
                    ActionEffectParams actionEffectParams = ladderCollapseFunction.DoActionFromDataAtIndex[currentdoActionWithID];
                    TypeOfAction actiontype = actionEffectParams.typeOfAction;
                    AnimationMovementType animationMovementType = actionEffectParams.animationMovementType;
                    AnimationLoopType animationLoopType = actionEffectParams.loopType;
                    IEnumerator animationActionFunction()
                    {
                        Vector3 targetLocation = tryHere;
                        if (animationMovementType == AnimationMovementType.NudgeToPoint)
                        {
                            Vector3 point1 = theroticalCurrentPos;
                            Vector3 point2 = tryHere;
                            float distanceFactor = 0.3f;  // Adjust this value to control the distance from point1

                            Vector3 direction = (point2 - point1).normalized;  // Calculate the direction between the points
                            Vector3 midPoint = point1 + direction * distanceFactor * Vector3.Distance(point1, point2);
                            targetLocation = midPoint;


                        }
                        else if (animationMovementType == AnimationMovementType.NoMovement)
                        {
                            targetLocation = theroticalCurrentPos;
                        }

                        yield return StartCoroutine(TransformAnimationScript.current.MoveUsingQueueSystem(thisCharacter.transform, targetLocation, moveTimeSpeed));
                        yield return StartCoroutine(characterCS.animationControllerScript.setAnimationAndWaitForIt(actionEffectParams.doActionTillKeyFrameAnimation));

                    }
                    IEnumerator afterAnimationOfAction()
                    {
                        theroticalCurrentPos = characterCS.getCharV3Int();
                        StartCoroutine(TransformAnimationScript.current.MoveUsingQueueSystem(thisCharacter.transform, theroticalCurrentPos, moveTimeSpeed));
                        StartCoroutine(characterCS.animationControllerScript.setAnimationAndWaitForIt(CharacterAnimationState.Idle, false));
                        if (!UserDataManager.skipWaitTime)
                            yield return new WaitForSeconds(UserDataManager.waitAction);
                    }

                    if (animationLoopType == AnimationLoopType.forAction)
                    {
                        yield return StartCoroutine(animationActionFunction());
                    }
                    foreach (Vector3Int point in variableNameToData[currentVarirable])
                    {
                        tryHere = point;
                        if (animationLoopType == AnimationLoopType.forEachPoint)
                            yield return StartCoroutine(animationActionFunction());
                        abilityNameToAction[actiontype]();//The Actual Action

                    }
                    yield return afterAnimationOfAction();



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
                characterCS.actionPoints = characterCS.actionPoints - costOfaction;
                if (characterCS.doActionPointsRemainAfterAbility())
                    characterCS.BeginThisCharacterTurn();
                else
                {
                    turnManager.endTurn();
                }
                //abilityNameToAction[TypeOfAction.apply_TryEndTurn]();
                ///Debug.LogError("Hey End Turn is Disableed");
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
    IEnumerator getInput(ActionInputParams actionInputParams)
    {
        //Declaring Variables
        float rangeOfAction = actionInputParams.getRangeOfAction();
        float magnititudeOfAction = actionInputParams.getMagnititudeOfAction();
        if (rangeOfAction == 0)
            Debug.Log(rangeOfAction + " was zero");
        yield return null;
        List<Vector3Int> listOfValidtargets = getValidTargetList(actionInputParams, theroticalCurrentPos);
        reticalManager.fromPoint = theroticalCurrentPos;
        ShouldContinue = false;

        List<Vector3Int> tempData = new List<Vector3Int>();
        //SettingUPReticle
        reticalManager.UpdateReticalInputParams(actionInputParams, listOfValidtargets);
        //Executing Script
        if (!characterCS.controlCharacter)//if Non Player Character
        {
            reticalManager.reDrawValidTiles(listOfValidtargets);
            tryHere = characterCS.getTarget(actionInputParams);
            ShouldContinue = true;
            yield return new WaitForSeconds(UserDataManager.waitAI);

        }
        else//if it is the player character
        {
            reticalManager.reDrawValidTiles(listOfValidtargets);//this sets the Valid Tiles Overlay
            addToolTip("select Purple Tile To Contine with Action " + actionInputParams + " Or Right Click to Cancel");
            setGetInputCoRoutineState(CoRoutineStateCheck.Waiting);
            yield return new WaitUntil(() => CheckContinue());//this waits for MB0 or MB1         
            tryHere = (reticalManager.getMovePoint());
        }
        if (CheckMovePoint())//if Getting tryHere was at a Valid Tile
        {


            if (reticalManager.ValidPosToShapeData.ContainsKey(tryHere))
            { tempData = reticalManager.ValidPosToShapeData[tryHere]; }
            else
            { Debug.Log("InvalidTile"); }
            if (actionInputParams.updateTheroticalPos)
            {
                theroticalCurrentPos = tempData.Last();
                tryHere = theroticalCurrentPos;
            }
            foreach (Vector3Int pos in tempData)
            {
                if (CheckIfTargetis(pos, actionInputParams.validTargets))
                    variableNameToData[currentVarirable].Add(pos);
            }
            setGetInputCoRoutineState(CoRoutineStateCheck.Proceeding);
        }
        else
        {
            setGetInputCoRoutineState(CoRoutineStateCheck.Misinput);
        }
        reticalManager.reDrawValidTiles();
        reticalManager.ResetReticalInputParams();
        //Methods
        bool CheckContinue()
        {
            if (listOfValidtargets.Count == 0)
            {
                addToolTip("No Valid Tiles for This Action; Select an Button To Perform an Action");
                setGetInputCoRoutineState(CoRoutineStateCheck.Aborting);
                //Debug.Log("No Valid Tiles Exist; Ending GetData; Debugging Just in Case;");
                //getValidTargetList(basicAction);
                ShouldContinue = false;
                return true;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                addToolTip("Select an Button To Perform an Action");
                setGetInputCoRoutineState(CoRoutineStateCheck.Waiting);
                ShouldContinue = true;
                return true;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                addToolTip("Action Cancelled; Select an Button To Perform an Action");
                setGetInputCoRoutineState(CoRoutineStateCheck.Aborting);
                ShouldContinue = false;
                return true;
            }
            setGetInputCoRoutineState(CoRoutineStateCheck.Waiting);
            return false;
        }
        bool CheckMovePoint()
        {
            if (ShouldContinue && listOfValidtargets.Contains(tryHere))
            {
                return true;
            }
            if (!characterCS.controlCharacter)
            {
                Debug.LogError("AI Exception");
                turnManager.endTurn();
            }
            return false;
        }

    }
    public bool CheckIfTargetis(Vector3Int checkPos, ValidTargets validTargets)
    {
        ValidTargets requitedCondtion = validTargets;
        if (mapManager.cellDataDir.ContainsKey(checkPos))
        {
            if (requitedCondtion == ValidTargets.Empty)
            {
                return !mapManager.isCellHoldingCharacer(checkPos);
            }
            else if (mapManager.isCellHoldingCharacer(checkPos))
            {
                string faction = mapManager.cellDataDir[checkPos].characterAtCell.GetComponent<CharacterControllerScript>().faction;
                string factionOfCaster = thisCharacter.GetComponent<CharacterControllerScript>().faction;
                //Debug.Log("Checking Factions between " + faction + " and " + factionOfCaster + " for condition " + requitedCondtion);
                //Debug.Log(faction == factionOfCaster);
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
    public List<Vector3Int> getValidTargetList(ActionInputParams action, Vector3Int fromPoint)
    {
        float rangeOfAction;
        rangeOfAction = action.getRangeOfAction();
        List<Vector3Int> listOfRanges = universalCalculator.generateTaxiRangeFromPoint(fromPoint, rangeOfAction);
        listOfRanges = universalCalculator.SortListAccordingtoDistanceFromPoint(listOfRanges, fromPoint);//Sorts Points Accoding to Distance
        //List<Vector3Int> listOfRanges = universalCalculator.generateDirectionalRange(centerPos, rangeOfAction, universalCalculator.generate9WayReffence());

        bool checkWalkability = true;
        bool requireCharacter = false;
        bool reverseRequireCharacterCondiditon = false;
        if (action.ignoreValidTargetsCheck)
        {
            checkWalkability = false;
            requireCharacter = false;
        }
        else if (action.validTargets == ValidTargets.Empty)
        {
            requireCharacter = true;
            reverseRequireCharacterCondiditon = true;
        }
        else// if (action.validTileType == ValidTileType.UnitTargeted)
        {
            requireCharacter = true;
        }
        if (!action.includeSelf)
        {
            listOfRanges.Remove(fromPoint);
        }
        //The Following Removes Invalid Tiles
        CheckTileValidity();
        CheckVectorValidity();
        if (action.includeSelf && !listOfRanges.Contains(fromPoint))
        {
            listOfRanges.Add(fromPoint);
            //Debug.Log("ForceAddedSelf");
        }
        return listOfRanges;
        void CheckVectorValidity()
        {
            //Direction and Distance are very Important Check For those if you are having problems
            var normalizedDirectionToTilePos = universalCalculator.DirectionToCellSnapData(fromPoint, listOfRanges);
            var listOfVectorRanges = new List<Vector3Int>();
            if (action.targetType == TargetType.AnyValid)
            {
                listOfVectorRanges = listOfRanges;
            }
            else
                foreach (var direction in normalizedDirectionToTilePos)
                {
                    if (action.targetType == TargetType.FirstValid)
                    {
                        listOfVectorRanges.Add(direction.Value.First());
                    }
                    else if (action.targetType == TargetType.LastValid)
                    {
                        listOfVectorRanges.Add(direction.Value.Last());
                    }
                }
            listOfRanges = listOfVectorRanges;

        }
        void CheckTileValidity()
        {
            for (int i = listOfRanges.Count - 1; i >= 0; i--)
            {
                bool requireCharacterCondition = requireCharacter ? mapManager.isCellHoldingCharacer(listOfRanges[i]) : true;
                if (reverseRequireCharacterCondiditon)
                {
                    requireCharacterCondition = !requireCharacterCondition;
                }

                if (!requireCharacterCondition || (checkWalkability && !mapManager.checkAtPosIfCharacterCanWalk(listOfRanges[i], characterCS)))
                {
                    listOfRanges.RemoveAt(i);
                }
            }
        }
    }
}


public enum CoRoutineStateCheck
{
    Waiting,
    Proceeding,
    Aborting,
    Misinput
}
public enum TypeOfAction
{
    apply_Damage,
    apply_Heal,
    apply_SelfMove,
}
public enum BoolEnum
{
    TrueOrFalse,
    True,
    False
}
