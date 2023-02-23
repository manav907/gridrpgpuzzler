using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ButtonManager : MonoBehaviour
{
    public GameObject ButtonPrefab;
    public List<GameObject> ActionButtons;
    public GameObject ButtonHolder;
    GameObject thisCharacter;


    public void makeButtons()
    {
        setVariable();
        clearButtons();
        InstantiateButtons();
    }
    TurnManager turnManager;
    ReticalManager rayCastReticalManager;
    MapManager mapManager;
    void setVariable()
    {
        turnManager = this.GetComponent<TurnManager>();
        thisCharacter = this.GetComponent<TurnManager>().characterThisTurn;
        rayCastReticalManager = this.GetComponent<ReticalManager>();
        mapManager = this.GetComponent<MapManager>();
    }
    public class MoveListClass
    {
        public string moveName;
        public Action action;
    }
    public MoveListClass thatUniquieMove;
    public List<MoveListClass> moveLists;
    public void MakeMoveListClassList()
    {
        moveLists = new List<MoveListClass>();
        moveLists.AddRange(new List<MoveListClass>
        {
            new MoveListClass { moveName = "Move", action = MoveCharacter },
            new MoveListClass { moveName = "Attack", action = AttackHere },
            new MoveListClass { moveName = "End Turn", action = endTurn }
        });
    }
    void clearButtons()
    {
        for (int i = 0; i < ActionButtons.Count; i++)
            Destroy(ActionButtons[i]);
        ActionButtons.Clear();
    }
    void InstantiateButtons()
    {
        for (int i = 0; i < moveLists.Count; i++)
        {
            ActionButtons.Add(Instantiate(ButtonPrefab));
            ActionButtons[i].transform.SetParent(ButtonHolder.transform, false);
            ActionButtons[i].transform.localPosition = new Vector3(ActionButtons[i].transform.localPosition.x, -50 * i);
            TMPro.TextMeshProUGUI TMPthis;
            TMPthis = ActionButtons[i].transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
            TMPthis.text = moveLists[i].moveName;
            ActionButtons[i].name = moveLists[i].moveName + " Button";
            int captured = i;
            ActionButtons[i].GetComponent<Button>().onClick.AddListener(delegate { moveLists[captured].action(); });
        }
    }

    IEnumerator waitUntileButton(Action action)
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        action();
    }

    Vector3Int tryHere;
    Dictionary<Vector3, GameObject> PositionToGameObject;
    void GetDataForActions()
    {
        tryHere = rayCastReticalManager.getMovePoint();//CriticalData
        PositionToGameObject = mapManager.PositionToGameObject;
    }

    void MoveCharacter()
    {

        void thisAction()
        {
            GetDataForActions();
            if (PositionToGameObject.ContainsKey(tryHere))//Checks if There is a Game Object Here
            {
                Debug.Log("Cant do that there is " + PositionToGameObject[tryHere] + " is Occuping this Space");
            }
            else if (!mapManager.getIsWalkable(tryHere))
            {
                Debug.Log("Not Walable Bro");
            }
            else
            {
                Vector3 currentPosition = thisCharacter.transform.position;
                mapManager.UpdateCharacterPosition(currentPosition, tryHere, thisCharacter);
                thisCharacter.transform.position = tryHere;
            }
        }
        StartCoroutine(waitUntileButton(thisAction));//the co routine starts the action not all actions need a co routine
    }
    void AttackHere()
    {
        void thisAction()
        {
            GetDataForActions();
            if (!PositionToGameObject.ContainsKey(tryHere))
            {
                Debug.Log("no Character Here");
            }
            else
            {
                characterDataHolder targetCharacter = PositionToGameObject[tryHere].gameObject.GetComponent<characterDataHolder>();
                characterDataHolder attackingCharacter = thisCharacter.GetComponent<characterDataHolder>();
                targetCharacter.health -= attackingCharacter.AttackDamage;
                targetCharacter.UpdateCharacterData();
            }
        }
        StartCoroutine(waitUntileButton(thisAction));
    }
    void endTurn()
    {
        this.GetComponent<TurnManager>().endTurn();
    }
}
