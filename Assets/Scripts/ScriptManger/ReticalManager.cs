using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;

public class ReticalManager : MonoBehaviour
{
    UniversalCalculator universalCalculator;
    TurnManager turnManager;

    [Header("Cursor References")]
    [SerializeField] Vector3 reticalPos;
    [SerializeField] Vector3 lastMovePoint;

    [Header("Retical Tilemap Reffrences")]
    [SerializeField] TileBase reticalTile;
    [SerializeField] Tilemap Grid;
    [SerializeField] Tilemap validReticalTilesTilemap;
    [SerializeField] Tilemap validTilesTileMap;
    [SerializeField] private Tilemap GhostTiles;
    void Start()
    {
        ValidPosToShapeData = new Dictionary<Vector3Int, List<List<Vector3Int>>>();
        lastMovePoint = getMovePoint();
        ResetReticalInputParams();
    }
    public void setVariables()
    {
        universalCalculator = this.GetComponent<UniversalCalculator>();
        turnManager = GetComponent<TurnManager>();
    }
    void FixedUpdate()
    {
        setMovePoint();
    }
    void setMovePoint()
    {
        Vector3Int currentMovePoint = getMovePoint();
        if (currentMovePoint != lastMovePoint)
        {
            reticalPos = currentMovePoint;
            lastMovePoint = currentMovePoint;
            //Setting Retical Tiles
            reDrawReticalTiles(compressLists(selectShape(currentMovePoint)));
        }
    }
    List<Vector3Int> compressLists(List<List<Vector3Int>> listSet)
    {
        var output = new List<Vector3Int>();
        foreach (var list in listSet)
            foreach (var point in list)
                output.Add(point);

        return output;
    }
    public void ResetReticalInputParams()
    {
        //ValidPosToShapeData.Clear();
        ValidPosToShapeData = new Dictionary<Vector3Int, List<List<Vector3Int>>>();
        ClearAllTiles(validReticalTilesTilemap);
    }
    public Dictionary<Vector3Int, List<List<Vector3Int>>> ValidPosToShapeData;
    public void UpdateReticalInputParams(Dictionary<Vector3Int, List<List<Vector3Int>>> ValidTiles)
    {
        //ValidPosToShapeData.Clear();
        ValidPosToShapeData = ValidTiles;
    }
    List<List<Vector3Int>> selectShape(Vector3Int currentMovePoint)
    {
        if (ValidPosToShapeData.ContainsKey(currentMovePoint))
            return ValidPosToShapeData[currentMovePoint];
        return (new List<List<Vector3Int>>()/* {currentMovePoint} */);
    }
    public Vector3Int getIntPoint()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        Vector3Int GridCellPos = Grid.WorldToCell(worldPos);
        return GridCellPos;
    }

    public Vector3Int getMovePoint()
    {
        Vector3Int GridCellPos = getIntPoint();
        if (UserDataManager.Snap && !ValidPosToShapeData.Keys.ToList().Contains(GridCellPos))
        {
            var list = universalCalculator.SortListAccordingtoDistanceFromPoint(ValidPosToShapeData.Keys.ToList(), GridCellPos);
            if (list.Count > 0)
                return list[0];
        }
        return GridCellPos;

    }
    //Tile Stuff
    void reDrawReticalTiles(List<Vector3Int> validReticalTiles)
    {
        reDrawTiles(validReticalTiles, validReticalTilesTilemap, reticalTile);
    }

    public void reDrawValidTiles(List<Vector3Int> validTilesList)
    {
        reDrawTiles(validTilesList, validTilesTileMap, reticalTile);
    }
    public void reDrawInValidTiles(List<Vector3Int> possibleValidTilesList)
    {
        reDrawTiles(possibleValidTilesList, GhostTiles, reticalTile);
    }
    void SetTiles(List<Vector3Int> range, Tilemap thistilemap, TileBase thistile)//This causes performece problems espcially when using rule tiles
    {
        //foreach (Vector3Int pos in range) { thistilemap.SetTile(pos, thistile); }
        TileBase[] tileArray = new TileBase[range.Count];
        Array.Fill(tileArray, thistile);
        thistilemap.SetTiles(range.ToArray(), tileArray);
    }
    void ClearAllTiles(Tilemap thistilemap)
    {
        thistilemap.ClearAllTiles();
    }
    public void reDrawTiles(List<Vector3Int> theseTiles, Tilemap onTileMap, TileBase thisTile)//Needs review
    {
        ClearAllTiles(onTileMap);
        if (theseTiles != null)
            SetTiles(theseTiles, onTileMap, thisTile);
    }
}
public enum AoeStyle
{
    SSingle,
    S3x3,
    SSweep,
    SArrow,
    Taxi,
    Square
}