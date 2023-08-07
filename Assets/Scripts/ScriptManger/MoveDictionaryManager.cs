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
    private readonly float moveTimeSpeed = 0.12f;
    [Header("Character Data")]
    GameObject thisCharacter;
    CharacterControllerScript characterCS;
    [Header("Retical And Tile Data")]
    [SerializeField] private Vector3Int tryHere;
    [Header("Current Ablity")]
    //[SerializeField] Ability currentAblity;
    [Header("Debug Data")]
    public bool ControlAI = false;
    bool ShouldContinue = false;
    public void SetVariables()
    {
        turnManager = this.GetComponent<TurnManager>();
        reticalManager = this.GetComponent<ReticalManager>();
        mapManager = this.GetComponent<MapManager>();
        universalCalculator = this.GetComponent<UniversalCalculator>();
    }
    public void GetThisCharacterData()
    {
        thisCharacter = TurnManager.thisCharacter;
        characterCS = thisCharacter.GetComponent<CharacterControllerScript>();
    }

    void AddToolTip(string Tip)
    {
        GameEvents.current.inGameUI.setTip(Tip);
    }

    private string abiPointMapString = "";
    public Dictionary<Vector3Int, List<List<Vector3Int>>> GenerateAbiltyPointMap(AbilityData abilityData, Vector3Int fromPoint)
    {
        abiPointMapString = "";
        //var pointsToScan = GlobalCal.GenerateArea(AoeStyle.Taxi, fromPoint, fromPoint, (float)abilityData.rangeOfAbility / 10);
        var pointsToScan = GenerateAreaWithParams(abilityData.rangeOfAbility, fromPoint, fromPoint);
        Dictionary<Vector3Int, List<List<Vector3Int>>> pointToScannedAreas = new();
        foreach (Vector3Int point in pointsToScan)
        {
            List<List<Vector3Int>> list = new();
            foreach (AreaGenerationParams TileDataPair in abilityData.ValidTileData)
            {
                List<Vector3Int> tileCount = GenerateAreaWithParams(TileDataPair, fromPoint, point);

                //if (TileDataPair.minpoints <= tileCount.Count && tileCount.Count <= TileDataPair.MaxPoints)
                if (TileDataPair.minpoints <= tileCount.Count)
                {
                    //Debug.Log(TileDataPair.name +" Was Added");
                    list.Add(tileCount);
                }
            }
            if (abilityData.ValidTileData.Count == list.Count)
                pointToScannedAreas[point] = list;
        }
        //
        //Debug.Log(abiPointMapString);
        return pointToScannedAreas;
    }
    public List<Vector3Int> GenerateAreaWithParams(AreaGenerationParams tileToEffectPair, Vector3Int fromPoint, Vector3Int AtPoint)
    {
        var direction = GlobalCal.getNormalizedDirection(fromPoint, AtPoint);
        fromPoint = fromPoint + direction * tileToEffectPair.adjustAtPointWithDirection;
        AtPoint = fromPoint + direction;
        abiPointMapString += "   " + "Creating Area For " + tileToEffectPair.name + " with AoeStyle " + tileToEffectPair.aoeStyle + "From Point " + fromPoint + " At Point " + AtPoint + ":  ";
        var output = GlobalCal.GenerateArea(tileToEffectPair.aoeStyle, fromPoint, AtPoint, tileToEffectPair.rangeOfArea);
        output = mapManager.filterListWithTileRequirements(output, characterCS, tileToEffectPair.tileValidityParms.ShowCastOn);
        output = mapManager.filterListWithWalkRequirements(output, tileToEffectPair.tileValidityParms.validFloors);
        output = CheckVectorValidity(fromPoint, output, tileToEffectPair.tileValidityParms.targetType);
        PrintOutputStatus();
        return output;
        void PrintOutputStatus()
        {
            foreach (var point in output)
                abiPointMapString += point + ", ";
            abiPointMapString += "\n";
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
    public AbilityData currnetAbility;
    public void DoAction(AbilityData abilityData)
    {
        currnetAbility = abilityData;
        int costOfaction = characterCS.abilityToCost.returnDict()[abilityData];
        if (characterCS.actionPoints < costOfaction)
        {
            AddToolTip("The Ability " + currnetAbility.name + " cannot be used as you dont have action Points Remaining " + characterCS.actionPoints);
            return;
        }
        if (abilityData.ValidTileData.Count != abilityData.ApplyEffects.Count)
            Debug.Log("Great Erros");
        var pointMap = GenerateAbiltyPointMap(abilityData, characterCS.GetCharV3Int());
        if (pointMap.Keys.Count == 0)
        {
            AddToolTip("The Ability " + currnetAbility.name + " cannot be used as No Valid Tilees");
            reticalManager.reDrawInValidTiles(new List<Vector3Int>());
            return;
        }
        reticalManager.UpdateReticalInputParams(pointMap);
        StartCoroutine(SequenceOfEvents());
        IEnumerator SequenceOfEvents()
        {
            yield return StartCoroutine(GetInput(pointMap.Keys.ToList()));
            if (ShouldContinue)
            {
                characterCS.actionPoints = characterCS.actionPoints - costOfaction;//Consume Ability Point
                //Debug.Log("Should continew was true consuming ability Point");
                List<List<Vector3Int>> ListOfListPointToEffect = pointMap[tryHere];
                for (int i = 0; i < abilityData.ApplyEffects.Count; i++)
                {
                    yield return StartCoroutine(AnimationCoRoutione(ListOfListPointToEffect[i], abilityData.ApplyEffects[i], characterCS.GetCharV3Int(), tryHere));
                }
            }
            else if (ControlAI)
            {
                Debug.Log("AI Exception");
                turnManager.endTurn();
            }
            ShouldContinue = false;
            if (characterCS.DoActionPointsRemainAfterAbility())
                characterCS.BeginThisCharacterTurn();
            else
            {
                turnManager.endTurn();
            }
            yield return null;
        }
    }
    IEnumerator GetInput(List<Vector3Int> validInputs)
    {
        //Declaring Variables        
        reticalManager.reDrawValidTiles(validInputs);//this sets the Valid Tiles Overlay
        reticalManager.reDrawInValidTiles(GlobalCal.GenerateArea(currnetAbility.rangeOfAbility.aoeStyle, characterCS.GetCharV3Int(), characterCS.GetCharV3Int(), currnetAbility.rangeOfAbility.rangeOfArea));//this sets the Valid Tiles Overlay
        ShouldContinue = false;
        //Executing Script
        if (!characterCS.ControlCharacter)//if Non Player Character
        {
            tryHere = characterCS.GetTarget(currnetAbility);
            ShouldContinue = true;
            yield return new WaitForSeconds(UserDataManager.waitAI);
        }
        else//if it is the player character
        {
            AddToolTip("Select Purple Tile To Contine with  " + currnetAbility.name + " Or Right Click to Cancel");
            yield return new WaitUntil(() => CheckContinue());//this waits for MB0 or MB1         
            tryHere = reticalManager.getMovePoint();
        }
        if (ShouldContinue && validInputs.Contains(tryHere))
        {            /* this is fine */        }
        else
        {
            ShouldContinue = false;
            if (!characterCS.ControlCharacter)
            {
                Debug.LogError("AI Exception");
                turnManager.endTurn();
            }
        }
        reticalManager.reDrawValidTiles(new List<Vector3Int>());
        reticalManager.reDrawInValidTiles(new List<Vector3Int>());
        reticalManager.ResetReticalInputParams();
        //Methods
        bool CheckContinue()
        {
            if (Input.GetMouseButtonDown(0))
            {
                AddToolTip("Select an Button To Perform an Action");
                ShouldContinue = true;
                return true;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                AddToolTip(currnetAbility.name + " Cancelled; Select an Button To Perform an Action");
                ShouldContinue = false;
                return true;
            }
            return false;
        }
    }

    IEnumerator AnimationCoRoutione(List<Vector3Int> points, ActionEffectParams actionEffectParams, Vector3Int fromPoint, Vector3Int atPoint)
    {
        points = mapManager.filterListWithTileRequirements(points, characterCS, actionEffectParams.OnlyApplyOn);
        /* float startTime = Time.time;
        float lastTime = Time.time;
        CreateDebugLogsForTime(false);
        void CreateDebugLogsForTime(bool debugit = true)
        {
            if (!debugit)
                return;
            float currentTime = Time.time;
            float deltaTime = currentTime - lastTime;

            Debug.Log("Delta Time: " + deltaTime);
            lastTime = currentTime;
        } */
        //
        GameEvents.current.inGameUI.ClearButtons();//Clearing Buttons while action is in progress
        TypeOfAction actiontype = actionEffectParams.typeOfAction;
        //AnimationMovementType animationMovementType = actionEffectParams.animationMovementType;
        AnimationLoopType animationLoopType = actionEffectParams.loopType;

        if (animationLoopType == AnimationLoopType.forAction)
        {
            yield return StartCoroutine(animationActionFunction());
        }
        foreach (Vector3Int point in points)
        {
            //Debug.Log(characterCS.characterName + " did action " + currnetAbility.name + " at Point " + point);
            tryHere = point;
            if (animationLoopType == AnimationLoopType.forEachPoint)
                yield return StartCoroutine(animationActionFunction());
            //abilityNameToAction[actiontype]();//The Actual Action
            yield return StartCoroutine(abinameToAction(actiontype));
        }
        //Debug.Break();
        yield return afterAnimationOfAction();
        ///////Functions
        Vector3 calculateNudge(Vector3Int towardsPoint, float distanceFactor = 0.3f)
        {
            /* if (animationMovementType == AnimationMovementType.NoMovement)                targetLocation = fromPoint;            else if (animationMovementType == AnimationMovementType.NudgeToPoint) */
            //float distanceFactor = 0.3f;  // Adjust this value to control the distance from point1
            Vector3 direction = GlobalCal.getNormalizedDirection(fromPoint, towardsPoint);  // Calculate the direction between the points
            Vector3 midPoint = fromPoint + direction * distanceFactor * Vector3.Distance(fromPoint, towardsPoint);
            return midPoint;
        }
        IEnumerator animationActionFunction()
        {
            yield return StartCoroutine(TransformAnimationScript.current.MoveUsingQueueSystem(thisCharacter.transform, calculateNudge(atPoint), moveTimeSpeed));
            yield return StartCoroutine(characterCS.animationControllerScript.trySetNewAnimation(actionEffectParams.doActionTillKeyFrameAnimation));
        }
        IEnumerator afterAnimationOfAction()
        {
            StartCoroutine(TransformAnimationScript.current.MoveUsingQueueSystem(thisCharacter.transform, characterCS.GetCharV3Int(), moveTimeSpeed));
            StartCoroutine(characterCS.animationControllerScript.trySetNewAnimation(CharacterAnimationState.Idle));
            if (!UserDataManager.skipWaitTime)
                yield return new WaitForSeconds(UserDataManager.waitAction);
        }
        IEnumerator abinameToAction(TypeOfAction typeOfAction)
        {
            List<CharacterControllerScript> refreshList = new();
            yield return null;
            switch (typeOfAction)
            {
                case TypeOfAction.apply_Damage:
                    {
                        CharacterControllerScript targetCharacter = mapManager.cellDataDir[tryHere].characterAtCell.GetComponent<CharacterControllerScript>();
                        CharacterControllerScript attackingCharacter = thisCharacter.GetComponent<CharacterControllerScript>();
                        targetCharacter.health -= attackingCharacter.attackDamage;
                        refreshList.Add(targetCharacter);
                        GameEvents.current.PlaySound(0);
                        break;
                    }
                case TypeOfAction.apply_Heal:
                    {
                        break;
                    }
                case TypeOfAction.apply_SelfMove:
                    {
                        Vector3Int currentPosition = thisCharacter.GetComponent<CharacterControllerScript>().GetCharV3Int();
                        mapManager.UpdateCharacterPosistion(currentPosition, tryHere, thisCharacter);
                        break;
                    }
                case TypeOfAction.apply_Push:
                    {
                        CharacterControllerScript targetCharacter = mapManager.cellDataDir[tryHere].characterAtCell.GetComponent<CharacterControllerScript>();
                        Vector3Int sourceOfPush = characterCS.GetCharV3Int();
                        Vector3Int directionOfPush = GlobalCal.getNormalizedDirection(sourceOfPush, tryHere);
                        Vector3Int newPos = tryHere + directionOfPush;
                        if (!mapManager.isCellHoldingCharacer(newPos) &&
                        mapManager.filterListWithWalkRequirements(new List<Vector3Int>() { newPos }, targetCharacter.canWalkOn).Count != 0)
                        {
                            mapManager.UpdateCharacterPosistion(targetCharacter.GetCharV3Int(), newPos, targetCharacter.gameObject);
                            refreshList.Add(targetCharacter);
                        }
                        else
                        {
                            yield return StartCoroutine(TransformAnimationScript.current.MoveUsingQueueSystem(targetCharacter.transform, calculateNudge(newPos, 0.6f), moveTimeSpeed));
                            yield return StartCoroutine(abinameToAction(TypeOfAction.apply_Damage));
                        }
                        break;
                    }
                default:
                    break;
            }
            foreach (CharacterControllerScript character in refreshList)
            {
                if (character.transform.position != character.currentCellPosOfCharcter)
                    yield return StartCoroutine(
                        TransformAnimationScript.current.MoveUsingQueueSystem
                        (
                            character.transform, character.currentCellPosOfCharcter, moveTimeSpeed)
                        );
                checkCharacters(character);
                void checkCharacters(CharacterControllerScript targetCharacter)
                {
                    targetCharacter.CheckIfCharacterIsDead();
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
    apply_Push
}
public enum BoolEnum
{
    TrueOrFalse,
    True,
    False
}
