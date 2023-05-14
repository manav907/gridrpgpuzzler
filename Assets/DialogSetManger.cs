using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSetManger : MonoBehaviour
{

    public List<DialogSetBranch> DialogTree;
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
        public string getDialog(int turnLoop)
        {
            try
            {
                return Dialogs[turnLoop];
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.LogError(" Yo Character" + NameOfCharacter + " did not have a dialog for turnLoop" + turnLoop );//+ " Previous Dialog was /n " + Dialogs[turnLoop - 1]);
            }
            return null;

        }
    }
}
