using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue>
{
    [SerializeField] public List<KeyPair> KeyValuePairs;
    [System.Serializable]
    public class KeyPair
    {
        [SerializeField] public TKey key;
        [SerializeField] public TValue value;
        public KeyPair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }
    public Dictionary<TKey, TValue> returnDict()
    {
        var dict = new Dictionary<TKey, TValue>();
        foreach (var pair in KeyValuePairs)
        {
            dict.Add(pair.key, pair.value);
        }
        return dict;
    }
    public void CopyDict(Dictionary<TKey, TValue> original)
    {
        KeyValuePairs = new List<KeyPair>();
        foreach (var pair in original)
        {
            KeyPair keyPair = new KeyPair(pair.Key, pair.Value);
            this.KeyValuePairs.Add(keyPair);
        }
    }
    public List<KeyPair> returnKeyPairList()
    {
        var newList = new List<KeyPair>();
        foreach (var pair in KeyValuePairs)
        {
            KeyPair newPair = new KeyPair(pair.key, pair.value);
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
