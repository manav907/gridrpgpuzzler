using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CharacterControllerScript : MonoBehaviour
{
    public string characterName;
    [HideInInspector]
    public bool controlCharacter
    {
        get
        {
            if (moveDictionaryManager.ControlAI == false)
                return isPlayerCharacter;
            return true;
        }
    }
    public bool isPlayerCharacter = true;
    public int health;
    public int attackDamage;
    public int speedValue;
    public int rangeOfVision;
    public string faction;
    public List<GroundFloorType> canWalkOn;
    public CharacterData CharacterDataSO;
    public Vector3Int currentCellPosOfCharcter;
    [SerializeField] private TMPro.TextMeshPro Heatlh;
    private MapManager mapManager;
    private TurnManager turnManager;
    public AnimationControllerScript animationControllerScript;
    UniversalCalculator universalCalculator;
    public SerializableDictionary<AbilityData, int> abilityToCost;
    public void InitilizeCharacter(GameObject gameController)
    {
        mapManager = gameController.GetComponent<MapManager>();
        turnManager = gameController.GetComponent<TurnManager>();
        moveDictionaryManager = gameController.GetComponent<MoveDictionaryManager>();
        universalCalculator = gameController.GetComponent<UniversalCalculator>();
        //Initilizing
        try
        { setVariables(); }
        catch (NullReferenceException)
        {
            Debug.Log("Hey if you are having null reffence here then check out the consutructor for the SO it might not have the new variable");
        }
        CheckIfCharacterIsDead();
        void setVariables()
        {
            characterName = CharacterDataSO.characterName;
            isPlayerCharacter = CharacterDataSO.isPlayerCharacter;
            health = CharacterDataSO.health;
            attackDamage = CharacterDataSO.attackDamage;
            speedValue = CharacterDataSO.speedValue;
            rangeOfVision = CharacterDataSO.rangeOfVision;
            faction = CharacterDataSO.Faction;
            //ListStuff
            canWalkOn = CharacterDataSO.canWalkOn;
            //Rewordk This
            defaultActionPoints = CharacterDataSO.defaultActionPoints;
            abilityToCost = CharacterDataSO.abilityToCost;
            //Setting Data
            //Setting Specific Name
            this.name = characterName + " " + CharacterDataSO.InstanceID;
            //Game Event Regersery
            GameEvents.current.addCharacter(isPlayerCharacter);
            //doingAnimController
            //animationControllerScript.setVariables(gameController.GetComponent<DataManager>().getFromSO(CharacterDataSO.NameEnum));
            animationControllerScript.setVariables(CharacterDataSO.characterAnimationData);
            //Methods
        }
    }
    public bool CheckIfCharacterIsDead()
    {
        Heatlh.text = health + "";
        if (health <= 0)
        {
            //Debug.Log(this.name + " Character has Died");
            KillCharacter();
            return true;
        }
        return false;
        void KillCharacter()
        {
            GameEvents.current.DeathEvent(this.GetComponent<CharacterControllerScript>());
            isALive = false;
            Vector3Int thisCharPos = universalCalculator.castAsV3Int(this.gameObject.transform.position);
            turnManager.OrderOfInteractableCharacters.Remove(gameObject);
            turnManager.ListOfInteractableCharacters.Remove(gameObject);
            if (controlCharacter)
                GameEvents.current.PlaySound(2);
            mapManager.KillCharacter(getCharV3Int());
            if (TurnManager.thisCharacter == this.gameObject)
            {
                turnManager.endTurn();
            }
            Destroy(this.gameObject);
        }
    }
    public int actionPoints = 1;
    public int defaultActionPoints = 1;
    public bool doActionPointsRemainAfterAbility()
    {
        if (actionPoints <= 0)
            return false;
        else
        {
            return true;
        }
    }
    MoveDictionaryManager moveDictionaryManager;
    bool isALive = true;
    public void BeginThisCharacterTurn()
    {
        if (isALive)
        {
            //animationControllerScript.setCharacterAnimationAndReturnLength(CharacterAnimationState.Walk);
            StartCoroutine(animationControllerScript.trySetNewAnimation(CharacterAnimationState.Walk));
            if (controlCharacter)
            {
                GameEvents.current.TriggerNextDialog();//Disable this laeter
                List<AbilityData> allAbilities = new List<AbilityData>();
                allAbilities.AddRange(abilityToCost.Keys());
                GameEvents.current.inGameUI.MakeButtonsFromLadderCollapseFunction(allAbilities);
                turnManager.setCameraPos(getCharV3Int());
            }
            else
            {
                determineAction();
            }
        }
    }
    [SerializeField] Vector3Int destinationTarget;
    int GhostVision = 1;
    Dictionary<TypeOfAction, List<AbilityData>> abilityMap()
    {
        var newDict = new Dictionary<TypeOfAction, List<AbilityData>>();
        foreach (var keypair in abilityToCost.KeyValuePairs)
        {
            if (!newDict.ContainsKey(keypair.key.Primaryuse))
            {
                newDict.Add(keypair.key.Primaryuse, new List<AbilityData>());
            }
            newDict[keypair.key.Primaryuse].Add(keypair.key);
            //Debug.Log(characterName + " Character added " + keypair.key.name + " it had a cost of " + keypair.value + " and actionPoints Remaining were" + actionPoints);

        }
        return newDict;
    }
    void determineAction()
    {
        var optionsofAbilities = abilityMap();
        var costIndex = abilityToCost.returnDict();
        if (actionPoints > 0)
        {
            Vector3Int thisCharpos = getCharV3Int();
            //var VisionList = GlobalCal.generateRangeFromPoint(thisCharpos, rangeOfVision + GhostVision);
            var VisionList = GlobalCal.generateArea(AoeStyle.Square, thisCharpos, thisCharpos, rangeOfVision + GhostVision);

            //GhostVision for tracking after leaving Vision
            var targetList = listOfPossibleTargets(VisionList);
            //Debug.LogError("AI Stuf Needs Rework");
            if (targetList.Count == 0)
            {
                turnManager.endTurn();
                Debug.Log("Ideling");
                return;
            }
            else
            {
                destinationTarget = selectOptimalTarget();
                //var attackRangeList = moveDictionaryManager.getValidTargetList(optionsofAbilities[TypeOfAction.apply_Damage][0].SetDataAtIndex[0], getCharV3Int());
                var attackRangeList = moveDictionaryManager.generateAbiltyPointMap(optionsofAbilities[TypeOfAction.apply_Damage][0], getCharV3Int()).Keys.ToList();
                attackRangeList = mapManager.filterListWithTileRequirements(attackRangeList, this, ValidTargets.Enemies);
                if (attackRangeList.Count > 0)
                {
                    if (costIndex[optionsofAbilities[TypeOfAction.apply_Damage][0]] <= actionPoints)
                    {
                        moveDictionaryManager.doAction(optionsofAbilities[TypeOfAction.apply_Damage][0]);
                    }
                    else
                    {
                        turnManager.endTurn();
                    }
                }
                else if (costIndex[optionsofAbilities[TypeOfAction.apply_SelfMove][0]] <= actionPoints)
                {
                    moveDictionaryManager.doAction(optionsofAbilities[TypeOfAction.apply_SelfMove][0]);
                }
                else
                {
                    Debug.Log("no Action Points for apply move");
                    turnManager.endTurn();
                }
            }
            //determineAction();
            List<Vector3Int> listOfPossibleTargets(List<Vector3Int> visionList)
            {
                var OrderOfInteractableCharacters = turnManager.OrderOfInteractableCharacters;
                List<Vector3Int> thisList = new List<Vector3Int>();
                foreach (GameObject character in OrderOfInteractableCharacters)
                {
                    CharacterControllerScript CSS = character.GetComponent<CharacterControllerScript>();
                    Vector3Int positionofCSS = CSS.getCharV3Int();
                    if (visionList.Contains(positionofCSS))//if Character is in vision
                        if (CSS.controlCharacter)//if is Player Character
                            thisList.Add(positionofCSS);
                }
                thisList.Remove(thisCharpos);
                return thisList;
            }
            Vector3Int selectOptimalTarget()
            {
                return universalCalculator.SortListAccordingtoDistanceFromPoint(targetList, getCharV3Int())[0];
            }
        }
        else
        {
            Debug.Log("no Action Points");
            turnManager.endTurn();
        }
    }
    public Vector3Int getCharV3Int()
    {
        if (mapManager.cellDataDir[currentCellPosOfCharcter].characterAtCell = this.gameObject)
            return currentCellPosOfCharcter;
        Debug.LogError("Fata error");
        return Vector3Int.zero;
    }
    public Vector3Int getTarget(AbilityData abilityData)
    //validTargets depends on the action being performed
    {
        List<Vector3Int> validTiles = moveDictionaryManager.generateAbiltyPointMap(abilityData, getCharV3Int()).Keys.ToList();
        if (abilityData.Primaryuse == TypeOfAction.apply_SelfMove)
        {
            var validPathToObjective = mapManager.findOptimalPath(getCharV3Int(), getUseableTarget(), abilityData, true);
            validPathToObjective.Remove(currentCellPosOfCharcter);
            if (validPathToObjective.Count == 0)
            {
                Debug.Log("using FallBAck");
                return getBasicDirection();
            }
            Vector3Int chosenPath = validPathToObjective[0];
            if (chosenPath == getCharV3Int())
            {
                Debug.Log("no Vaid Path Found");
                return getBasicDirection();
            }
            if (!validTiles.Contains(chosenPath))
            {
                Debug.Log("path does not have currently moveable Tiles using Fallback");
                return getCharV3Int();
            }
            return chosenPath;
        }
        return getBasicDirection();
        Vector3Int getBasicDirection()
        {
            Vector3Int selectedValidTile = universalCalculator.SortListAccordingtoDistanceFromPoint(validTiles, getUseableTarget()[0])[0];
            return selectedValidTile;
        }
        List<Vector3Int> getUseableTarget()
        {
            List<Vector3Int> validTiles = moveDictionaryManager.generateAbiltyPointMap(abilityData, destinationTarget).Keys.ToList();
            validTiles.Remove(destinationTarget);
            if (UserDataManager.SmartPosistioning == false || validTiles.Count == 0)
            {
                return new List<Vector3Int>() { destinationTarget };
            }
            return universalCalculator.SortListAccordingtoDistanceFromPoint(validTiles, getCharV3Int());
        }
    }
}