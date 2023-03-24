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
    private ButtonManager buttonManager;
    private MapManager mapManager;
    MoveDictionaryManager moveDictionaryManager;
    ReticalManager reticalManager;
    UniversalCalculator universalCalculator;

    void GetGameObjects()
    {
        OrderOfInteractableCharacters = new List<GameObject>();
        buttonManager = this.gameObject.GetComponent<ButtonManager>();
        mapManager = this.GetComponent<MapManager>();
        moveDictionaryManager = this.GetComponent<MoveDictionaryManager>();
        reticalManager = this.GetComponent<ReticalManager>();
        universalCalculator = this.GetComponent<UniversalCalculator>();

        buttonManager.setVariables();
        mapManager.setVariables();
        moveDictionaryManager.setVariables();
        reticalManager.setVariables();
    }
    public GameObject characterPrefab;
    public int numberOfCharacterToInstansitate = 1;
    [SerializeField]
    List<GameObject> OrderOfInteractableCharacters;
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
    [SerializeField] int TurnCountInt = 0;
    public void beginTurnIfPossible()
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
        else if (noPlayersRemain)
        {
            Debug.Log("No Players Remain");
            triggerGameEnd();
        }
        else
            beginTurnThisCharacter();
    }
    void triggerGameEnd()
    {
        Debug.Log("Game Over");
        buttonManager.clearButtons();
    }

    void beginTurnThisCharacter()
    {
        setCharacterData();
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
    void setCharacterData()
    {
        thisCharacter = OrderOfInteractableCharacters[TurnCountInt];//updateing thisCharacterReffrence
        thisCharacterData = thisCharacter.gameObject.GetComponent<characterDataHolder>();
        buttonManager.getThisCharacterData();
        moveDictionaryManager.getThisCharacterData();

        mapManager.fixErrors();
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
    Dictionary<Vector3Int, GameObject> PositionToGameObjectCopy;
    void recalculateOrder()
    {
        PositionToGameObjectCopy = mapManager.PositionToGameObject;
        var shadowrange = reticalManager.reDrawShadows();
        OrderOfInteractableCharacters.Clear();

        List<bool> isPlayerCharacter = new List<bool>();


        foreach (var position in PositionToGameObjectCopy)
        {

            OrderOfInteractableCharacters.Add(position.Value);
            bool isPlayer = position.Value.GetComponent<characterDataHolder>().isPlayerCharacter;
            if (!isPlayer)
                isPlayerCharacter.Add(isPlayer);
        }
        if (isPlayerCharacter.Count == OrderOfInteractableCharacters.Count)
        {
            noPlayersRemain = true;
        }
        OrderOfInteractableCharacters = universalCalculator.SortBySpeed(OrderOfInteractableCharacters);

    }
    bool noPlayersRemain = false;
}
