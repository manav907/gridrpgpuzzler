using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    void Start()
    {
        GetGameObjects();
        InstantiateallIntractableCharacters();//Can only be called after getting game objects
        RecalculateOrder();//can only be called after Instanstiating the Characterts       
        //GameEvents.current.setText("");
        beginTurnIfPossible();
    }
    [Header("Reffrences to Important Game Objects")]
    GameObject gameController;
    MapManager mapManager;
    MoveDictionaryManager moveDictionaryManager;
    ReticalManager reticalManager;
    UniversalCalculator universalCalculator;
    [SerializeField] BasicCameraController basicCameraController;
    [Header("Character Prefab Data to Instanstace Characters")]
    [SerializeField] GameObject characterPrefab;
    [SerializeField] GameObject characterHolder;
    [Header("List of Active Characters")]
    public List<GameObject> ListOfInteractableCharacters;
    public List<GameObject> OrderOfInteractableCharacters;
    [Header("Current Turn Data")]
    public static GameObject thisCharacter;
    CharacterControllerScript thisCharacterData;
    int TurnCountInt = 0;
    void GetGameObjects()
    {
        OrderOfInteractableCharacters = new List<GameObject>();
        ListOfInteractableCharacters = new List<GameObject>();
        mapManager = this.GetComponent<MapManager>();
        moveDictionaryManager = this.GetComponent<MoveDictionaryManager>();
        reticalManager = this.GetComponent<ReticalManager>();
        universalCalculator = this.GetComponent<UniversalCalculator>();

        gameController = this.gameObject;

        mapManager.SetVariables();
        moveDictionaryManager.SetVariables();
        reticalManager.setVariables();
        basicCameraController.setVariables(gameController);

    }
    void InstantiateallIntractableCharacters()
    {

        var dict = mapManager.LoadThisLevel.GenerateV3IntToCharacterDataDir();
        foreach (var characterDataPair in dict)
        {
            GameObject InstansiatedCharacter = Instantiate(characterPrefab);//Instansiateding Character
            CharacterControllerScript InstansiatedCCS = InstansiatedCharacter.GetComponent<CharacterControllerScript>();//Getting CCS
            //Setting data
            InstansiatedCharacter.transform.position = characterDataPair.Key; //The Location of Character                          
            InstansiatedCCS.CharacterDataSO = characterDataPair.Value;//Setting Character SO
            InstansiatedCCS.InitilizeCharacter(gameController);//Setting Varirables including initilinging SO

            InstansiatedCCS.name = InstansiatedCCS.characterName + " " + InstansiatedCharacter.GetInstanceID();//Setting Instance ID         
            //Adding to Lists
            ListOfInteractableCharacters.Add(InstansiatedCharacter);//Adding Character to List
            if (mapManager.cellDataDir.ContainsKey(characterDataPair.Key))//Checks if Tile is Valid ie recorded in Dictionary does not do walkablity checks
            {
                mapManager.PlaceCharacterAtPos(characterDataPair.Key, InstansiatedCharacter);
            }//Setting Mapmanager Posistion
            else
            {
                Debug.Log("Wrong Tile Skipping Character");
            }
            InstansiatedCharacter.transform.SetParent(characterHolder.transform, false);//Setting Parent GameObjects 
        }
        GameEvents.current.SetUpCamera();//Move This Somewhere Else
    }
    public void beginTurnIfPossible()
    {
        ///reticalManager.reDrawShadows();
        if (noPlayerCharacterRemaining())
            triggerGameEnd();
        else
        {
            //Debug.Log("New Turn");
            setCharacterData();//sets new chracterData
            thisCharacterData.currentStamina = thisCharacterData.maxStamina;
            thisCharacterData.BeginThisCharacterTurn();

        }
        void triggerGameEnd()
        {
            //Debug.Log("Game Over");
            GameEvents.current.SetText("Game Over");
        }
        void setCharacterData()
        {
            thisCharacter = OrderOfInteractableCharacters[TurnCountInt];//updateing thisCharacterReffrence
            thisCharacterData = thisCharacter.gameObject.GetComponent<CharacterControllerScript>();
            moveDictionaryManager.GetThisCharacterData();
        }
        bool noPlayerCharacterRemaining()
        {
            foreach (GameObject character in ListOfInteractableCharacters)
            {
                if (character.GetComponent<CharacterControllerScript>().isPlayerCharacter)
                {
                    return false;
                }
            }
            return true;
        }
    }
    public void SetCameraPos(Vector3 pos)
    {
        basicCameraController.setCameraPos(pos);
    }

    public void EndTurn()
    {

        GameEvents.current.inGameUI.ClearButtons();
        StartCoroutine(thisCharacterData.animationControllerScript.TrySetNewAnimation(CharacterAnimationState.Idle));
        TurnCountInt++;
        if (TurnCountInt >= OrderOfInteractableCharacters.Count)
        {
            RecalculateOrder();
            TurnCountInt = 0;
        }
        else
        {
            //Debug.Log(TurnCountInt + " " + TurnLoop + " " + PositionToGameObjectCopy.Count);
        }
        beginTurnIfPossible();
    }
    int TurnLoop = -1;
    void RecalculateOrder()
    {
        //reticalManager.reDrawShadows();
        TurnLoop++;
        OrderOfInteractableCharacters = SortBySpeed(ListOfInteractableCharacters);

        List<GameObject> SortBySpeed(List<GameObject> thisList)
        {
            SortedList<float, GameObject> sortedList = universalCalculator.SortListWithVar(thisList, speed, speedTieBreaker);
            thisList = sortedList.Values.ToList();
            thisList.Reverse();
            return thisList;
            //this list needs to be reversed as higher speed characters should move faster
            //declaring necessary function to be used as a delegate
            float speed(GameObject thisGameObject)
            {
                return thisGameObject.GetComponent<CharacterControllerScript>().speedValue;
            }
            float speedTieBreaker(GameObject thisGameObject)
            {
                //return thisGameObject.GetComponent<CharacterControllerScript>().speedValue - 1 / 10f;
                return -1 / 10f;
            }
        }
    }
}
public enum GameStoryState
{
    UnAssignedevent = 0,
    PlayerCharacterTurn,
    EnemyCharacterTurn,
    PlayerPartyDialogEvent,
    PlayerCharacterDialogEvent,
    EnemyCharacterDialogEnent,
    AllCharacterDialogEvent,
    WorldDialogEvent,

}
public enum ControlCharacterState
{

}


