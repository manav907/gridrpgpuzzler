using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChoiceBasedDialogManager
{
    Dictionary<string, DialogEvent> ChoiceToBranchMap;
    public DialogEvent currentBranch;
    BranchID cuurentBranchID;
    public bool ChangeBranch(Choice newActiontaken)
    {
        if (currentBranch.isChoiceContained(newActiontaken))
        {
            
            cuurentBranchID.choiceStack.Add(newActiontaken);
            if (ChoiceToBranchMap.ContainsKey(cuurentBranchID.ID))//Need To Write New Check For This As Well
            {
                currentBranch = ChoiceToBranchMap[cuurentBranchID.ID];
                Debug.Log("Path Exists");
                return true;
            }
            else
            {
                Debug.LogError("Path Does not Exist");
                return false;
            }
        }
        else
        {
            return false;
        }


    }
    public ChoiceBasedDialogManager(SerializableDictionary<BranchID, DialogEvent> ChoiceToBranchMapSD)
    {
        var tempDict = ChoiceToBranchMapSD.returnDict();
        ChoiceToBranchMap = new Dictionary<string, DialogEvent>();
        foreach (var keyPair in tempDict)
        {
            ChoiceToBranchMap.Add(keyPair.Key.ID, keyPair.Value);
        }
        currentBranch = ChoiceToBranchMapSD.Values()[0];
        cuurentBranchID = new BranchID();
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
    public string ID
    {
        get
        {
            string IDstring = "";
            foreach (Choice choice in choiceStack)
            {
                IDstring += choice.CheckAction();
            }
            return IDstring;
        }
    }
    public List<Choice> choiceStack;
    public BranchID()
    {
        choiceStack = new List<Choice>();
    }
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