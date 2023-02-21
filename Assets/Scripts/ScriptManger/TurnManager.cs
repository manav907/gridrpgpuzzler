using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject TurnCount;
    int TurnCountInt = 0;
    public List<GameObject> allInteractableCharacters;
    public List<GameObject> OrderOfInteractableCharacters;
    public GameObject characterHolder;
    void Start()
    {
        InstantiateallIntractableCharacters();
        recalculateOrder();
        GetGameObjects();
        beginTurn();
    }
    public GameObject characterPrefab;
    public int numberOfCharacterToInstansitate = 1;
    void InstantiateallIntractableCharacters()
    {
        for (int i = 0; i < numberOfCharacterToInstansitate; i++)
        {
            allInteractableCharacters.Add(Instantiate(characterPrefab));
            allInteractableCharacters[i].transform.SetParent(characterHolder.transform, false);
        }
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
    public GameObject characterThisTurn;
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
