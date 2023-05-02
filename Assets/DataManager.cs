using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public List<CharacterAnimationData> listofCAD;
    public Dictionary<CharacterName, CharacterAnimationData> CharNameToData;
    public CharacterAnimationData getFromSO(CharacterName characterName)
    {
        if (CharNameToData == null)
            {
                Debug.Log("Null Dictonary Set");
                setDictionarty();}
        if (CharNameToData.ContainsKey(characterName))
            return CharNameToData[characterName];
        Debug.Log(characterName.ToString() + " was not found in dictionary");
        return null;


    }
    void setDictionarty()
    {
        CharNameToData = new Dictionary<CharacterName, CharacterAnimationData>();
        foreach (var CAD in listofCAD)
        {
            if (CharNameToData.ContainsKey(CAD.nameEnum))
            {
                Debug.Log("Dupplicate Entry Will Not Add");
            }
            else
                CharNameToData.Add(CAD.nameEnum, CAD);
        }
    }
}
