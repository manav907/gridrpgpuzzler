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
    public void setVariables()
    {
        turnManager = this.GetComponent<TurnManager>();
        reticalManager = this.GetComponent<ReticalManager>();
        mapManager = this.GetComponent<MapManager>();
        moveDictionaryManager = this.GetComponent<MoveDictionaryManager>();
    }

    GameObject thisCharacter;
    characterDataHolder thisCharacterCDH;
    public void getThisCharacterData()
    {
        thisCharacter = turnManager.thisCharacter;
        thisCharacterCDH = thisCharacter.GetComponent<characterDataHolder>();
    }
    public void clearButtons()
    {
        for (int i = 0; i < ActionButtons.Count; i++)
            Destroy(ActionButtons[i]);
        ActionButtons.Clear();
    }
    public void InstantiateButtons(List<String> listFromCDH)
    {
        var Dictionary = moveDictionaryManager.aDCL;
        for (int i = 0; i < listFromCDH.Count; i++)
        {
            ActionButtons.Add(Instantiate(ButtonPrefab));//Just Instanting
            //Setting Transforms
            ActionButtons[i].transform.SetParent(ButtonHolder.transform, false);
            ActionButtons[i].transform.localPosition = new Vector3(ActionButtons[i].transform.localPosition.x, -50 * i);
            //getting TMP to assign Text
            TMPro.TextMeshProUGUI TMPthis;
            TMPthis = ActionButtons[i].transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
            // getting cache for captured variables
            //int captured = i;//no Longer needed
            string listCDHTEXT = listFromCDH[i];//This is the cache now
            // using variables to set text
            TMPthis.text = listCDHTEXT;
            ActionButtons[i].name = listCDHTEXT + " Button";

            //Assigning On Click Functions
            ActionButtons[i].GetComponent<Button>().onClick.AddListener(delegate
            {
                moveDictionaryManager.doAction(listCDHTEXT);
            });
        }
    }


}
