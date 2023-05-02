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
    [SerializeField] basicCameraController basicCameraController;
    [Header("Character Prefab Data to Instanstace Characters")]
    [SerializeField] GameObject characterPrefab;
    [SerializeField] GameObject characterHolder;
    [Header("Level Data")]
    [SerializeField] LevelDataSO loadThisLevel;
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
        gameController = this.gameObject;


        buttonManager.setVariables();
        mapManager.setVariables();
        moveDictionaryManager.setVariables();
        reticalManager.setVariables();
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
        var shadowrange = reticalManager.reDrawShadows();
        if (noCharactersInCamera(shadowrange))
            triggerGameEnd();
        else
        {
            setCharacterData();
            if (shadowrange.Contains(thisCharacterData.getCharV3Int()))
            {
                thisCharacterData.BeginThisCharacterTurn();
            }
            else
            {
                Debug.Log("Turn Skiped");
                endTurn();
            }
        }
        void triggerGameEnd()
        {
            Debug.Log("Game Over");
            buttonManager.clearButtons();
        }
        void setCharacterData()
        {
            thisCharacter = OrderOfInteractableCharacters[TurnCountInt];//updateing thisCharacterReffrence
            thisCharacterData = thisCharacter.gameObject.GetComponent<CharacterControllerScript>();
            moveDictionaryManager.getThisCharacterData();
        }
    }
    bool noCharactersInCamera(List<Vector3Int> thislist)
    {
        int numberofcharactershere = 0;
        foreach (GameObject thischar in OrderOfInteractableCharacters)
        {
            if (thislist.Contains(universalCalculator.convertToVector3Int(thischar.transform.position)))
            {
                numberofcharactershere++;
            }
        }
        if (numberofcharactershere == 0)
            return true;
        else
            return false;
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
    void recalculateOrder()
    {
        var shadowrange = reticalManager.reDrawShadows();
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


