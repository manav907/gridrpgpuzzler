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
    public Dictionary<int, CharacterData> posToCharacterData;
    public int atPos;
    public CharacterData thisCharData;
    public void addToDictionary()//Called from Custom Inspector Button
    {
        if (posToCharacterData == null)
        {
            posToCharacterData = new Dictionary<int, CharacterData>();
            Debug.Log("Never Trigger");
        }
        if (posToCharacterData.ContainsKey(atPos) || thisCharData == null)
        {
            Debug.Log("Cannot Add Key as \n" + atPos + thisCharData);
            return;
        }
        posToCharacterData.Add(atPos, thisCharData);
    }
    public void RemoveKeyFromDictionary()
    {
        posToCharacterData.Remove(atPos);
    }
    JsonSerializerSettings settings = new JsonSerializerSettings
    {
        Converters = new JsonConverter[] {
        new Vector3IntConverter(),
        new CharacterDataConverter()
    }
    };
    public void SaveData()
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Converters = new JsonConverter[] {
        new Vector3IntConverter(),
    }
        };



        string json = JsonConvert.SerializeObject(posToCharacterData, settings);
        File.WriteAllText(GlobalCal.persistantDataPath + "/levelData.json", json);
    }

    public void LoadData()
    {
        if (File.Exists(GlobalCal.persistantDataPath + "/levelData.json"))
        {
            string json = File.ReadAllText(GlobalCal.persistantDataPath + "/levelData.json");
            var data = JsonConvert.DeserializeObject<Dictionary<int, CharacterData>>(json, settings);
            posToCharacterData = data;
            /*
            foreach (var kvp in data)
            {
                // Instantiate a new CharacterData instance using the constructor that takes a string parameter
                var characterData = new CharacterData(kvp.Value);
                // Add the new instance to the posToCharacterData dictionary
                posToCharacterData.Remove(kvp.Key);
                posToCharacterData.Add(kvp.Key, characterData);
            }
            */

        }
        else
        {
            Debug.Log("File not found!");
        }
    }
}

public class Vector3IntConverter : JsonConverter<Vector3Int>
{
    public override void WriteJson(JsonWriter writer, Vector3Int value, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        writer.WriteValue(value.x);
        writer.WriteValue(value.y);
        writer.WriteValue(value.z);
        writer.WriteEndArray();
    }


    public override Vector3Int ReadJson(JsonReader reader, Type objectType, Vector3Int existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        reader.Read();
        int x = (int)(long)reader.Value;
        reader.Read();
        int y = (int)(long)reader.Value;
        reader.Read();
        int z = (int)(long)reader.Value;
        reader.Read();
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
        serializer.Serialize(writer, value);
    }
}