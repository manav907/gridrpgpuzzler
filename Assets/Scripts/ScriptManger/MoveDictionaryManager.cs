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
    [SerializeField] string currentAbiltyName;
    public Dictionary<Vector3Int, List<List<Vector3Int>>> generateAbiltyPointMap(AbilityData abilityData, Vector3Int fromPoint)
    {
        var pointsToScan = generateAreaForTileToEffectPair(abilityData.AreaOfAbility, fromPoint, fromPoint);
        Dictionary<Vector3Int, List<List<Vector3Int>>> pointToScannedAreas = new Dictionary<Vector3Int, List<List<Vector3Int>>>();
        foreach (Vector3Int point in pointsToScan)
        {
            List<List<Vector3Int>> list = new List<List<Vector3Int>>();
            foreach (TileToEffectPair TileDataPair in abilityData.ValidTileData)
            {
                list.Add(generateAreaForTileToEffectPair(TileDataPair, fromPoint, point));
            }
            pointToScannedAreas[point] = list;
        }
        return pointToScannedAreas;
    }
    public AbilityData currnetAbility;
    public void doAction(AbilityData abilityData)
    {
        currentAbiltyName = abilityData.name;

        int costOfaction = abilityData.costOfaction;
        if (characterCS.actionPoints < costOfaction)
        {
            addToolTip("The Ability " + currentAbiltyName + " cannot be used as you dont have action Points Remaining " + characterCS.actionPoints);
            return;
        }
        if (abilityData.ValidTileData.Count != abilityData.ApplyEffects.Count)
            Debug.Log("Great Erros");
        var pointMap = generateAbiltyPointMap(abilityData, characterCS.getCharV3Int());
        if (pointMap.Keys.Count == 0)
        {
            addToolTip("The Ability " + currentAbiltyName + " cannot be used as No Valid Tilees");
            Debug.Log("No Valid Tilees");
            return;
        }
        currnetAbility = abilityData;
        reticalManager.UpdateReticalInputParams(pointMap);
        setGetInputCoRoutineState(CoRoutineStateCheck.Proceeding);
        StartCoroutine(SequenceOfEvents());
        IEnumerator SequenceOfEvents()
        {
            yield return StartCoroutine(getInput(pointMap.Keys.ToList()));
            if (pointMap.ContainsKey(tryHere))
                for (int i = 0; i < abilityData.ApplyEffects.Count; i++)
                {
                    yield return StartCoroutine(AnimationCoRoutione(pointMap[tryHere][i], abilityData.ApplyEffects[i], characterCS.getCharV3Int(), tryHere));
                }
            else
                setGetInputCoRoutineState(CoRoutineStateCheck.Aborting);

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
    IEnumerator getInput(List<Vector3Int> validInputs)
    {
        //Declaring Variables        
        reticalManager.reDrawValidTiles(validInputs, validInputs);//this sets the Valid Tiles Overlay
        ShouldContinue = false;
        //Executing Script
        if (!characterCS.controlCharacter)//if Non Player Character
        {
            tryHere = characterCS.getTarget(currnetAbility);
            ShouldContinue = true;
            yield return new WaitForSeconds(UserDataManager.waitAI);

        }
        else//if it is the player character
        {
            addToolTip("Select Purple Tile To Contine with  " + currentAbiltyName + " Or Right Click to Cancel");
            setGetInputCoRoutineState(CoRoutineStateCheck.Waiting);
            yield return new WaitUntil(() => CheckContinue());//this waits for MB0 or MB1         
            tryHere = (reticalManager.getMovePoint());
        }
        if (CheckMovePoint())//if Getting tryHere was at a Valid Tile
        {
            setGetInputCoRoutineState(CoRoutineStateCheck.Proceeding);
        }
        else
        {
            setGetInputCoRoutineState(CoRoutineStateCheck.Misinput);
        }
        reticalManager.reDrawValidTiles(new List<Vector3Int>(), new List<Vector3Int>());
        reticalManager.ResetReticalInputParams();
        //Methods
        bool CheckContinue()
        {
            if (Input.GetMouseButtonDown(0))
            {
                addToolTip("Select an Button To Perform an Action");
                setGetInputCoRoutineState(CoRoutineStateCheck.Waiting);
                ShouldContinue = true;
                return true;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                addToolTip(currentAbiltyName + " Cancelled; Select an Button To Perform an Action");
                setGetInputCoRoutineState(CoRoutineStateCheck.Aborting);
                ShouldContinue = false;
                return true;
            }
            setGetInputCoRoutineState(CoRoutineStateCheck.Waiting);
            return false;
        }
        bool CheckMovePoint()
        {
            if (ShouldContinue && validInputs.Contains(tryHere))
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
    public List<Vector3Int> generateAreaForTileToEffectPair(TileToEffectPair tileToEffectPair, Vector3Int fromPoint, Vector3Int AtPoint)
    {
        string TestDebugString = "Creating Area For " + tileToEffectPair + " with AoeStyle" + tileToEffectPair.aoeStyle;
        var output = GlobalCal.generateArea(tileToEffectPair.aoeStyle, fromPoint, AtPoint, tileToEffectPair.getRangeOfAction());
        PrintOutputStatus();
        //output = mapManager.filterListWithTileRequirements(fromPoint, output, tileToEffectPair.TileRequirements);
        //output = mapManager.filterListWithWalkRequirements(fromPoint, output, characterCS.canWalkOn);
        //output = CheckVectorValidity(fromPoint, output, tileToEffectPair.targetType);
        Debug.Log(TestDebugString);
        return output;
        void PrintOutputStatus()
        {
            //TestDebugString += "\n " + output.Count;
            foreach (var point in output)
                TestDebugString += "\n      :" + point;
        }
        List<Vector3Int> CheckVectorValidity(Vector3Int fromPoint, List<Vector3Int> listOfRanges, TargetType targetType)
        {
            //Direction and Distance are very Important Check For those if you are having problems
            var normalizedDirectionToTilePos = universalCalculator.DirectionToCellSnapData(fromPoint, listOfRanges);
            var listOfVectorRanges = new List<Vector3Int>();
            if (targetType == TargetType.AnyValid)
            {
                return listOfRanges;
            }
            else
                foreach (var direction in normalizedDirectionToTilePos)
                {
                    if (targetType == TargetType.FirstValid)
                    {
                        listOfVectorRanges.Add(direction.Value.First());
                    }
                    else if (targetType == TargetType.LastValid)
                    {
                        listOfVectorRanges.Add(direction.Value.Last());
                    }
                }
            listOfRanges = listOfVectorRanges;
            return listOfRanges;

        }
    }
    IEnumerator AnimationCoRoutione(List<Vector3Int> points, ActionEffectParams actionEffectParams, Vector3Int fromPoint, Vector3Int atPoint)
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
        TypeOfAction actiontype = actionEffectParams.typeOfAction;
        AnimationMovementType animationMovementType = actionEffectParams.animationMovementType;
        AnimationLoopType animationLoopType = actionEffectParams.loopType;
        IEnumerator animationActionFunction()
        {
            Vector3 targetLocation = tryHere;
            if (animationMovementType == AnimationMovementType.NudgeToPoint)
            {
                Vector3 point1 = fromPoint;
                Vector3 point2 = atPoint;
                float distanceFactor = 0.3f;  // Adjust this value to control the distance from point1

                Vector3 direction = (point2 - point1).normalized;  // Calculate the direction between the points
                Vector3 midPoint = point1 + direction * distanceFactor * Vector3.Distance(point1, point2);
                targetLocation = midPoint;


            }
            else if (animationMovementType == AnimationMovementType.NoMovement)
            {
                targetLocation = fromPoint;
            }

            yield return StartCoroutine(TransformAnimationScript.current.MoveUsingQueueSystem(thisCharacter.transform, targetLocation, moveTimeSpeed));
            yield return StartCoroutine(characterCS.animationControllerScript.trySetNewAnimation(actionEffectParams.doActionTillKeyFrameAnimation));

        }
        IEnumerator afterAnimationOfAction()
        {
            StartCoroutine(TransformAnimationScript.current.MoveUsingQueueSystem(thisCharacter.transform, characterCS.getCharV3Int(), moveTimeSpeed));
            StartCoroutine(characterCS.animationControllerScript.trySetNewAnimation(CharacterAnimationState.Idle));
            if (!UserDataManager.skipWaitTime)
                yield return new WaitForSeconds(UserDataManager.waitAction);
        }

        if (animationLoopType == AnimationLoopType.forAction)
        {
            yield return StartCoroutine(animationActionFunction());
        }
        foreach (Vector3Int point in points)
        {
            tryHere = point;
            if (animationLoopType == AnimationLoopType.forEachPoint)
                yield return StartCoroutine(animationActionFunction());
            abilityNameToAction[actiontype]();//The Actual Action

        }
        yield return afterAnimationOfAction();
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
