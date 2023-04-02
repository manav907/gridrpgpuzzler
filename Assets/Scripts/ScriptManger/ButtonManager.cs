using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] GameObject ButtonPrefab;
    [SerializeField] List<GameObject> ActionButtons;
    [SerializeField] GameObject ButtonHolder;
    TurnManager turnManager;
    ReticalManager reticalManager;
    MapManager mapManager;
    MoveDictionaryManager moveDictionaryManager;
    void Awake()
    {
        Debug.Log("ButtonManager Awake!");
        setVariables();
    }
    public void setVariables()
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
    public void InstantiateButtons(List<String> listFromCDH)
    {
        clearButtons();
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
            Button thisButton = ActionButtons[i].GetComponent<Button>();
            thisButton.onClick.RemoveAllListeners();
            thisButton.onClick.AddListener(delegate
            {
                //moveDictionaryManager.doAction(listCDHTEXT);
                Debug.Log("Button clicked: " + listCDHTEXT);
            });
        }
    }


}
