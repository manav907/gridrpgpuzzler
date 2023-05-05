using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[CreateAssetMenu(fileName = "New LevelData", menuName = "Level Data")]
public class LevelDataSO : ScriptableObject
{
    public Dictionary<Vector3Int, CharacterData> posToCharacterData;
    [HideInInspector] public Vector3Int CheckAtPos;
    [HideInInspector] public CharacterData importThisCharData;
    public void addToDictionary()//Called from Custom Inspector Button
    {
        if (posToCharacterData == null)
        {
            posToCharacterData = new Dictionary<Vector3Int, CharacterData>();
            Debug.Log("Never Trigger");
        }
        if (posToCharacterData.ContainsKey(CheckAtPos) || importThisCharData == null)
        {
            Debug.Log("Cannot Add Key as \n" + CheckAtPos + importThisCharData);
            return;
        }
        posToCharacterData.Add(CheckAtPos, importThisCharData);
    }
    JsonSerializerSettings settings = new JsonSerializerSettings
    {
        Converters = new JsonConverter[]
        {
            new DictionaryConverterV3IntCharacterData()
        }
    };
    public void SaveData()
    {
        string json = JsonConvert.SerializeObject(posToCharacterData, settings);
        string path = "Assets/Resources/levelData.json"; // the path of the asset relative to the Resources folder
        File.WriteAllText(path, json);
        UnityEditor.AssetDatabase.Refresh();
    }

    public void LoadData()
    {

        var asset = Resources.Load<TextAsset>("/levelData.json");
        if (asset != null)
        {
            string json = asset.text;
            var data = JsonConvert.DeserializeObject<Dictionary<Vector3Int, CharacterData>>(json, settings);
            posToCharacterData = data;
        }
        else
        {
            Debug.Log("File not found!");
        }
    }
    public void ClearData()
    {
        posToCharacterData = new Dictionary<Vector3Int, CharacterData>();
    }
    public bool loadDataifNotLoaded()
    {
        foreach (var thisvar in posToCharacterData)
        {
            if (thisvar.Value == null)
            {
                LoadData();
                Debug.Log("Values Loaded");
            }
            break;
        }
        return true;
    }
    //[SerializeField] string tryThis;
    public void tryDiagonose()
    {
    }
}
public class DictionaryConverterV3IntCharacterData : JsonConverter<Dictionary<Vector3Int, CharacterData>>
{
    JsonSerializerSettings settings = new JsonSerializerSettings
    {
        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        Converters = new JsonConverter[]
        {
            new CharacterDataConverter()
        }
    };
    public override bool CanRead => true;
    public override bool CanWrite => true;

    public override Dictionary<Vector3Int, CharacterData> ReadJson(JsonReader reader, Type objectType, Dictionary<Vector3Int, CharacterData> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var dict = new Dictionary<Vector3Int, CharacterData>();
        var jObject = JObject.Load(reader);

        foreach (var kvp in jObject)
        {
            var key = JsonConvert.DeserializeObject<Vector3Int>(kvp.Key);
            var value = JsonConvert.DeserializeObject<CharacterData>(kvp.Value.ToString(), settings);
            dict.Add(key, value);
        }

        return dict;
    }

    public override void WriteJson(JsonWriter writer, Dictionary<Vector3Int, CharacterData> value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        foreach (var kvp in value)
        {
            var key = JsonConvert.SerializeObject(kvp.Key);
            var val = JObject.Parse(JsonConvert.SerializeObject(kvp.Value));
            writer.WritePropertyName(key);
            val.WriteTo(writer);
        }

        writer.WriteEndObject();
    }
}
public class CharacterDataConverter : JsonConverter<CharacterData>
{
    public override CharacterData ReadJson(JsonReader reader, Type objectType, CharacterData existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var data = ScriptableObject.CreateInstance<CharacterData>();
        var jObject = JObject.Load(reader);
        serializer.Populate(jObject.CreateReader(), data);
        return data;
    }

    public override void WriteJson(JsonWriter writer, CharacterData value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanRead { get { return true; } }
    public override bool CanWrite { get { return false; } }
}