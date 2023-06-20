using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue>
{

    [SerializeField] public List<KeyPair<TKey, TValue>> KeyValuePairs;
    public SerializableDictionary()
    {
        KeyValuePairs = new List<KeyPair<TKey, TValue>>();
    }
    public void Add(TKey key, TValue value)
    {
        var newKeyPair = new KeyPair<TKey, TValue>(key, value);
        KeyValuePairs.Add(newKeyPair);
    }
    Dictionary<TKey, TValue> DictCache;

    public Dictionary<TKey, TValue> returnDict()
    {
        var dict = new Dictionary<TKey, TValue>();
        foreach (var pair in KeyValuePairs)
        {
            if (!dict.ContainsKey(pair.key))
                dict.Add(pair.key, pair.value);
            else
            {
                Debug.LogError(pair.key + " was Omited Might be Intended");
            }
        }
        DictCache = dict;
        return dict;
    }
    public void AddDict(Dictionary<TKey, TValue> original)
    {
        foreach (var pair in original)
        {
            KeyPair<TKey, TValue> keyPair = new KeyPair<TKey, TValue>(pair.Key, pair.Value);
            this.KeyValuePairs.Add(keyPair);
        }
    }
    public void CopyDict(Dictionary<TKey, TValue> original)
    {
        KeyValuePairs = new List<KeyPair<TKey, TValue>>();
        foreach (var pair in original)
        {
            KeyPair<TKey, TValue> keyPair = new KeyPair<TKey, TValue>(pair.Key, pair.Value);
            this.KeyValuePairs.Add(keyPair);
        }
    }
    public List<KeyPair<TKey, TValue>> returnKeyPairList()
    {
        var newList = new List<KeyPair<TKey, TValue>>();
        foreach (var pair in KeyValuePairs)
        {
            KeyPair<TKey, TValue> newPair = new KeyPair<TKey, TValue>(pair.key, pair.value);
            newList.Add(newPair);
        }
        return newList;
    }
    public List<TKey> Keys()
    {
        return returnDict().Keys.ToList();
    }
    public List<TValue> Values()
    {
        return returnDict().Values.ToList();
    }
}
[System.Serializable]
public class KeyPair<TKey, TValue>
{
    [SerializeField] public TKey key;
    [SerializeField] public TValue value;
    public KeyPair(TKey key, TValue value)
    {
        this.key = key;
        this.value = value;
    }
}