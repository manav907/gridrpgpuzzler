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
            if (dataManager.EditMapMode == false)
                return isPlayerCharacter;
            return true;
        }
    }
    public bool isPlayerCharacter = true;
    [SerializeField] private bool checkAI = false;
    public int health;
    public int attackDamage;
    public int speedValue;
    public int rangeOfVision;
    public string faction;
    public List<GroundFloorType> canWalkOn;
    public CharacterData CharacterDataSO;
    public Vector3Int CellPosOfCharcter;
    [SerializeField] private TMPro.TextMeshPro Heatlh;
    //[SerializeField] private TextMesh Heatlh;
    private ButtonManager buttonManager;
    private MapManager mapManager;
    private TurnManager turnManager;
    public AnimationControllerScript animationControllerScript;
    DataManager dataManager;
    UniversalCalculator universalCalculator;
    [SerializeField] List<LadderCollapseFunction> ladderList;

    public void InitilizeCharacter(GameObject gameController)
    {
        buttonManager = gameController.GetComponent<ButtonManager>();
        mapManager = gameController.GetComponent<MapManager>();
        turnManager = gameController.GetComponent<TurnManager>();
        moveDictionaryManager = gameController.GetComponent<MoveDictionaryManager>();
        dataManager = gameController.GetComponent<DataManager>();
        universalCalculator = gameController.GetComponent<UniversalCalculator>();
        //Initilizing
        setVariables();
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
            //ladderList.AddRange(GlobalCal.createCopyListUsingConstructor(CharacterDataSO.LadderAbility));
            ladderList = CharacterDataSO.LadderAbility;
            //Setting Data
            //Setting Specific Name
            this.name = characterName + " " + CharacterDataSO.InstanceID;
            //Game Event Regersery
            GameEvents.current.addCharacter(isPlayerCharacter);
            //doingAnimController
            animationControllerScript.setVariables(gameController.GetComponent<DataManager>().getFromSO(CharacterDataSO.NameEnum));
            //Methods
        }
    }
    public void saveCharacterDataToCSO()
    {
        CharacterDataSO.characterName = characterName;
        CharacterDataSO.isPlayerCharacter = isPlayerCharacter;
        CharacterDataSO.health = health;
        CharacterDataSO.attackDamage = attackDamage;
        CharacterDataSO.speedValue = speedValue;
        CharacterDataSO.rangeOfVision = rangeOfVision;
        CharacterDataSO.canWalkOn = canWalkOn;
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
    public bool doActionPointsRemainAfterAbility()
    {
        actionPoints--;
        if (actionPoints == 0)
            return false;
        else
        {
            determineAction();
            return true;
        }
    }
    MoveDictionaryManager moveDictionaryManager;
    bool isALive = true;
    public void BeginThisCharacterTurn()
    {

        if (isALive)
        {
            animationControllerScript.setCharacterAnimation(CharacterAnimationState.Walk);
            actionPoints = 1;//Remove This later
                             //buttonManager.clearButtons();
            if (controlCharacter)
            {
                GameEvents.current.TriggerNextDialog();//Disable this laeter
                buttonManager.InstantiateButtons(ladderList);
                turnManager.setCameraPos(getCharV3Int());
            }
            else
            {
                determineAction();
            }
        }
    }
    [SerializeField] Vector3Int currentTarget;
    int GhostVision = 1;
    Dictionary<TypeOfAction, LadderCollapseFunction> abilityMap()
    {
        var newDict = new Dictionary<TypeOfAction, LadderCollapseFunction>();
        foreach (var ability in ladderList)
        {
            newDict.Add(ability.primaryUseForAction, ability);
        }
        return newDict;
    }
    void determineAction()
    {

        if (checkAI)
            Debug.Log("Determining Action");
        Vector3Int thisCharpos = getCharV3Int();
        var VisionList = universalCalculator.generateRangeFromPoint(thisCharpos, rangeOfVision + GhostVision);
        var optionsofAbilities = abilityMap();
        //GhostVision for tracking after leaving Vision
        var targetList = listOfPossibleTargets(VisionList);
        var attackRangeList = moveDictionaryManager.getValidTargetList(optionsofAbilities[TypeOfAction.apply_Damage].SetDataAtIndex[0]);

        //Debug.LogError("AI Stuf Needs Rework");
        if (targetList.Count == 0)
        {
            if (checkAI)
                Debug.Log("Ideling");
            moveDictionaryManager.doAction(optionsofAbilities[TypeOfAction.apply_TryEndTurn]);
            return;
        }
        else
        {
            currentTarget = selectOptimalTarget();
            if (attackRangeList.Contains(currentTarget))
            {
                if (checkAI)
                    Debug.Log("Attacking");
                //moveDictionaryManager.doAction(AbilityName.Attack);
                moveDictionaryManager.doAction(optionsofAbilities[TypeOfAction.apply_Damage]);
            }
            else if (true)//if character not in attack range
            {
                if (checkAI)
                    Debug.Log("Moving");
                moveDictionaryManager.doAction(optionsofAbilities[TypeOfAction.apply_SelfMove]);
            }
        }
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
    public Vector3Int getCharV3Int()
    {
        if (mapManager.cellDataDir[CellPosOfCharcter].characterAtCell = this.gameObject)
            return CellPosOfCharcter;
        Debug.LogError("Fata chara erro");
        return Vector3Int.zero;
    }


    public Vector3Int getTarget(List<Vector3Int> validTargets)
    //validTargets depends on the action being performed
    {
        Vector3Int target = universalCalculator.SortListAccordingtoDistanceFromPoint(validTargets, currentTarget)[0];
        if (checkAI)
        {
            Debug.Log("Chosen Target " + target);
        }
        return target;
        //For Moving it selects the closet point to target which when character is at point black range(not attacking when it should) just moves around the target character
        //For Attacking since the determineAction confirms a target(currentTarget) exist in valid targets the universalCalculator returns the currentTarget
    }
}
