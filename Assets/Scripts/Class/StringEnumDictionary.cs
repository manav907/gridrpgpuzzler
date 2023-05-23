using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringEnumDictionary<TKey, TValue>
{
    [SerializeField] SerializableDictionary<string, TValue> IDDir;
    string[] options;
    [SerializeField] SerializableDictionary<TKey, string> ValueDir;
}
