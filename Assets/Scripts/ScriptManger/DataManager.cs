using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public bool EditMapMode = false;
    public int alternateRange = 50;
    public bool checkValidActionTiles = false;

    [Header("Data Stuff")]
    public List<CharacterAnimationData> listofCAD;
    public Dictionary<CharacterName, CharacterAnimationData> CharNameToData;
    public CharacterAnimationData getFromSO(CharacterName characterName)
    {
        if (CharNameToData == null)
        {
            Debug.Log("Null Dictonary Set");
            setDictionarty();
        }
        if (CharNameToData.ContainsKey(characterName))
            return CharNameToData[characterName];
        Debug.Log(characterName.ToString() + " was not found in dictionary");
        return null;
    }
    [Header("Reffences")]
    TurnManager turnManager;
    MoveDictionaryManager moveDictionaryManager;
    [HideInInspector] public MapManager mapManager;
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
    public void viewCurrentCharacter()
    {
        turnManager.setCameraPos(turnManager.thisCharacter.transform.position);
    }
    public void setCameraPos(Vector3Int pos)
    {
        turnManager.setCameraPos(pos);
    }

    public void setVariables()
    {
        turnManager = GetComponent<TurnManager>();
        moveDictionaryManager = GetComponent<MoveDictionaryManager>();
        mapManager = GetComponent<MapManager>();

        setDictionarty();
    }
}
