using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;


public class characterDataHolder : MonoBehaviour
{
    public string characterName;
    public int health;
    public int rangeOfMove;
    public int rangeOfAttack;
    public int AttackDamage;
    public int speedValue;
    public int rangeOfVision;
    public List<GroundFloorType> canWalkOn;
    public CharacterData thisCharacterData;
    [SerializeField] private TextMesh Heatlh;

    private ButtonManager thisButtonManager;
    private MapManager thisMapManager;
    private TurnManager thisTurnManager;
    UniversalCalculator universalCalculator;
    public void InitilizeCharacter(GameObject gameController)
    {
        thisButtonManager = gameController.GetComponent<ButtonManager>();
        thisMapManager = gameController.GetComponent<MapManager>();
        thisTurnManager = gameController.GetComponent<TurnManager>();
        moveDictionaryManager = gameController.GetComponent<MoveDictionaryManager>();
        universalCalculator = gameController.GetComponent<UniversalCalculator>();


        setVariables();
        CheckIfCharacterIsDead();
    }
    void setVariables()
    {
        characterName = thisCharacterData.name;
        health = thisCharacterData.health;
        rangeOfMove = thisCharacterData.rangeOfMove;
        rangeOfAttack = thisCharacterData.rangeOfAttack;
        AttackDamage = thisCharacterData.AttackDamage;
        speedValue = thisCharacterData.speedValue;
        rangeOfVision = thisCharacterData.rangeOfVision;
        canWalkOn = thisCharacterData.canWalkOn;

        //Setting Specific Name

        this.name = characterName + " " + thisCharacterData.InstanceID;

        //creating Override
        AnimatorController originalController = animator.runtimeAnimatorController as AnimatorController;


        //doingAnimController
        animator.runtimeAnimatorController = thisCharacterData.GetanimatorOverrideController(originalController);
        //Setting Sprite Stuff
        setSpriteHolderProperties();
    }
    void setSpriteHolderProperties()
    {
        Transform spriteHolder = gameObject.transform.Find("SpriteHolder");
        if (spriteHolder != null)
        {
            spriteHolder.position = new Vector3(spriteHolder.position.x, thisCharacterData.spriteOffsetY, spriteHolder.position.z);
        }
        else
        {
            Debug.Log("SpriteHolder not found.");
        }
    }

    [SerializeField] Animator animator;
    public void ToggleCharacterTurnAnimation(bool isCharacterTurn)
    {
        if (isCharacterTurn)
            animator.SetTrigger("Walk");
        else
            animator.SetTrigger("Idle");


    }
    //public Dictionary <string, int> MoveToRange;
    public Dictionary<string, int> MoveToRange()
    {
        var thisDir = new Dictionary<string, int>();
        thisDir.Add("Move", rangeOfMove);
        thisDir.Add("Attack", rangeOfAttack);
        thisDir.Add("FireBall", 0);
        thisDir.Add("EndTurn", 0);
        return thisDir;
    }

    List<AbilityName> GetCharacterMoveList()
    {
        List<AbilityName> defaultMovesAvaliable;
        defaultMovesAvaliable = new List<AbilityName>();
        defaultMovesAvaliable.AddRange(new List<AbilityName>
        {
            AbilityName.Move,
            AbilityName.Attack,
            AbilityName.FireBall,
            AbilityName.EndTurn
        });
        return defaultMovesAvaliable;
    }
    public bool CheckIfCharacterIsDead()
    {
        Heatlh.text = health + "";
        if (health <= 0)
        {
            Debug.Log("I am dying");
            KillCharacter();
            return true;
        }
        return false;
    }
    void KillCharacter()
    {
        Vector3Int thisCharPos = universalCalculator.convertToVector3Int(this.gameObject.transform.position);
        thisTurnManager.OrderOfInteractableCharacters.Remove(gameObject);
        Destroy(this.gameObject);
        if (thisTurnManager.thisCharacter == this.gameObject)
        {
            thisTurnManager.endTurn();
        }
    }
    public bool isPlayerCharacter = true;
    MoveDictionaryManager moveDictionaryManager;
    public void BeginThisCharacterTurn()
    {
        ToggleCharacterTurnAnimation(true);
        thisButtonManager.clearButtons();
        if (isPlayerCharacter)
            thisButtonManager.InstantiateButtons(GetCharacterMoveList());
        else
        {
            determineAction();
        }
    }
    Vector3Int currentTarget;
    int GhostVision = 1;
    void determineAction()
    {
        Vector3Int thisCharpos = getCharV3Int();
        var VisionList = universalCalculator.generateRangeFromPoint(thisCharpos, rangeOfVision + GhostVision);
        //GhostVision for tracking after leaving Vision
        var targetList = listOfPossibleTargets(VisionList);
        var attackRangeList = universalCalculator.generateRangeFromPoint(thisCharpos, rangeOfAttack);
        if (targetList.Count == 0)
        {
            Debug.Log("Ideling");
            moveDictionaryManager.doAction("End Turn");
            return;
        }
        else
        {
            currentTarget = targetList[universalCalculator.SelectRandomBetweenZeroAndInt(targetList.Count)];
            if (attackRangeList.Contains(currentTarget))
            {
                moveDictionaryManager.doAction("Attack");
                //Debug.Log("Health of " + this.gameObject.name + " is " + health + "Attacked character");
                //Attack Character
            }
            else if (true)//if character not in attack range
            {
                moveDictionaryManager.doAction("Move");
            }

        }
        List<Vector3Int> listOfPossibleTargets(List<Vector3Int> visionList)
        {
            var OrderOfInteractableCharacters = thisTurnManager.OrderOfInteractableCharacters;
            List<Vector3Int> thisList = new List<Vector3Int>();
            foreach (GameObject thisCharacter in OrderOfInteractableCharacters)
            {
                var thisPos = thisCharacter.GetComponent<characterDataHolder>().getCharV3Int();
                if (visionList.Contains(thisPos))
                    thisList.Add(thisPos);
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
        return universalCalculator.SortListAccordingtoDistanceFromPoint(validTargets, currentTarget)[0];
        //Need to clarify how this works
        //For Moving it selects the closet point to target which when character is at point black range(not attacking when it should) just moves around the target character
        //For Attacking since the determineAction confirms a target(currentTarget) exist in valid targets the universalCalculator returns the currentTarget

    }


}
