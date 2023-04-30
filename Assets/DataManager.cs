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
            setDictionarty();
        return CharNameToData[characterName];

    }
    void setDictionarty()
    {
        CharNameToData = new Dictionary<CharacterName, CharacterAnimationData>();
        foreach (var CAD in listofCAD)
        {
            CharNameToData.Add(CAD.nameEnum, CAD);
        }
    }
}
