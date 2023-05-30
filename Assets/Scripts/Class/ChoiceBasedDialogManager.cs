using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChoiceBasedDialogManager
{
    Dictionary<BranchID, DialogEvent> ChoiceToBranchMap;
    public DialogEvent currentBranch;
    BranchID cuurentBranchID;
    public bool ChangeBranch(Choice newActiontaken)
    {
        if (currentBranch.isChoiceContained(newActiontaken))
        {
            //Debug.Log("Choice Was Available");
            cuurentBranchID.choiceStack.Add(newActiontaken);
            if (ChoiceToBranchMap.ContainsKey(cuurentBranchID))
            {
                currentBranch = ChoiceToBranchMap[cuurentBranchID];
                //Debug.Log("Path Exixts");
                return true;
            }
            else
            {
                //Debug.LogError("Path Does not Exist");
                return false;
            }
        }
        else
        {
            //Debug.Log("Choice Was NOT Available");
            return false;
        }


    }
    public ChoiceBasedDialogManager(SerializableDictionary<BranchID, DialogEvent> ChoiceToBranchMapSD)
    {
        ChoiceToBranchMap = ChoiceToBranchMapSD.returnDict();
        currentBranch = ChoiceToBranchMapSD.Values()[0];
        cuurentBranchID = ChoiceToBranchMapSD.Keys()[0];
    }

}


[System.Serializable]
public class DialogEvent
{

    [SerializeField] public SerializableDictionary<CharacterName, Dialog> OrderOfDialogs;
    [SerializeField] public List<Choice> AvaiableChoices;
    public bool isChoiceContained(Choice newChoic)
    {
        string newChoiceString = newChoic.CheckAction();
        List<string> listOfChoices = new List<string>();
        foreach (Choice choice in AvaiableChoices)
        {
            listOfChoices.Add(choice.CheckAction());
        }
        if (listOfChoices.Contains(newChoiceString))
        {
            return true;
        }
        return false;
    }


}
[System.Serializable]
public class Dialog
{
    [SerializeField] public string dialogText;
}
[System.Serializable]
public class BranchID
{
    public List<Choice> choiceStack;
}

[System.Serializable]
public class Choice
{
    [SerializeField] CharacterName subjectCharacter;
    [SerializeField] public Actions performedAction;
    [SerializeField] CharacterName objectCharacter;

    public Choice(CharacterName subjectCharacter, Actions performedAction, CharacterName objectCharacter)
    {
        this.subjectCharacter = subjectCharacter;
        this.performedAction = performedAction;
        this.objectCharacter = objectCharacter;
    }
    public string CheckAction()
    {
        return (subjectCharacter + " Performed " + performedAction + " on " + objectCharacter);
    }

    public enum Actions
    {
        Attacked,
        Killed,
        Healed,
        Observed
    }
}