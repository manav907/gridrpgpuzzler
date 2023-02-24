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
    private ReticalManager reticalManager;
    private MapManager mapManager;

    void GetGameObjects()
    {
        OrderOfInteractableCharacters = new List<GameObject>();
        tempSpawnPoint = new List<Vector3Int>();
        turnCountTMP = TurnCount.GetComponent<TMPro.TextMeshProUGUI>();
        thisButtonManager = this.gameObject.GetComponent<ButtonManager>();
        thisButtonManager.SetMoveDictionary();
        reticalManager = this.gameObject.GetComponent<ReticalManager>();
        mapManager = this.GetComponent<MapManager>();
        mapManager.SetDictionary();
    }
    public GameObject characterPrefab;
    public int numberOfCharacterToInstansitate = 1;

    List<GameObject> OrderOfInteractableCharacters;
    List<Vector3Int> tempSpawnPoint;
    public GameObject characterHolder;
    void InstantiateallIntractableCharacters()
    {
        List<GameObject> allInteractableCharacters = new List<GameObject>();
        for (int i = 0; i < numberOfCharacterToInstansitate; i++)
        {
            allInteractableCharacters.Add(Instantiate(characterPrefab));
            allInteractableCharacters[i].transform.SetParent(characterHolder.transform, false);
            allInteractableCharacters[i].transform.position += i * Vector3.right;
            allInteractableCharacters[i].name += i;
        }
        mapManager.AddCharactersToDictionaryAfterInstantiating(allInteractableCharacters);
        PositionToGameObjectCopy = mapManager.PositionToGameObject;//after SetDictionary
    }


    public GameObject characterThisTurn;
    public GameObject TurnCount;
    int TurnCountInt = 0;
    public void beginTurn()
    {
        characterThisTurn = OrderOfInteractableCharacters[TurnCountInt];
        characterDataHolder targetCharacter = characterThisTurn.gameObject.GetComponent<characterDataHolder>();
        targetCharacter.isCharacterTurn = true;
        targetCharacter.UpdateCharacterData();
        turnCountTMP.text = (TurnCountInt + "");
        thisButtonManager.makeButtons();

    }
    int TurnLoop = 1;
    public void endTurn()
    {
        TurnCountInt++;
        if (TurnCountInt / TurnLoop == PositionToGameObjectCopy.Count)
        {
            recalculateOrder();
            TurnLoop++;
        }
        else
        {
            //Debug.Log(TurnCountInt + " " + TurnLoop);
        }
        beginTurn();
    }
    Dictionary<Vector3, GameObject> PositionToGameObjectCopy;

    void recalculateOrder()
    {

        foreach (var position in PositionToGameObjectCopy)
        {
            OrderOfInteractableCharacters.Add(PositionToGameObjectCopy[position.Key]);
        }
    }

}
