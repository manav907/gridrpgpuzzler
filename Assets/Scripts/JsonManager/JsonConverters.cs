using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class JsonConverters : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
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
            var key = JsonConvert.SerializeObject(kvp.Key);//This is Special Beacause this is a Object Type
            var val = JObject.Parse(JsonConvert.SerializeObject(kvp.Value));//This is Special Beacause this is a Object Type
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
public class DictionaryConverterV3IntString : JsonConverter<Dictionary<Vector3Int, string>>
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

    public override Dictionary<Vector3Int, string> ReadJson(JsonReader reader, Type objectType, Dictionary<Vector3Int, string> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var dict = new Dictionary<Vector3Int, string>();
        var jObject = JObject.Load(reader);

        foreach (var kvp in jObject)
        {
            var key = JsonConvert.DeserializeObject<Vector3Int>(kvp.Key);//This is Special Beacause this is a Object Type
            var value = kvp.Value.ToString();//This is Special Beacause this is a Value Type
            dict.Add(key, value);
        }

        return dict;
    }

    public override void WriteJson(JsonWriter writer, Dictionary<Vector3Int, string> value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        foreach (var kvp in value)
        {
            var key = JsonConvert.SerializeObject(kvp.Key);
            writer.WritePropertyName(key);//This is Special Beacause this is a Object Type
            writer.WriteValue(kvp.Value);//This is Special Beacause this is a Value Type
        }

        writer.WriteEndObject();
    }
}
public class DictionaryConverterStringCharacterData : JsonConverter<Dictionary<string, CharacterData>>
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
    public override bool CanWrite => false;

    public override Dictionary<string, CharacterData> ReadJson(JsonReader reader, Type objectType, Dictionary<string, CharacterData> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var dict = new Dictionary<string, CharacterData>();
        var jObject = JObject.Load(reader);

        foreach (var kvp in jObject)
        {
            var key = kvp.Key.ToString();//This is Special Beacause this is a Value Type
            var value = JsonConvert.DeserializeObject<CharacterData>(kvp.Value.ToString(), settings);
            dict.Add(key, value);
        }

        return dict;
    }

    public override void WriteJson(JsonWriter writer, Dictionary<string, CharacterData> value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        foreach (var kvp in value)
        {
            var key = JsonConvert.SerializeObject(kvp.Key);//This is Special Beacause this is a Object Type
            var val = JObject.Parse(JsonConvert.SerializeObject(kvp.Value));//This is Special Beacause this is a Object Type
            writer.WritePropertyName(key);
            val.WriteTo(writer);
        }

        writer.WriteEndObject();
    }
}