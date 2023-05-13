using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonManager : MonoBehaviour
{
    [SerializeField] List<GameObject> ActionButtons;
    TurnManager turnManager;
    ReticalManager reticalManager;
    MapManager mapManager;
    MoveDictionaryManager moveDictionaryManager;
    UniversalCalculator universalCalculator;
    void Awake()
    {
        //Debug.Log("ButtonManager Awake!");
        setVariables();
    }
    public void setVariables()
    {
        turnManager = this.GetComponent<TurnManager>();
        reticalManager = this.GetComponent<ReticalManager>();
        mapManager = this.GetComponent<MapManager>();
        moveDictionaryManager = this.GetComponent<MoveDictionaryManager>();
        universalCalculator = GetComponent<UniversalCalculator>();

        buttonHight = ButtonPrefab.GetComponent<RectTransform>().rect.height;

    }
    public void clearButtons()
    {
        for (int i = 0; i < ActionButtons.Count; i++)
            Destroy(ActionButtons[i]);
        ActionButtons.Clear();
    }
    [SerializeField] GameObject ButtonPrefab;
    [SerializeField] GameObject ButtonHolder;

    [SerializeField] float buttonSpacing = 0;
    [SerializeField] float buttonHight;
    public void InstantiateButtons(List<AbilityName> abilityOfCharacter)
    {
        clearButtons();
        for (int i = 0; i < abilityOfCharacter.Count; i++)
        {
            ActionButtons.Add(Instantiate(ButtonPrefab));//Just Instanting
            //Setting Transforms
            ActionButtons[i].transform.SetParent(ButtonHolder.transform, false);
            ActionButtons[i].transform.localPosition = new Vector3(0, -i * (buttonHight + buttonSpacing), 0);
            //getting TMP to assign Text
            TMPro.TextMeshProUGUI TMPthis;
            TMPthis = ActionButtons[i].transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
            // getting cache for captured variables
            //int captured = i;//no Longer needed

            AbilityName abilityName = abilityOfCharacter[i];
            string stringOfAbilityName = abilityName.ToString();//This is the cache now
            stringOfAbilityName = universalCalculator.CamelCaseToSpaces(stringOfAbilityName);
            // using variables to set text
            TMPthis.text = stringOfAbilityName;
            ActionButtons[i].name = stringOfAbilityName + " Button";

            //Assigning On Click Functions
            Button thisButton = ActionButtons[i].GetComponent<Button>();
            thisButton.onClick.RemoveAllListeners();
            thisButton.onClick.AddListener(delegate
            {
                moveDictionaryManager.doAction(abilityName);
                //Debug.Log("Button clicked: " + listCDHTEXT);
            });
        }
    }
}
