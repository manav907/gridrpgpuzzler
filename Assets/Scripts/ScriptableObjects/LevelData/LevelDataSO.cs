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
    public Vector3Int CheckAtPos;
    public CharacterData importThisCharData;
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
    public void RemoveKeyFromDictionary()
    {
        posToCharacterData.Remove(CheckAtPos);
    }
    JsonSerializerSettings settings = new JsonSerializerSettings
    {
        Converters = new JsonConverter[]
        {
            new DictionaryConverter(),
            new Vector3IntConverter(),
            new CharacterDataConverter()
        }
    };
    public void SaveData()
    {
        string json = JsonConvert.SerializeObject(posToCharacterData, settings);
        File.WriteAllText(GlobalCal.persistantDataPath + "/levelData.json", json);
    }

    public void LoadData()
    {

        if (File.Exists(GlobalCal.persistantDataPath + "/levelData.json"))
        {
            string json = File.ReadAllText(GlobalCal.persistantDataPath + "/levelData.json");
            var data = JsonConvert.DeserializeObject<Dictionary<Vector3Int, CharacterData>>(json, settings);
            posToCharacterData = data;
        }
        else
        {
            Debug.Log("File not found!");
        }
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
    [SerializeField] string tryThis;
    public void tryDiagonose()
    {
    }
}
public class DictionaryConverter : JsonConverter<Dictionary<Vector3Int, CharacterData>>
{
    JsonSerializerSettings settings = new JsonSerializerSettings
    {
        Converters = new JsonConverter[]
        {
            //new DictionaryConverter(),
            //new Vector3IntConverter(),
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
[JsonConverter(typeof(Vector3IntConverter))]
public class Vector3IntConverter : JsonConverter<Vector3Int>
{
    public override void WriteJson(JsonWriter writer, Vector3Int value, JsonSerializer serializer)
    {
        Debug.Log("Here wu");
        writer.WriteStartArray();
        writer.WriteValue((int)55);
        writer.WriteValue((int)value.y);
        writer.WriteValue((int)value.z);
        writer.WriteEndArray();
    }
    public override bool CanRead { get { Debug.Log("CanRead"); return false; } }
    public override bool CanWrite { get { Debug.Log("CanWrite"); return false; } }

    public override Vector3Int ReadJson(JsonReader reader, Type objectType, Vector3Int existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        Debug.Log("Here");
        reader.Read();
        string[] values = reader.Value.ToString().Trim('(', ')').Split(',');
        int x = int.Parse(values[0].Trim());
        int y = int.Parse(values[1].Trim());
        int z = int.Parse(values[2].Trim());
        return new Vector3Int(x, y, z);
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

    public override bool CanRead { get { Debug.Log("CanRead"); return true; } }
    public override bool CanWrite { get { Debug.Log("CanWrite"); return false; } }
}