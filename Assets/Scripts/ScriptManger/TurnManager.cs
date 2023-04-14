using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    void Start()
    {
        GetGameObjects();
        InstantiateallIntractableCharacters();//Can only be called after getting game objects
        recalculateOrder();//can only be called after Instanstiating the Characterts
        beginTurnIfPossible();
    }
    GameObject gameController;
    ButtonManager buttonManager;
    MapManager mapManager;
    MoveDictionaryManager moveDictionaryManager;
    ReticalManager reticalManager;
    UniversalCalculator universalCalculator;
    TurnManager turnManager;
    [SerializeField] basicCameraController basicCameraController;


    void GetGameObjects()
    {
        OrderOfInteractableCharacters = new List<GameObject>();
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
    [SerializeField] GameObject characterPrefab;

    [SerializeField] GameObject characterHolder;
    [SerializeField] List<CharacterData> listOfCD;
    void InstantiateallIntractableCharacters()
    {
        List<GameObject> allInteractableCharacters = new List<GameObject>();
        for (int i = 0; i < listOfCD.Count; i++)
        {
            allInteractableCharacters.Add(Instantiate(characterPrefab));
            GameObject thisChar = allInteractableCharacters[i];
            thisChar.transform.SetParent(characterHolder.transform, false);
            thisChar.transform.position += i * Vector3.right;
            thisChar.name += i;

            //Assigning CharacterData
            CharacterControllerScript thisCDH = thisChar.GetComponent<CharacterControllerScript>();
            thisCDH.thisCharacterData = listOfCD[i];
            thisCDH.thisCharacterData.InstanceID = i;
            thisCDH.InitilizeCharacter(gameController);
            OrderOfInteractableCharacters.Add(thisChar);

            Vector3Int thisPos = universalCalculator.convertToVector3Int(thisChar.transform.position);
            mapManager.cellDataDir[thisPos].characterAtCell = thisChar;
        }
    }
    public List<GameObject> OrderOfInteractableCharacters;
    public GameObject thisCharacter;
    CharacterControllerScript thisCharacterData;
    [SerializeField] int TurnCountInt = 0;
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
    }

    void setCharacterData()
    {
        thisCharacter = OrderOfInteractableCharacters[TurnCountInt];//updateing thisCharacterReffrence
        thisCharacterData = thisCharacter.gameObject.GetComponent<CharacterControllerScript>();
        moveDictionaryManager.getThisCharacterData();
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
        OrderOfInteractableCharacters = universalCalculator.SortBySpeed(OrderOfInteractableCharacters);
    }
}
