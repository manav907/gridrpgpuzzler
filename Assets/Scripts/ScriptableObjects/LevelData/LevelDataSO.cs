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
        Converters = new JsonConverter[] {
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
}

public class Vector3IntConverter : JsonConverter<Vector3Int>
{
    public override void WriteJson(JsonWriter writer, Vector3Int value, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        writer.WriteValue((int)value.x);
        writer.WriteValue((int)value.y);
        writer.WriteValue((int)value.z);
        writer.WriteEndArray();
    }
    //public override bool CanWrite { get { return false; } }

    public override Vector3Int ReadJson(JsonReader reader, Type objectType, Vector3Int existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        int[] values = serializer.Deserialize<int[]>(reader);
        return new Vector3Int(values[0], values[1], values[2]);
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

    public override bool CanWrite { get { return false; } }
}