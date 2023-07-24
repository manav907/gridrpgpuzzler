using System;
using System.Collections;
using System.Collections.Generic;
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
    public bool smartPosistiong = false;
    public List<GroundFloorType> canWalkOn;
    public CharacterData CharacterDataSO;
    public Vector3Int currentCellPosOfCharcter;
    [SerializeField] private TMPro.TextMeshPro Heatlh;
    private MapManager mapManager;
    private TurnManager turnManager;
    public AnimationControllerScript animationControllerScript;
    UniversalCalculator universalCalculator;
    public SerializableDictionary<LadderCollapseFunction, int> abilityToCost;
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
            smartPosistiong = CharacterDataSO.smartPosistiong;
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
            StartCoroutine(animationControllerScript.setAnimationAndWaitForIt(CharacterAnimationState.Walk));
            if (controlCharacter)
            {
                GameEvents.current.TriggerNextDialog();//Disable this laeter
                List<LadderCollapseFunction> allAbilities = new List<LadderCollapseFunction>();
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
    Dictionary<TypeOfAction, List<LadderCollapseFunction>> abilityMap()
    {
        var newDict = new Dictionary<TypeOfAction, List<LadderCollapseFunction>>();
        foreach (var keypair in abilityToCost.KeyValuePairs)
        {
            //if (keypair.value <= actionPoints)
            {
                if (!newDict.ContainsKey(keypair.key.primaryUseForAction))
                {
                    newDict.Add(keypair.key.primaryUseForAction, new List<LadderCollapseFunction>());
                }
                newDict[keypair.key.primaryUseForAction].Add(keypair.key);
                //Debug.Log(characterName + " Character added " + keypair.key.Name + " it had a cost of " + keypair.value + " and actionPoints Remaining were" + actionPoints);
            }
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
            var VisionList = universalCalculator.generateRangeFromPoint(thisCharpos, rangeOfVision + GhostVision);
            //GhostVision for tracking after leaving Vision
            var targetList = listOfPossibleTargets(VisionList);
            //Debug.LogError("AI Stuf Needs Rework");
            if (targetList.Count == 0)
            {
                turnManager.endTurn();
                //Debug.Log("Ideling");
                return;
            }
            else
            {
                destinationTarget = selectOptimalTarget();
                var attackRangeList = moveDictionaryManager.getValidTargetList(optionsofAbilities[TypeOfAction.apply_Damage][0].SetDataAtIndex[0], getCharV3Int());
                for (int i = 0; i < attackRangeList.Count; i++)
                {
                    if (!moveDictionaryManager.CheckIfTargetis(attackRangeList[i], ValidTargets.Enemies))
                    {
                        //Debug.Log("Will Remove this Target is not an Enemy");
                        attackRangeList.Remove(attackRangeList[i]);
                        i--;
                    }
                    else
                    {
                        //Debug.Log("This is an Enemy");
                    }
                }
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
    public Vector3Int getTarget(ActionInputParams actionInputParams)
    //validTargets depends on the action being performed
    {
        if (actionInputParams.updateTheroticalPos)
        {
            var validPathToObjective = mapManager.findOptimalPath(getCharV3Int(), getUseableTarget(), actionInputParams, true);
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
            if (!moveDictionaryManager.getValidTargetList(actionInputParams, getCharV3Int()).Contains(chosenPath))
                return getCharV3Int();
            return chosenPath;
        }
        return getBasicDirection();
        Vector3Int getBasicDirection()
        {
            List<Vector3Int> validTiles = moveDictionaryManager.getValidTargetList(actionInputParams, getCharV3Int());
            Vector3Int selectedValidTile = universalCalculator.SortListAccordingtoDistanceFromPoint(validTiles, getUseableTarget()[0])[0];
            return selectedValidTile;
        }
        List<Vector3Int> getUseableTarget()
        {
            List<Vector3Int> validTiles = moveDictionaryManager.getValidTargetList(actionInputParams, destinationTarget);
            validTiles.Remove(destinationTarget);
            if (smartPosistiong == false || validTiles.Count == 0)
            {
                return new List<Vector3Int>() { destinationTarget };
            }
            return universalCalculator.SortListAccordingtoDistanceFromPoint(validTiles, getCharV3Int());
        }
    }
}