using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    void Start()
    {
        InstantiateallIntractableCharacters();
        recalculateOrder();
        GetGameObjects();
        beginTurn();
    }

    private TMPro.TextMeshProUGUI turnCountTMP;
    private ButtonManager thisButtonManager;
    private RayCastReticalManager rayCastReticalManager;

    void GetGameObjects()
    {
        turnCountTMP = TurnCount.GetComponent<TMPro.TextMeshProUGUI>();
        thisButtonManager = this.gameObject.GetComponent<ButtonManager>();
        thisButtonManager.MakeMoveListClassList();
        rayCastReticalManager = this.gameObject.GetComponent<RayCastReticalManager>();
        rayCastReticalManager.SetDictionary();
    }
    public GameObject characterPrefab;
    public int numberOfCharacterToInstansitate = 1;
    public List<GameObject> allInteractableCharacters;
    public List<GameObject> OrderOfInteractableCharacters;
    public GameObject characterHolder;
    void InstantiateallIntractableCharacters()
    {
        for (int i = 0; i < numberOfCharacterToInstansitate; i++)
        {
            allInteractableCharacters.Add(Instantiate(characterPrefab));
            allInteractableCharacters[i].transform.SetParent(characterHolder.transform, false);
            allInteractableCharacters[i].transform.position += i * Vector3.right;
            allInteractableCharacters[i].name += i;
        }
        AddCharactersToDictionaryAfterInstantiating();
    }
    Dictionary<Vector3, GameObject> PositionToGameObject;
    void AddCharactersToDictionaryAfterInstantiating()
    {
        PositionToGameObject = new Dictionary<Vector3, GameObject>();
        foreach (GameObject character in allInteractableCharacters)
        {
            PositionToGameObject.Add(character.transform.position, character);
        }
    }
    public GameObject isDictionarySpaceOccupied(Vector3 checkhere)
    {
        if (PositionToGameObject.ContainsKey(checkhere))
            return PositionToGameObject[checkhere];
        else
            return null;
    }

    [SerializeField] private List<Vector3> PositionToGameObjectVector3;
    [SerializeField] private List<GameObject> PositionToGameObjectGameObjects;
    public void UpdateCharacterPosition(Vector3 previousPosition, Vector3 newPosition, GameObject thisCharacter)
    {
        PositionToGameObject.Remove(previousPosition);
        PositionToGameObject.Add(newPosition, thisCharacter);
        PositionToGameObjectGameObjects.Clear();
        PositionToGameObjectVector3.Clear();
        foreach (Vector3 position in PositionToGameObject.Keys)
        {
            PositionToGameObjectGameObjects.Add(PositionToGameObject[position]);
            PositionToGameObjectVector3.Add(position);
        }
    }

    public GameObject characterThisTurn;
    public GameObject TurnCount;
    int TurnCountInt = 0;
    public void beginTurn()
    {
        characterThisTurn = OrderOfInteractableCharacters[TurnCountInt];
        turnCountTMP.text = (TurnCountInt + "");
        thisButtonManager.makeButtons();

    }

    int TurnLoop = 1;
    public void endTurn()
    {
        TurnCountInt++;
        if (TurnCountInt / TurnLoop == allInteractableCharacters.Count)
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
    void recalculateOrder()
    {
        for (int i = 0; i < allInteractableCharacters.Count; i++)
        {
            OrderOfInteractableCharacters.Add(allInteractableCharacters[i]);
        }
    }

}
