using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "New LevelData", menuName = "Level Data")]
public class LevelDataSO : ScriptableObject
{
    public LevelData levelData;
    public int atPos;
    public CharacterData thisCharData;
    public void addToDictionary()//Called from Custom Inspector Button
    {

        if (levelData == null)
            levelData = new LevelData();
        if (levelData.posToCharacterData == null)
            levelData.posToCharacterData = new Dictionary<int, CharacterData>();
        if (levelData.posToCharacterData.ContainsKey(atPos) || thisCharData == null)
        {
            Debug.Log("Cannot Add Key as \n" + atPos + thisCharData);
            return;
        }
        levelData.posToCharacterData.Add(atPos, thisCharData);
    }
    public void SaveData()
    {
        var settings = new JsonSerializerSettings { Converters = new[] { new Vector3IntConverter() } };
        string json = JsonConvert.SerializeObject(levelData, settings);
        File.WriteAllText(GlobalCal.persistantDataPath + "/levelData.json", json);
    }

    public void LoadData()
    {
        if (File.Exists(GlobalCal.persistantDataPath + "/levelData.json"))
        {
            string json = File.ReadAllText(GlobalCal.persistantDataPath + "/levelData.json");
            var settings = new JsonSerializerSettings { Converters = new[] { new Vector3IntConverter() } };
            levelData = JsonConvert.DeserializeObject<LevelData>(json, settings);
        }
        else
        {
            Debug.Log("File not found!");
        }
    }
}
public class LevelData
{
    public Dictionary<int, CharacterData> posToCharacterData;
    public LevelData()
    {

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
        Debug.Log("Starting");
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