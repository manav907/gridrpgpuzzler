using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSetManger : MonoBehaviour
{

    public List<DialogSetBranch> DialogTree;
    public Dictionary<string, DialogSetBranch> branchNameToSet;
    public DialogSetBranch curentBranch;
    public void setBranch(string branchName)
    {
        try
        {
            curentBranch = branchNameToSet[branchName];
        }
        catch (NullReferenceException)
        {
            branchNameToSet = new Dictionary<string, DialogSetBranch>();
            foreach (DialogSetBranch branch in DialogTree)
            {
                branchNameToSet.Add(branch.branchName, branch);
            }
            curentBranch = branchNameToSet[branchName];
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError(branchName + "This Dialog Path Does not Exits");
        }
    }

    [System.Serializable]
    public class DialogSetBranch
    {
        public string branchName;
        public List<CharacterDialogSet> DialogSets;
        private Dictionary<string, CharacterDialogSet> NameToSetDir;
        public string getCharacterDialog(string NameOfCharacter, int turnLoop)
        {
            try
            {
                return NameToSetDir[NameOfCharacter].getDialog(turnLoop);
            }
            catch (NullReferenceException)
            {
                initilizeDictionary();
                return getCharacterDialog(NameOfCharacter, turnLoop);
            }
            catch (KeyNotFoundException)
            {
                Debug.Log("No Dialog Here Bro the Dicitionary does not have the character bruh");
                return "dialog Error";
            }
        }
        public Sprite getCharacterSprite(string NameOfCharacter, int turnLoop)
        {
            try
            {
                return NameToSetDir[NameOfCharacter].GetSprite(turnLoop);
            }
            catch (NullReferenceException)
            {
                initilizeDictionary();
                return NameToSetDir[NameOfCharacter].GetSprite(turnLoop);
            }
        }
        void initilizeDictionary()
        {
            NameToSetDir = new Dictionary<string, CharacterDialogSet>();
            foreach (CharacterDialogSet characterDialogSet in DialogSets)
            {
                NameToSetDir.Add(characterDialogSet.NameOfCharacter, characterDialogSet);
            }
        }

    }
    [System.Serializable]
    public class CharacterDialogSet
    {
        public string NameOfCharacter;
        public List<string> Dialogs;
        public List<Sprite> characterSprites;
        public string getDialog(int turnLoop)
        {
            try
            {
                return Dialogs[turnLoop];
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.LogError(" Yo Character" + NameOfCharacter + " did not have a dialog for turnLoop" + turnLoop);//+ " Previous Dialog was /n " + Dialogs[turnLoop - 1]);
            }
            return null;

        }
        public Sprite GetSprite(int turnLoop)
        {
            try
            {
                return characterSprites[turnLoop];
            }
            catch (ArgumentOutOfRangeException)
            {
                return characterSprites[0];
                //Debug.LogError(" Yo Character" + NameOfCharacter + " did not have a Sprite for turnLoop" + turnLoop);//+ " Previous Dialog was /n " + Dialogs[turnLoop - 1]);
            }
        }
    }
}
