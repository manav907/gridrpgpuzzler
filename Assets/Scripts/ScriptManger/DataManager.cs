using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DataManager : MonoBehaviour
{
    public bool EditMapMode = false;
    public int alternateRange = 50;
    public bool checkValidActionTiles = false;
    public static DataManager current;
    void Awake()
    {
current=this;
    }

    [Header("Data Stuff")]
    public SerializableDictionary<TileBase, CharacterData> tiltoCad;    
    [Header("Reffences")]
    TurnManager turnManager;
    MoveDictionaryManager moveDictionaryManager;
    [HideInInspector] public MapManager mapManager;
  
    public void viewCurrentCharacter()
    {
        turnManager.setCameraPos(TurnManager.thisCharacter.transform.position);
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
    }
}
