using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CharacterControllerScript : MonoBehaviour
{
    [HideInInspector]
    public bool ControlCharacter
    {
        get
        {
            if (moveDictionaryManager.ControlAI == false)
                return isPlayerCharacter;
            return true;
        }
    }
    public CharacterData CharacterDataSO;
    [SerializeField] private TMPro.TextMeshPro Heatlh;
    private MapManager mapManager;
    private TurnManager turnManager;
    MoveDictionaryManager moveDictionaryManager;
    public AnimationControllerScript animationControllerScript;
    UniversalCalculator universalCalculator;
    [Header("SO Data")]
    public string characterName;
    public bool isPlayerCharacter = true;
    public int health;
    public int attackDamage;
    public int speedValue;
    public int rangeOfVision;
    public string faction;
    public List<AbilityData> listOfAbilities;
    public List<GroundFloorType> canWalkOn;
    public int maxStamina = 1;
    public int maxFocusPoints = 2;
    [Header("Debuff Data")]
    public Vector3Int currentCellPosOfCharcter;
    public int StaminaPenelty = 0;
    public int currentStamina;
    public int currentFocusPoints;
    bool isALive = true;
    [SerializeField] Vector3Int destinationTarget;
    private readonly int GhostVision = 1;
    public void InitilizeCharacter(GameObject gameController)
    {
        mapManager = gameController.GetComponent<MapManager>();
        turnManager = gameController.GetComponent<TurnManager>();
        moveDictionaryManager = gameController.GetComponent<MoveDictionaryManager>();
        universalCalculator = gameController.GetComponent<UniversalCalculator>();
        reticalManager = gameController.GetComponent<ReticalManager>();
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
            maxStamina = CharacterDataSO.maxStamina;
            maxFocusPoints = CharacterDataSO.maxFocusPoints;
            currentFocusPoints = maxFocusPoints;
            listOfAbilities = CharacterDataSO.listOfAbilities;
            //Setting Data
            //Setting Specific Name
            this.name = characterName + " " + CharacterDataSO.InstanceID;
            //Game Event Regersery
            GameEvents.current.AddCharacter(isPlayerCharacter);
            //doingAnimController
            //animationControllerScript.setVariables(gameController.GetComponent<DataManager>().getFromSO(CharacterDataSO.NameEnum));
            animationControllerScript.SetVariables(CharacterDataSO.characterAnimationData);
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
            GameEvents.current.DeathEvent(GetComponent<CharacterControllerScript>());
            isALive = false;
            turnManager.OrderOfInteractableCharacters.Remove(gameObject);
            turnManager.ListOfInteractableCharacters.Remove(gameObject);
            if (ControlCharacter)
                GameEvents.current.PlaySound(2);
            mapManager.KillCharacter(GetCharV3Int());
            if (TurnManager.thisCharacter == gameObject)
            {
                turnManager.EndTurn();
            }
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }
    public bool DoActionPointsRemainAfterAbility()
    {
        if (currentStamina > 0)//if we have more than 0 actionPoints
            return true;
        return false;
    }
    public void BeginThisCharacterTurn()
    {
        if (isALive)
        {
            if (StaminaPenelty > 0)
            {
                currentStamina = 0;
                StaminaPenelty--;
                if (StaminaPenelty <= 0)
                    StartCoroutine(animationControllerScript.SetStatusEffect(StatusEffect.Normal));
                turnManager.EndTurn();
            }
            else
            {
                StartCoroutine(animationControllerScript.TrySetNewAnimation(CharacterAnimationState.Walk));
                if (ControlCharacter)
                {
                    GameEvents.current.TriggerNextDialog();//Disable this laeter
                    List<AbilityData> allAbilities = new();
                    allAbilities.AddRange(listOfAbilities);
                    GameEvents.current.inGameUI.MakeButtonsFromAbilityies(allAbilities);
                    turnManager.SetCameraPos(GetCharV3Int());
                }
                else
                {
                    DetermineAction();
                }
            }
        }
    }

    Dictionary<TypeOfAction, List<AbilityData>> AbilityMap()
    {
        var newDict = new Dictionary<TypeOfAction, List<AbilityData>>();
        foreach (var keypair in listOfAbilities)
        {
            if (!newDict.ContainsKey(keypair.Primaryuse))
            {
                newDict.Add(keypair.Primaryuse, new List<AbilityData>());
            }
            newDict[keypair.Primaryuse].Add(keypair);
            //Debug.Log(characterName + " Character added " + keypair.key.name + " it had a cost of " + keypair.value + " and actionPoints Remaining were" + actionPoints);
        }
        return newDict;
    }
    void DetermineAction()
    {
        var optionsofAbilities = AbilityMap();
        if (currentStamina > 0)
        {
            Vector3Int thisCharpos = GetCharV3Int();
            var VisionList = GlobalCal.GenerateArea(AoeStyle.Square, thisCharpos, thisCharpos, rangeOfVision + GhostVision);
            //GhostVision for tracking after leaving Vision
            var targetList = listOfPossibleTargets(VisionList);
            //Debug.LogError("AI Stuf Needs Rework");
            if (targetList.Count == 0)
            {
                turnManager.EndTurn();
                //Debug.Log("Ideling");
                return;
            }
            else
            {
                destinationTarget = selectOptimalTarget();
                //var attackRangeList = moveDictionaryManager.getValidTargetList(optionsofAbilities[TypeOfAction.apply_Damage][0].SetDataAtIndex[0], getCharV3Int());
                var attackRangeList = moveDictionaryManager.GenerateAbiltyPointMap(optionsofAbilities[TypeOfAction.apply_Damage][0], GetCharV3Int()).Keys.ToList();
                attackRangeList = mapManager.FilterListWithTileRequirements(attackRangeList, this, ValidTargets.Enemies);
                if (attackRangeList.Count > 0)
                {
                    if (optionsofAbilities[TypeOfAction.apply_Damage][0].CheckAbilityBudget(this))
                    {
                        moveDictionaryManager.DoAction(optionsofAbilities[TypeOfAction.apply_Damage][0]);
                    }
                    else
                    {
                        turnManager.EndTurn();
                    }
                }
                else if (optionsofAbilities[TypeOfAction.apply_SelfMove][0].CheckAbilityBudget(this))
                {
                    moveDictionaryManager.DoAction(optionsofAbilities[TypeOfAction.apply_SelfMove][0]);
                }
                else
                {
                    Debug.Log("no Action Points for apply move");
                    turnManager.EndTurn();
                }
            }
            //determineAction();
            List<Vector3Int> listOfPossibleTargets(List<Vector3Int> visionList)
            {
                var OrderOfInteractableCharacters = turnManager.OrderOfInteractableCharacters;
                List<Vector3Int> thisList = new();
                foreach (GameObject character in OrderOfInteractableCharacters)
                {
                    CharacterControllerScript CSS = character.GetComponent<CharacterControllerScript>();
                    Vector3Int positionofCSS = CSS.GetCharV3Int();
                    if (visionList.Contains(positionofCSS))//if Character is in vision
                        if (CSS.ControlCharacter)//if is Player Character
                            thisList.Add(positionofCSS);
                }
                thisList.Remove(thisCharpos);
                return thisList;
            }
            Vector3Int selectOptimalTarget()
            {
                return universalCalculator.SortListAccordingtoDistanceFromPoint(targetList, GetCharV3Int())[0];
            }
        }
        else
        {
            Debug.Log("no Action Points");
            turnManager.EndTurn();
        }
    }
    public Vector3Int GetCharV3Int()
    {
        if (mapManager.cellDataDir[currentCellPosOfCharcter].characterAtCell = this.gameObject)
            return currentCellPosOfCharcter;
        Debug.LogError("Fata error");
        return Vector3Int.zero;
    }
    ReticalManager reticalManager;
    public Vector3Int GetTarget(AbilityData abilityData)
    //validTargets depends on the action being performed
    {
        List<Vector3Int> validTiles = moveDictionaryManager.GenerateAbiltyPointMap(abilityData, GetCharV3Int()).Keys.ToList();
        if (abilityData.Primaryuse == TypeOfAction.apply_SelfMove)
        {
            var validPathToObjective = mapManager.FindOptimalPath(GetCharV3Int(), getUseableTarget(), abilityData, true);
            //
            /* reticalManager.reDrawValidTiles(validPathToObjective);
            Debug.Break(); */
            //
            validPathToObjective.Remove(currentCellPosOfCharcter);
            if (validPathToObjective.Count == 0)
            {
                Debug.Log("using FallBAck");
                return getBasicDirection();
            }
            Vector3Int chosenPath = validPathToObjective[0];
            if (chosenPath == GetCharV3Int())
            {
                Debug.Log("no Vaid Path Found");
                return getBasicDirection();
            }
            if (!validTiles.Contains(chosenPath))
            {
                Debug.Log("path does not have currently moveable Tiles using Fallback");
                return GetCharV3Int();
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
            List<Vector3Int> validTiles = moveDictionaryManager.GenerateAbiltyPointMap(abilityData, destinationTarget).Keys.ToList();
            validTiles.Remove(destinationTarget);
            if (UserDataManager.SmartPosistioning == false || validTiles.Count == 0)
            {
                return new List<Vector3Int>() { destinationTarget };
            }
            return universalCalculator.SortListAccordingtoDistanceFromPoint(validTiles, GetCharV3Int());
        }
    }
}