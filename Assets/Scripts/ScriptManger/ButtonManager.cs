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
    void setVariable()
    {
        thisCharacter = this.GetComponent<TurnManager>().characterThisTurn;
        rayCastReticalManager = this.GetComponent<RayCastReticalManager>();
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
            //Buttons[i] = Instantiate(ButtonPrefab);
            ActionButtons[i].transform.SetParent(ButtonHolder.transform, false);
            ActionButtons[i].transform.localPosition = new Vector3(ActionButtons[i].transform.localPosition.x, -50 * i);
            TMPro.TextMeshProUGUI TMPthis;
            TMPthis = ActionButtons[i].transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
            TMPthis.text = moveLists[i].moveName;
            ActionButtons[i].name = moveLists[i].moveName + " Button";
            int captured = i;
            ActionButtons[i].GetComponent<Button>().onClick.AddListener(delegate { moveLists[captured].action(); });
            //ActionButtons[i].GetComponent<Button>().onClick.AddListener(delegate { StartCoroutine(waitUntileButton(moveLists[captured].action)); });
        }
    }

    IEnumerator waitUntileButton(Action action)
    {
        //Debug.Log("Waiting");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        //Debug.Log("Pressed button0");
        action();
    }
    RayCastReticalManager rayCastReticalManager;

    void MoveCharacter()
    {
        void thisAction()
        {
            Vector3Int tryMoveHere = rayCastReticalManager.getMovePoint();
            bool isWalkable = rayCastReticalManager.checkOrder(tryMoveHere);
            if (isWalkable)
            {
                thisCharacter.transform.position = tryMoveHere;
            }
            Debug.Log(isWalkable);
        }
        StartCoroutine(waitUntileButton(thisAction));
    }
    void AttackHere()
    {
        void thisAction()
        {
            Debug.Log("Characert Attact at" + rayCastReticalManager.getMovePoint());
        }
        StartCoroutine(waitUntileButton(thisAction));
    }
    void endTurn()
    {
        Debug.Log("Turn Ended" + thisCharacter.transform.position);
        this.GetComponent<TurnManager>().endTurn();
    }
}
