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
        beginTurn();
    }
    private ButtonManager thisButtonManager;
    private MapManager mapManager;
    MoveDictionaryManager moveDictionaryManager;
    ReticalManager reticalManager;
    TileCalculator tileCalculator;

    void GetGameObjects()
    {
        OrderOfInteractableCharacters = new List<GameObject>();
        tempSpawnPoint = new List<Vector3Int>();
        thisButtonManager = this.gameObject.GetComponent<ButtonManager>();
        thisButtonManager.setButtonManagerVariables();
        mapManager = this.GetComponent<MapManager>();
        mapManager.setTileDictionary();
        moveDictionaryManager = this.GetComponent<MoveDictionaryManager>();
        moveDictionaryManager.setMoveDictionaryManagerVariables();
        reticalManager = this.GetComponent<ReticalManager>();
        reticalManager.setReticalMangerVariables();
        tileCalculator = this.GetComponent<TileCalculator>();
    }
    public GameObject characterPrefab;
    public int numberOfCharacterToInstansitate = 1;
    [SerializeField]
    List<GameObject> OrderOfInteractableCharacters;
    List<Vector3Int> tempSpawnPoint;
    [SerializeField] private GameObject characterHolder;
    void InstantiateallIntractableCharacters()
    {
        List<GameObject> allInteractableCharacters = new List<GameObject>();
        for (int i = 0; i < numberOfCharacterToInstansitate; i++)
        {
            allInteractableCharacters.Add(Instantiate(characterPrefab));
            allInteractableCharacters[i].transform.SetParent(characterHolder.transform, false);
            allInteractableCharacters[i].transform.position += i * Vector3.right;
            allInteractableCharacters[i].name += i;
            allInteractableCharacters[i].GetComponent<characterDataHolder>().InitilizeCharacter(this.gameObject);
        }
        mapManager.AddCharactersToDictionaryAfterInstantiating(allInteractableCharacters);
    }
    public GameObject thisCharacter;
    characterDataHolder thisCharacterData;
    int TurnCountInt = 0;
    public void beginTurn()
    {
        if (OrderOfInteractableCharacters.Count == TurnCountInt)//this works because Count Starts from 1 not 0
        {
            triggerGameEnd();
        }
        else if (OrderOfInteractableCharacters[TurnCountInt] == null)
        {
            Debug.Log("Null char");
            endTurn();
        }
        else
            beginTurnThisCharacter();
    }
    void triggerGameEnd()
    {
        Debug.Log("Game Over");
        thisButtonManager.clearButtons();
    }

    void beginTurnThisCharacter()
    {
        thisCharacter = OrderOfInteractableCharacters[TurnCountInt];//updateing thisCharacterReffrence
        thisCharacterData = thisCharacter.gameObject.GetComponent<characterDataHolder>();

        var shadowrange = reticalManager.reDrawShadows();        
        if (shadowrange.Contains(thisCharacterData.getCharV3Int()))
        {
            thisCharacterData.BeginThisCharacterTurn();
        }
        else
        {
            Debug.Log("Turn Skiped");
            if (noCharactersInCamera(shadowrange))
                triggerGameEnd();
            else
                endTurn();
        }
    }
    bool noCharactersInCamera(List<Vector3Int> thislist)
    {
        int numberofcharactershere = 0;
        foreach (GameObject thischar in OrderOfInteractableCharacters)
        {
            if (thislist.Contains(tileCalculator.convertToVector3Int(thischar.transform.position)))
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
        beginTurn();
    }
    Dictionary<Vector3Int, GameObject> PositionToGameObjectCopy;
    void recalculateOrder()
    {
        PositionToGameObjectCopy = mapManager.PositionToGameObject;
        var shadowrange = reticalManager.reDrawShadows();
        OrderOfInteractableCharacters.Clear();
        foreach (var position in PositionToGameObjectCopy)
        {
            //if (shadowrange.Contains(position.Key))
            OrderOfInteractableCharacters.Add(position.Value);
        }
    }
}
