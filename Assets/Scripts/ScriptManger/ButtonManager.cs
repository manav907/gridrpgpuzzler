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
    TurnManager turnManager;
    ReticalManager reticalManager;
    MapManager mapManager;
    MoveDictionaryManager moveDictionaryManager;
    public void setButtonManagerVariables()
    {
        turnManager = this.GetComponent<TurnManager>();
        reticalManager = this.GetComponent<ReticalManager>();
        mapManager = this.GetComponent<MapManager>();
        moveDictionaryManager = this.GetComponent<MoveDictionaryManager>();
    }

    public void clearButtons()
    {
        for (int i = 0; i < ActionButtons.Count; i++)
            Destroy(ActionButtons[i]);
        ActionButtons.Clear();
    }

    GameObject thisCharacter;
    characterDataHolder thisCharacterCDH;
    void getThisCharacterData()
    {
        thisCharacter = turnManager.thisCharacter;
        thisCharacterCDH = thisCharacter.GetComponent<characterDataHolder>();
    }
    public void InstantiateButtons(List<String> listFromCDH)
    {

        getThisCharacterData();
        for (int i = 0; i < listFromCDH.Count; i++)
        {
            ActionButtons.Add(Instantiate(ButtonPrefab));//Just Instanting
            //Setting Transforms
            ActionButtons[i].transform.SetParent(ButtonHolder.transform, false);
            ActionButtons[i].transform.localPosition = new Vector3(ActionButtons[i].transform.localPosition.x, -50 * i);
            //getting TMP to assign Text
            TMPro.TextMeshProUGUI TMPthis;
            TMPthis = ActionButtons[i].transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
            TMPthis.text = listFromCDH[i];
            ActionButtons[i].name = listFromCDH[i] + " Button";

            //Assigning On Click Functions
            int captured = i;
            //if (!MoveNameToActionDictionary.ContainsKey(basicList[captured])) Debug.Log(basicList[captured]+" not Found in Dictionary"); //for Debugging

            ActionButtons[i].GetComponent<Button>().onClick.AddListener(delegate
            {
                moveDictionaryManager.MoveNameToActionDictionary[listFromCDH[captured]]();//Takes string from basicList and gets the Meathod from Dictionary
            });
        }
    }


}
