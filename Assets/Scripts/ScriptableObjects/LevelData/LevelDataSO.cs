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

    [Header("Serilizable LevelData")]
    [SerializeField] SerializableDictionary<string, CharacterData> IDToChar;
    [SerializeField] SerializableDictionary<Vector3Int, string> PosToID;
    [SerializeField] public SerializableDictionary<Vector3Int, TileBase> Obstacles;
    [SerializeField] public SerializableDictionary<Vector3Int, TileBase> Ground_Floor_Over;
    [SerializeField] public SerializableDictionary<Vector3Int, TileBase> Ground_Floor;
    public Dictionary<Vector3Int, CharacterData> GenerateV3IntToCharacterDataDir()
    {

        var data = new Dictionary<Vector3Int, CharacterData>();
        Dictionary<Vector3Int, string> PosToIDDir = PosToID.returnDict();
        Dictionary<string, CharacterData> IDToCharDir = IDToChar.returnDict();
        foreach (var dataPair in PosToIDDir)
        {

            Vector3Int atPos = dataPair.Key;
            string CharacterDataID = dataPair.Value;
            CharacterData characterData = ScriptableObject.CreateInstance<CharacterData>();
            characterData.ReplaceDataWithPreset(IDToCharDir[CharacterDataID]);
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
