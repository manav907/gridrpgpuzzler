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
    Dictionary<String, Action> MoveNameToActionDictionary;
    public void SetMoveDictionary()
    {
        MoveNameToActionDictionary = new Dictionary<String, Action>();
        MoveNameToActionDictionary.Add("Move", MoveCharacter);
        MoveNameToActionDictionary.Add("Attack", AttackHere);
        MoveNameToActionDictionary.Add("End Turn", endTurn);
        MoveNameToActionDictionary.Add("FireBall", ThrowFireBall);
    }
    void clearButtons()
    {
        for (int i = 0; i < ActionButtons.Count; i++)
            Destroy(ActionButtons[i]);
        ActionButtons.Clear();
    }
    void InstantiateButtons()
    {
        List<String> basicList = thisCharacter.GetComponent<characterDataHolder>().GetCharacterMoveList();
        for (int i = 0; i < basicList.Count; i++)
        {
            ActionButtons.Add(Instantiate(ButtonPrefab));//Just Instanting
            //Setting Transforms
            ActionButtons[i].transform.SetParent(ButtonHolder.transform, false);
            ActionButtons[i].transform.localPosition = new Vector3(ActionButtons[i].transform.localPosition.x, -50 * i);
            //getting TMP to assign Text
            TMPro.TextMeshProUGUI TMPthis;
            TMPthis = ActionButtons[i].transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
            TMPthis.text = basicList[i];
            ActionButtons[i].name = basicList[i] + " Button";

            //Assigning On Click Functions
            int captured = i;
            //if (!MoveNameToActionDictionary.ContainsKey(basicList[captured])) Debug.Log(basicList[captured]+" not Found in Dictionary"); //for Debugging
            ActionButtons[i].GetComponent<Button>().onClick.AddListener(delegate
            {
                MoveNameToActionDictionary[basicList[captured]]();//Takes string from basicList and gets the Meathod from Dictionary
            });
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
    void ThrowFireBall()
    {
        Debug.Log("Throw Fire Ball");
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
            }
        }
        StartCoroutine(waitUntileButton(thisAction));
    }
    void endTurn()
    {
        characterDataHolder targetCharacter = thisCharacter.gameObject.GetComponent<characterDataHolder>();
        targetCharacter.isCharacterTurn = false;
        targetCharacter.UpdateCharacterData();
        this.GetComponent<TurnManager>().endTurn();
    }
}
