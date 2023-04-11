using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileData : ScriptableObject
{
    public TileBase[] tiles;
    //public bool isWalkable;

    public GroundFloorType floorType;
}