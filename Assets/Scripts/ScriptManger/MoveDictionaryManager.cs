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
    MoveDictionaryManager moveDictionaryManager;
    UniversalCalculator universalCalculator;
    ButtonManager buttonManager;
    public void setVariables()
    {
        turnManager = this.GetComponent<TurnManager>();
        reticalManager = this.GetComponent<ReticalManager>();
        mapManager = this.GetComponent<MapManager>();
        universalCalculator = this.GetComponent<UniversalCalculator>();
        buttonManager = this.GetComponent<ButtonManager>();
        SetMoveDictionary();
    }
    GameObject thisCharacter;
    CharacterControllerScript thisCharacterCDH;
    public void getThisCharacterData()
    {
        thisCharacter = turnManager.thisCharacter;
        //Debug.Log(thisCharacter.name);//
        thisCharacterCDH = thisCharacter.GetComponent<CharacterControllerScript>();
    }
    Dictionary<AbilityName, ActionDataClass> aDCL;
    void SetMoveDictionary()
    {
        aDCL = new Dictionary<AbilityName, ActionDataClass>();
        //Debug.Log(thisCharacter.name);
        List<ActionDataClass> actionDataClass = new List<ActionDataClass>() {
            new ActionDataClass(AbilityName.Move,MoveCharacter, true, false, true ),
            new ActionDataClass(AbilityName.Attack,AttackHere, true, true, true || false),
            new ActionDataClass(AbilityName.EndTurn,endTurn, false, false, false),
            new ActionDataClass(AbilityName.OpenInventory,OpenInventory, false, false, true),
            new ActionDataClass(AbilityName.CloseInventory,CloseInventory, false, false, true),
            new ActionDataClass(AbilityName.FireBall,ThrowFireBall, false, false, true),
            new ActionDataClass(AbilityName.HeartPickup,HeartPickup,false,false,true)
            };
        //This is for Refference (string NameofMove, Action actionOfMove, bool needsButton, bool GameObjectHere, bool WalkableTileHere)    
        aDCL = actionDataClass.ToDictionary(ad => ad.abilityName, ad => ad);
        void MoveCharacter()
        {
            if (GetDataForActions())
            {
                Vector3Int currentPosition = universalCalculator.convertToVector3Int(thisCharacter.transform.position);
                mapManager.cellDataDir[currentPosition].characterAtCell = null;
                mapManager.cellDataDir[tryHere].characterAtCell = thisCharacter;
                thisCharacter.transform.position = tryHere;
                endTurn();
            }
            else
            {
                //Action Failed
                //Debug.Log("MoveCharacter");
            }
        }
        void AttackHere()
        {
            if (GetDataForActions())
            {
                CharacterControllerScript targetCharacter = mapManager.cellDataDir[tryHere].characterAtCell.GetComponent<CharacterControllerScript>();
                CharacterControllerScript attackingCharacter = thisCharacter.GetComponent<CharacterControllerScript>();
                targetCharacter.health -= attackingCharacter.AttackDamage;
                if (targetCharacter.CheckIfCharacterIsDead() == false)
                    endTurn();
            }
            //else
            {
                //problem
                //Debug.Log("AttackHere");
            }
        }
        void endTurn()
        {
            CharacterControllerScript targetCharacter = thisCharacter.gameObject.GetComponent<CharacterControllerScript>();
            targetCharacter.ToggleCharacterTurnAnimation(false); ;
            this.GetComponent<TurnManager>().endTurn();

        }
        void OpenInventory()
        {
            //Debug.Log("Open Inv");
            buttonManager.InstantiateButtons(thisCharacterCDH.CharacterInventoryList);

        }
        void CloseInventory()
        {
            //Debug.Log("Close Inv");
            buttonManager.InstantiateButtons(thisCharacterCDH.CharacterMoveList);
        }
        void ThrowFireBall()
        {
            //Debug.Log("Throw Fire Ball");
            StartCoroutine(getInput(delegate
            {
                MoveCharacter();
            }, aDCL[AbilityName.Move]
            ));

        }
        void HeartPickup()
        {
            Debug.Log(thisCharacterCDH.health);
            thisCharacterCDH.health += 3;
            if (thisCharacterCDH.CheckIfCharacterIsDead() == false)
                endTurn();
        }
    }
    class ActionDataClass
    {
        public AbilityName abilityName;
        public Action actionOfMove;
        public bool needsButton;
        public bool gameObjectHere;
        public bool walkableTileHere;
        public ActionDataClass()
        {
        }
        public ActionDataClass(AbilityName abilityName, Action actionOfMove, bool needsButton, bool GameObjectHere, bool WalkableTileHere)
        {
            this.abilityName = abilityName;
            this.actionOfMove = actionOfMove;
            this.needsButton = needsButton;
            this.gameObjectHere = GameObjectHere;
            this.walkableTileHere = WalkableTileHere;
        }
    }

    public void doAction(AbilityName abilityName)
    {
        ActionDataClass thisADL = aDCL[abilityName];
        Action actionOfMove = thisADL.actionOfMove;
        bool needsButton = thisADL.needsButton;
        bool GameObjectHere = thisADL.gameObjectHere;
        bool WalkableTileHere = thisADL.walkableTileHere;
        //int rangeOfAction = thisADL.rangeOfAction;
        int rangeOfAction = thisCharacterCDH.GetAbilityRange(abilityName);

        StartCoroutine(waitUntileButton());

        IEnumerator waitUntileButton()
        {
            listOfValidtargets = getValidTargetList(GameObjectHere, WalkableTileHere, rangeOfAction);
            if (thisCharacterCDH.isPlayerCharacter && needsButton)
            {
                reticalManager.reDrawValidTiles(listOfValidtargets);//this sets the Valid Tiles Overlay
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));//this waits for MB1 to be pressed before processeding
                actionOfMove();
                reticalManager.reDrawValidTiles(null);//this clears out the Valid Tiles Overlay
            }
            else
            {
                //Debug.Log("Waiting");
                yield return new WaitForSeconds(0.25f);
                actionOfMove();
            }
            reticalManager.reDrawShadows();
        }
    }
    IEnumerator getInput(Action doThisAction, ActionDataClass thisADL)
    {
        //Debug.Log("Get Input Works");
        listOfValidtargets = getValidTargetList(thisADL.gameObjectHere, thisADL.walkableTileHere, thisCharacterCDH.GetAbilityRange(thisADL.abilityName));
        if (thisCharacterCDH.isPlayerCharacter && thisADL.needsButton)
            reticalManager.reDrawValidTiles(listOfValidtargets);//this sets the Valid Tiles Overlay

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));//this waits for MB1 to be pressed before processeding
        if (GetDataForActions())
        {
            doThisAction();

        }
        reticalManager.reDrawValidTiles(null);
        reticalManager.reDrawShadows();
    }
    Vector3Int tryHere;
    List<Vector3Int> listOfValidtargets;
    bool GetDataForActions()
    {
        if (thisCharacterCDH.isPlayerCharacter)
        {
            tryHere = reticalManager.getMovePoint();
            if (listOfValidtargets.Contains(tryHere))
                return true;
            else
                return false;
        }
        else
        {
            tryHere = thisCharacterCDH.getTarget(listOfValidtargets);
            return true;
        }
    }
    List<Vector3Int> getValidTargetList(bool GameObjectHere, bool WalkableTileHere, int rangeOfAction)
    {
        //Debug.Log("Generating List of valid Targets for the character" + thisCharacter.name);
        Vector3Int centerPos = universalCalculator.convertToVector3Int(thisCharacter.transform.position);
        List<Vector3Int> listOfRanges = universalCalculator.generateRangeFromPoint(centerPos, rangeOfAction);
        List<Vector3Int> listOfNonNullTiles = new List<Vector3Int>(mapManager.cellDataDir.Keys);
        listOfRanges = universalCalculator.filterOutList(listOfRanges, listOfNonNullTiles);

        //The Following Removes Invalid Tiles
        for (int i = 0; i < listOfRanges.Count; i++)
        {
            //Normal Checks         
            bool isWalkableHere = mapManager.checkAtPosIfCharacterCanWalk(listOfRanges[i], thisCharacterCDH);
            bool isGameObjectHere = mapManager.cellDataDir[listOfRanges[i]].isCellHoldingCharacer();

            if (isWalkableHere == WalkableTileHere && isGameObjectHere == GameObjectHere)
            {
                //Debug.Log(listOfAttackRange[i] + "Valid " + i);
                //Do Nothing since all conditions are fine
            }
            else
            {
                //Debug.Log(listOfAttackRange[i] + "Invalid ");
                listOfRanges.RemoveAt(i);
                i--;
                bool debugThis = false;
                if (debugThis)
                {
                    bool condtion = false;//Will be reassigned later
                    string needConditon = (condtion) ? "Need " : "Does Not Need ";//Used to Concatinate String
                    if (isWalkableHere != WalkableTileHere)
                    {
                        condtion = WalkableTileHere;
                        //Debug.Log(needConditon + "Walkable Tile Here " + " but isWalkableHere is " + isWalkableHere);
                        Debug.Log(needConditon + "Walkable Tile Here ");
                    }
                    if (isGameObjectHere != GameObjectHere)
                    {
                        condtion = GameObjectHere;
                        //Debug.Log(needConditon + "Game Object Here " + " but isGameObjectHere is " + isGameObjectHere);
                        Debug.Log(needConditon + "Game Object Here ");
                    }

                }
            }
        }
        return listOfRanges;
    }
}
public enum AbilityName
{
    Move,
    Attack,
    EndTurn,
    OpenInventory,
    CloseInventory,
    FireBall,
    HeartPickup
}