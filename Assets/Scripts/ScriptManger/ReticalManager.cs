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

    [Header("Retical Shape")]
    public ActionInputParams actionInputParams;
    public Vector3Int fromPoint = Vector3Int.zero;

    [Header("Retical References")]
    [SerializeField] Vector3 reticalPos;
    [SerializeField] Vector3 lastMovePoint;
    [Header("Grid References")]
    public Tilemap Grid;

    [Header("Retical Tilemap Reffrences")]
    [SerializeField] TileBase reticalTile;
    [SerializeField] Tilemap validReticalTilesTilemap;
    void reDrawReticalTiles(List<Vector3Int> validReticalTiles)
    {
        reDrawTiles(validReticalTiles, validReticalTilesTilemap, reticalTile);
    }
    [Header("Valid Tilemap References")]
    [SerializeField] private Tilemap validTilesTileMap;
    public void reDrawValidTiles(List<Vector3Int> validTilesList, List<Vector3Int> possibleValidTilesList)
    {
        reDrawTiles(validTilesList, validTilesTileMap, reticalTile);
        reDrawTiles(possibleValidTilesList, GhostTiles, reticalTile);
    }
    [SerializeField] private Tilemap GhostTiles;
    public void setVariables()
    {
        universalCalculator = this.GetComponent<UniversalCalculator>();
        turnManager = GetComponent<TurnManager>();
    }
    void Start()
    {
        defaultShape = new ActionInputParams();
        ValidPosToShapeData = new Dictionary<Vector3Int, List<Vector3Int>>();
        lastMovePoint = getMovePoint();
        ResetReticalInputParams();
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
            reDrawReticalTiles(selectShape(currentMovePoint));
        }
    }
    //Retical Stuff
    ActionInputParams defaultShape;
    public void ResetReticalInputParams()
    {
        actionInputParams = defaultShape;
        ValidPosToShapeData.Clear();
        ClearAllTiles(validReticalTilesTilemap);
    }
    public Dictionary<Vector3Int, List<Vector3Int>> ValidPosToShapeData;
    public void UpdateReticalInputParams(ActionInputParams inputParams, List<Vector3Int> ValidTiles)
    {
        actionInputParams = inputParams;
        ValidPosToShapeData.Clear();
        for (int i = 0; i < ValidTiles.Count; i++)
        {
            ValidPosToShapeData[ValidTiles[i]] = generateShape(ValidTiles[i]);//ValidPosToShapeData.Add(ValidTiles[i], generateShape(ValidTiles[i]));
        }
    }
    List<Vector3Int> selectShape(Vector3Int currentMovePoint)
    {
        if (ValidPosToShapeData.ContainsKey(currentMovePoint))
            return ValidPosToShapeData[currentMovePoint];
        return (new List<Vector3Int>()/* {currentMovePoint} */);
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
    public List<Vector3Int> generateShape(Vector3Int atPoint)
    {
        var retiacalTiles = new List<Vector3Int>();
        if (actionInputParams.areaOfEffectType == AoeType.SSingle)
        {
            retiacalTiles.Add(atPoint);
        }
        else if (actionInputParams.areaOfEffectType == AoeType.SSweep)
        {
            retiacalTiles.AddRange(universalCalculator.generateComplexArc(fromPoint, atPoint, actionInputParams.getMagnititudeOfAction()));
            retiacalTiles.Remove(fromPoint);
        }
        else if (actionInputParams.areaOfEffectType == AoeType.S3x3)
        {
            retiacalTiles.AddRange(
                universalCalculator.generateRangeFrom2Vectors(
                atPoint + Vector3Int.up + Vector3Int.left, atPoint + Vector3Int.right + Vector3Int.down));
        }
        return retiacalTiles;
    }
    //Tile Stuff
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
    public void setReticalToFromPoint()
    {
        List<Vector3Int> thisPos = new List<Vector3Int>() { };
        thisPos.Add(fromPoint);
        reDrawReticalTiles(thisPos);
    }
    public void reDrawTiles(List<Vector3Int> theseTiles, Tilemap onTileMap, TileBase thisTile)//Needs review
    {
        ClearAllTiles(onTileMap);
        if (theseTiles != null)
            SetTiles(theseTiles, onTileMap, thisTile);
    }
}
public enum AoeType
{
    SSingle,
    S3x3,
    SSweep,
    SArrow
}