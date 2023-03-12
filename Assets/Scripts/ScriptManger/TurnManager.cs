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

    private TMPro.TextMeshProUGUI turnCountTMP;
    private ButtonManager thisButtonManager;
    private MapManager mapManager;
    MoveDictionaryManager moveDictionaryManager;
    ReticalManager reticalManager;

    void GetGameObjects()
    {
        OrderOfInteractableCharacters = new List<GameObject>();
        tempSpawnPoint = new List<Vector3Int>();
        turnCountTMP = TurnCount.GetComponent<TMPro.TextMeshProUGUI>();
        thisButtonManager = this.gameObject.GetComponent<ButtonManager>();
        thisButtonManager.setButtonManagerVariables();
        mapManager = this.GetComponent<MapManager>();
        mapManager.setTileDictionary();
        moveDictionaryManager = this.GetComponent<MoveDictionaryManager>();
        moveDictionaryManager.setMoveDictionaryManagerVariables();
        reticalManager = this.GetComponent<ReticalManager>();
        reticalManager.setReticalMangerVariables();
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
    public GameObject TurnCount;
    characterDataHolder thisCharacterData;
    int TurnCountInt = 0;
    public void beginTurn()
    {

        turnCountTMP.text = (TurnCountInt + "");
        if (OrderOfInteractableCharacters.Count == TurnCountInt)//this works because Count Starts from 1 not 0
        {
            //Debug.Log(OrderOfInteractableCharacters.Count + " " + TurnCountInt);
            Debug.Log("Game Over");
            thisButtonManager.clearButtons();

        }
        else
            beginTurnThisCharacter();
        /*
        if (OrderOfInteractableCharacters[TurnCountInt])
        {
            beginTurnThisCharacter();
        }
        else if (thisCharacter == null)
        {
            Debug.Log(TurnCountInt + " has been Skipped");
            endTurn();
        }
        */
    }
    void beginTurnThisCharacter()
    {
        thisCharacter = OrderOfInteractableCharacters[TurnCountInt];//updateing thisCharacterReffrence
        thisCharacterData = thisCharacter.gameObject.GetComponent<characterDataHolder>();
        thisCharacterData.BeginThisCharacterTurn();
        //thisButtonManager.makeButtons();
    }
    int TurnLoop = 1;
    public void endTurn()
    {
        //PositionToGameObjectCopy = mapManager.PositionToGameObject;
        TurnCountInt++;
        //if (TurnCountInt / TurnLoop >= PositionToGameObjectCopy.Count)
        if (TurnCountInt >= OrderOfInteractableCharacters.Count)
        {
            recalculateOrder();
            TurnLoop++;
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
        foreach (var position in PositionToGameObjectCopy)
        {
            if (shadowrange.Contains(position.Key))
            {
                OrderOfInteractableCharacters.Add(position.Value);
                Debug.Log("Valid Target found called " + position.Value.name + " at Pos " + position.Key);
            }
            else
            {
                Debug.Log("invalid Target found called " + position.Value.name + " at Pos " + position.Key);
            }
            //OrderOfInteractableCharacters.Add(PositionToGameObjectCopy[position.Key]);
        }

    }
}
