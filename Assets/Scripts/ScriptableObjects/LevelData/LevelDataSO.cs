using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[CreateAssetMenu(fileName = "New LevelData", menuName = "Level Data")]
public class LevelDataSO : ScriptableObject
{
    public Dictionary<Vector3Int, CharacterData> posToCharacterData;
    [Header("Level Builder Stuff")]
    public Dictionary<string, CharacterData> IdToCharacterData;
    public Dictionary<Vector3Int, string> V3IntToID;
    public Dictionary<Vector3Int, CharacterData> CharacterDataOverrides;
    [Header("DataStore")]
    public Dictionary<string, string> objectNameToJsonString;
    [SerializeField][HideInInspector] public string DataStore;
    [Header("Serilizable LevelData")]
    [SerializeField] SerializableDictionary<string, CharacterData> IDToChar;
    [SerializeField] SerializableDictionary<Vector3Int, string> PosToID;
    [SerializeField] SerializableDictionary<Vector3Int, StringEnum> V3String;
    [Header("Dfactor ")]
    [SerializeField] StringEnumDictionary<Vector3Int, CharacterData> V3IntChar;

    public void SaveDataInDictionary()
    {
        objectNameToJsonString = new Dictionary<string, string>();
        objectNameToJsonString.Add(IdToCharacterData.ToString(), JsonConvert.SerializeObject(IdToCharacterData, GeneralSettings));
        objectNameToJsonString.Add(V3IntToID.ToString(), JsonConvert.SerializeObject(V3IntToID, GeneralSettings));
        DataStore = JsonConvert.SerializeObject(objectNameToJsonString, GeneralSettings);
        Debug.Log("Saved Data in Data Strore");
    }
    public void LoadDataFromDictionary()
    {
        ClearData();
        Dictionary<string, string> objectNameToJsonString = JsonConvert.DeserializeObject<Dictionary<string, string>>(DataStore);
        IdToCharacterData = JsonConvert.DeserializeObject<Dictionary<string, CharacterData>>(objectNameToJsonString[IdToCharacterData.ToString()], GeneralSettings);
        V3IntToID = JsonConvert.DeserializeObject<Dictionary<Vector3Int, string>>(objectNameToJsonString[V3IntToID.ToString()], GeneralSettings);


        ////

        IDToChar.CopyDict(IdToCharacterData);
        PosToID.CopyDict(V3IntToID);
    }
    public Dictionary<Vector3Int, CharacterData> GenerateV3IntToCharacterDataDir(string json)
    {

        var data = new Dictionary<Vector3Int, CharacterData>();
        LoadDataFromDictionary();
        foreach (var dataPair in V3IntToID)
        {

            Vector3Int atPos = dataPair.Key;
            string CharacterDataID = dataPair.Value;
            CharacterData characterData = ScriptableObject.CreateInstance<CharacterData>();
            characterData.ReplaceDataWithPreset(IdToCharacterData[CharacterDataID]);
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
    JsonSerializerSettings GeneralSettings = new JsonSerializerSettings
    {
        Converters = new JsonConverter[]
        {
            new DictionaryConverterStringCharacterData(),
            new DictionaryConverterV3IntCharacterData(),
            new DictionaryConverterV3IntString()
        }
    };
#if UNITY_EDITOR
    public void SaveData()
    {
        string json = JsonConvert.SerializeObject(posToCharacterData, GeneralSettings);
        string path = "levelData"; // the name of the asset relative to the Resources folder
        TextAsset existingAsset = Resources.Load<TextAsset>(path);
        if (existingAsset != null)
        {
            // update existing asset
            StreamWriter writer = new StreamWriter(Application.dataPath + "/Resources/" + path + ".json", false);
            writer.Write(json);
            writer.Close();
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.Log("New File Created");
            // create new asset
            StreamWriter writer = new StreamWriter(Application.dataPath + "/Resources/" + path + ".json", false);
            writer.Write(json);
            writer.Close();
            AssetDatabase.Refresh();
        }
    }
#endif
    public void LoadData()
    {
        var asset = Resources.Load<TextAsset>("levelData");
        if (asset != null)
        {
            string json = asset.text;
            var data = JsonConvert.DeserializeObject<Dictionary<Vector3Int, CharacterData>>(json, GeneralSettings);
            posToCharacterData = data;
        }
        else
        {
            Debug.Log("File not found!");
        }


        //posToCharacterData = GenerateV3IntToCharacterDataDir(DataStore);

    }
    public void ClearData()
    {
        posToCharacterData = new Dictionary<Vector3Int, CharacterData>();
        IdToCharacterData = new Dictionary<string, CharacterData>();
        V3IntToID = new Dictionary<Vector3Int, string>();
        CharacterDataOverrides = new Dictionary<Vector3Int, CharacterData>();
    }
    //[SerializeField] string tryThis;
    public void tryDiagonose()
    {
    }
    void OnGUI()

    {
        if (GUILayout.Button("Help"))
        //if (GUI.Button(new Rect(10, 10, 50, 50), "Help"))
        {
            Debug.Log("Help");
        }
    }
}
