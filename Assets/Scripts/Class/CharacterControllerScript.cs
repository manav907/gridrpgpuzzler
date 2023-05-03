using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;


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
    public List<GroundFloorType> canWalkOn;
    public CharacterData CharacterDataSO;
    [SerializeField] private TextMesh Heatlh;
    private ButtonManager thisButtonManager;
    private MapManager thisMapManager;
    private TurnManager thisTurnManager;
    DataManager dataManager;
    UniversalCalculator universalCalculator;
    [SerializeField] List<Ability> abilityList;

    public void InitilizeCharacter(GameObject gameController)
    {
        thisButtonManager = gameController.GetComponent<ButtonManager>();
        thisMapManager = gameController.GetComponent<MapManager>();
        thisTurnManager = gameController.GetComponent<TurnManager>();
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

            //ListStuff
            canWalkOn = CharacterDataSO.canWalkOn;
            abilityList.AddRange(GlobalCal.createCopyListUsingConstructor(CharacterDataSO.listOfAbility));
            //Setting Data
            AbilityNameToAbilityDataDIR = AbilityNameToAbilityData();
            //Setting Specific Name
            this.name = characterName + " " + CharacterDataSO.InstanceID;
            //creating Override
            AnimatorController originalController = animator.runtimeAnimatorController as AnimatorController;
            //doingAnimController
            var SO = gameController.GetComponent<DataManager>().getFromSO(CharacterDataSO.NameEnum);
            animator.runtimeAnimatorController = SO.GetanimatorOverrideController(originalController);
            //Setting Sprite Stuff
            //Transform spriteHolder = gameObject.transform.Find("SpriteHolder");
            spriteHolder.position = new Vector3(spriteHolder.position.x, spriteHolder.position.y + SO.spriteOffsetY, spriteHolder.position.z);
            //Methods
            Dictionary<AbilityName, Ability> AbilityNameToAbilityData()
            {
                Dictionary<AbilityName, Ability> thisDir = new Dictionary<AbilityName, Ability>();
                foreach (Ability ability in abilityList)
                {
                    thisDir.Add(ability.abilityName, ability);
                    CharacterMoveList.Add(ability.abilityName);
                }
                return thisDir;
            }
        }
    }
    void ReplaceCharacterData()
    {
        //thisTurnManager.le
    }
    CharacterData generateCharacterDataFromCurrentCharacter()
    {
        CharacterData characterData = new CharacterData();
        characterData.InstanceID = CharacterDataSO.InstanceID;
        characterData.characterName = characterName;
        characterData.isPlayerCharacter = isPlayerCharacter;
        characterData.health = health;
        characterData.attackDamage = attackDamage;
        characterData.speedValue = speedValue;
        characterData.rangeOfVision = rangeOfVision;
        characterData.canWalkOn = CharacterDataSO.canWalkOn;
        characterData.listOfAbility = GlobalCal.createCopyListUsingConstructor(abilityList);
        return characterData;
    }
    public Dictionary<AbilityName, Ability> AbilityNameToAbilityDataDIR;
    public List<AbilityName> CharacterMoveList;
    public List<AbilityName> CharacterInventoryList;
    public bool CheckIfCharacterIsDead()
    {
        Heatlh.text = health + "";
        if (health <= 0)
        {
            Debug.Log(this.name + " Character has Died");
            KillCharacter();
            return true;
        }
        return false;
        void KillCharacter()
        {
            Vector3Int thisCharPos = universalCalculator.convertToVector3Int(this.gameObject.transform.position);
            thisTurnManager.OrderOfInteractableCharacters.Remove(gameObject);
            thisTurnManager.ListOfInteractableCharacters.Remove(gameObject);
            if (thisTurnManager.thisCharacter == this.gameObject)
            {
                thisTurnManager.endTurn();
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
    public void BeginThisCharacterTurn()
    {
        ToggleCharacterTurnAnimation(true);
        actionPoints = 1;//Remove This later
        thisButtonManager.clearButtons();
        if (controlCharacter)
            thisButtonManager.InstantiateButtons(CharacterMoveList);
        else
        {
            determineAction();
        }
    }
    Vector3Int currentTarget;
    int GhostVision = 1;
    void determineAction()
    {

        if (checkAI)
            Debug.Log("Determining Action");
        Vector3Int thisCharpos = getCharV3Int();
        var VisionList = universalCalculator.generateRangeFromPoint(thisCharpos, rangeOfVision + GhostVision);
        //GhostVision for tracking after leaving Vision
        var targetList = listOfPossibleTargets(VisionList);
        var attackRangeList = moveDictionaryManager.getValidTargetList(AbilityNameToAbilityDataDIR[AbilityName.Attack]);
        if (targetList.Count == 0)
        {
            Debug.Log("Ideling");
            moveDictionaryManager.doAction(AbilityName.EndTurn);
            return;
        }
        else
        {
            currentTarget = targetList[universalCalculator.SelectRandomBetweenZeroAndInt(targetList.Count)];//Setting Current Target for later use
            if (attackRangeList.Contains(currentTarget))
            {
                if (checkAI)
                    Debug.Log("Attacking");
                moveDictionaryManager.doAction(AbilityName.Attack);
            }
            else if (true)//if character not in attack range
            {
                if (checkAI)
                    Debug.Log("Moving");
                moveDictionaryManager.doAction(AbilityName.Move);
            }
        }
        List<Vector3Int> listOfPossibleTargets(List<Vector3Int> visionList)
        {
            var OrderOfInteractableCharacters = thisTurnManager.OrderOfInteractableCharacters;
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
    }
    public Vector3Int getCharV3Int()
    {
        return universalCalculator.convertToVector3Int(this.gameObject.transform.position);
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
    [SerializeField] Animator animator;
    [SerializeField] Transform spriteHolder;
    public void ToggleCharacterTurnAnimation(bool isCharacterTurn)
    {
        if (isCharacterTurn)
            animator.SetTrigger("Walk");
        else
            animator.SetTrigger("Idle");
    }


}
