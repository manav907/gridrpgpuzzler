using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New TileData", menuName = "Tile Data")]
public class TileData : ScriptableObject
{
    public TileBase[] tiles;
    public GroundFloorType groundFloorType;
}