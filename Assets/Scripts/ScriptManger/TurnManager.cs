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
        recalculateOrder();//can only be called after Instanstiating the Characterts
        beginTurnIfPossible();
    }
    [Header("Reffrences to Important Game Objects")]
    GameObject gameController;
    ButtonManager buttonManager;
    MapManager mapManager;
    MoveDictionaryManager moveDictionaryManager;
    ReticalManager reticalManager;
    UniversalCalculator universalCalculator;
    TurnManager turnManager;
    DataManager dataManager;
    [SerializeField] BasicCameraController basicCameraController;
    [Header("Character Prefab Data to Instanstace Characters")]
    [SerializeField] GameObject characterPrefab;
    [SerializeField] GameObject characterHolder;
    [Header("Level Data")]
    public LevelDataSO loadThisLevel;
    [SerializeField] List<CharacterData> listOfCD;
    [Header("List of Active Characters")]
    public List<GameObject> ListOfInteractableCharacters;
    public List<GameObject> OrderOfInteractableCharacters;
    [Header("Current Turn Data")]
    public GameObject thisCharacter;
    CharacterControllerScript thisCharacterData;
    [SerializeField] int TurnCountInt = 0;
    void GetGameObjects()
    {
        OrderOfInteractableCharacters = new List<GameObject>();
        ListOfInteractableCharacters = new List<GameObject>();
        buttonManager = this.gameObject.GetComponent<ButtonManager>();
        mapManager = this.GetComponent<MapManager>();
        moveDictionaryManager = this.GetComponent<MoveDictionaryManager>();
        reticalManager = this.GetComponent<ReticalManager>();
        universalCalculator = this.GetComponent<UniversalCalculator>();
        dataManager = GetComponent<DataManager>();

        gameController = this.gameObject;


        buttonManager.setVariables();
        dataManager.setVariables();
        mapManager.setVariables();
        moveDictionaryManager.setVariables();
        reticalManager.setVariables();
        universalCalculator.setVariables();
        basicCameraController.setVariables(gameController);
    }
    void InstantiateallIntractableCharacters()
    {
        loadThisLevel.LoadData();
        foreach (var characterDataPair in loadThisLevel.posToCharacterData)
        {
            GameObject InstansiatedCharacter = Instantiate(characterPrefab);//Instansiateding Character
            CharacterControllerScript InstansiatedCCS = InstansiatedCharacter.GetComponent<CharacterControllerScript>();//Getting CCS
            //Setting data
            InstansiatedCharacter.transform.position = characterDataPair.Key; //The Locatrion of Character
            InstansiatedCCS.CharacterDataSO = characterDataPair.Value;//Setting The Character Data
            InstansiatedCCS.CharacterDataSO.InstanceID = InstansiatedCharacter.GetInstanceID();//Setting Instance ID
            //Adding to Lists
            ListOfInteractableCharacters.Add(InstansiatedCharacter);//Adding Character to List
            mapManager.cellDataDir[characterDataPair.Key].characterAtCell = InstansiatedCharacter;//Setting Mapmanager Posistion
            InstansiatedCharacter.transform.SetParent(characterHolder.transform, false);//Setting Parent GameObjects 
            //last step
            InstansiatedCCS.InitilizeCharacter(gameController);//Last step Initilization
        }
    }
    public void beginTurnIfPossible()
    {
        ///reticalManager.reDrawShadows();
        if (noPlayerCharacterRemaining())
            triggerGameEnd();
        else
        {
            setCharacterData();
            thisCharacterData.BeginThisCharacterTurn();
        }
        void triggerGameEnd()
        {
            Debug.Log("Game Over");
            //buttonManager.clearButtons();
            buttonManager.InstantiateButtons(new List<AbilityName>() { AbilityName.Restart });
        }
        void setCharacterData()
        {
            thisCharacter = OrderOfInteractableCharacters[TurnCountInt];//updateing thisCharacterReffrence
            thisCharacterData = thisCharacter.gameObject.GetComponent<CharacterControllerScript>();
            moveDictionaryManager.getThisCharacterData();
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
    public void setCameraPos(Vector3 pos)
    {
        basicCameraController.setCameraPos(pos);
    }
    public void endTurn()
    {
        TurnCountInt++;
        if (TurnCountInt >= OrderOfInteractableCharacters.Count)
        {
            recalculateOrder();
            TurnCountInt = 0;
        }
        else
        {
            //Debug.Log(TurnCountInt + " " + TurnLoop + " " + PositionToGameObjectCopy.Count);
        }
        beginTurnIfPossible();
    }
    [Header("Turn Stuff")]
    public int TurnLoop = -1;
    void recalculateOrder()
    {
        //reticalManager.reDrawShadows();
        TurnLoop++;
        OrderOfInteractableCharacters = SortBySpeed(ListOfInteractableCharacters);

        List<GameObject> SortBySpeed(List<GameObject> thisList)
        {
            SortedList<float, GameObject> sortedList = universalCalculator.sortListWithVar(thisList, speed);
            thisList = sortedList.Values.ToList();
            thisList.Reverse();
            return thisList;
            //this list needs to be reversed as higher speed characters should move faster
            //declaring necessary function to be used as a delegate
            float speed(GameObject thisGameObject)
            {
                return thisGameObject.GetComponent<CharacterControllerScript>().speedValue;
            }
        }
    }
}


