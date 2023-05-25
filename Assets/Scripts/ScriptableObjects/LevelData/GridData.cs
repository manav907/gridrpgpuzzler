using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New GridData", menuName = "Grid Data")]
public class GridData : ScriptableObject
{
    [SerializeField] public SerializableDictionary<Vector3Int, TileBase> Obstacles;
    [SerializeField] public SerializableDictionary<Vector3Int, TileBase> Ground_Floor_Over;
    [SerializeField] public SerializableDictionary<Vector3Int, TileBase> Ground_Floor;
}
