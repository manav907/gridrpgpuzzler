using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New LevelData", menuName = "Level Data")]
public class LevelDataSO : ScriptableObject
{
    [Header("TileMapData")]
    [SerializeField] public SerializableDictionary<Vector3Int, TileBase> Obstacles;
    [SerializeField] public SerializableDictionary<Vector3Int, TileBase> Ground_Floor_Over;
    [SerializeField] public SerializableDictionary<Vector3Int, TileBase> Ground_Floor;
    [Header("CharacterData")]
    [SerializeField] public SerializableDictionary<Vector3Int, TileBase> Character_Placeholder;
    [SerializeField] public SerializableDictionary<TileBase, CharacterData> TileToChar;
    
    


    public Dictionary<Vector3Int, CharacterData> GenerateV3IntToCharacterDataDir()
    {
        var data = new Dictionary<Vector3Int, CharacterData>();
        Dictionary<Vector3Int, TileBase> Character_Placeholder = this.Character_Placeholder.returnDict();
        Dictionary<TileBase, CharacterData> TileToChar = DataManager.current.tiltoCad.returnDict();
        foreach (var dataPair in Character_Placeholder)
        {

            Vector3Int atPos = dataPair.Key;
            TileBase CharacterDataID = dataPair.Value;
            CharacterData characterData = ScriptableObject.CreateInstance<CharacterData>();
            characterData.ReplaceDataWithPreset(TileToChar[CharacterDataID]);
            //characterData.(IdToCharacterData[CharacterDataID]);
            try
            {
                data.Add(atPos, characterData);
            }
            catch (KeyNotFoundException)
            {
                Debug.LogError("Enemy Key not Present in Level Preset Dictionary");
            }
        }
        return data;
    }
}