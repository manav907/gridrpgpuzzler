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
    [SerializeField] inputType currentInputType;
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
    public void reDrawValidTiles(List<Vector3Int> validTilesList = null)
    {
        reDrawTiles(validTilesList, validTilesTileMap, reticalTile);
    }
    [Header("Shadow References")]
    [SerializeField] Tilemap shadowTilemap;
    [SerializeField] TileBase shadowTilePrefab;
    Vector3Int topleftoffset;
    Vector3Int downrightoffset;

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
        //Needs Optimization for perfoemce
        for (int i = 0; i < ValidTiles.Count; i++)
        {
            var newShape = generateShape(ValidTiles[i]);
            if (ValidPosToShapeData.ContainsKey(ValidTiles[i]))
            {
                ValidPosToShapeData.Remove(ValidTiles[i]);
            }
            ValidPosToShapeData[ValidTiles[i]] = newShape;
        }
    }
    List<Vector3Int> selectShape(Vector3Int currentMovePoint)
    {
        if (ValidPosToShapeData.ContainsKey(currentMovePoint))
            return ValidPosToShapeData[currentMovePoint];
        return (new List<Vector3Int>()/* {currentMovePoint} */);
    }

    public Vector3Int getMovePoint()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        Vector3Int GridCellPos = Grid.WorldToCell(worldPos);
        if (currentInputType == inputType.CellBased)
        {
            return GridCellPos;
        }
        else if (currentInputType == inputType.CellSnapNear)
        {
            //ValidPosToShapeData.Keys
            var list = universalCalculator.SortListAccordingtoDistanceFromPoint(ValidPosToShapeData.Keys.ToList(), GridCellPos);
            if (list.Count > 0)
                return list[0];
            return GridCellPos;

        }
        var DirectionToShapeDir = universalCalculator.PointsInDirectionFilter(fromPoint, GridCellPos, ValidPosToShapeData.Keys.ToList());
        if (DirectionToShapeDir.Count == 0)
            return GridCellPos;
        if (currentInputType == inputType.VectorFurthest)
            return DirectionToShapeDir.Last();
        if (currentInputType == inputType.VectorNearest)
            return DirectionToShapeDir.First();

        Debug.LogError("Using Default Escape This Should not Happen");
        return GridCellPos;
    }
    enum inputType
    {
        CellBased,
        CellSnapNear,
        VectorFurthest,
        VectorNearest,
    }
    public List<Vector3Int> generateShape(Vector3Int atPoint)
    {
        var retiacalTiles = new List<Vector3Int>();
        if (actionInputParams.areaOfEffectType == ReticalShapes.SSingle)
        {
            retiacalTiles.Add(atPoint);
        }
        else if (actionInputParams.areaOfEffectType == ReticalShapes.SSweep)
        {
            //retiacalTiles.AddRange(universalCalculator.getSimpleArc(fromPoint, atPoint, rangeOfAction));
            retiacalTiles.AddRange(universalCalculator.generateComplexArc(fromPoint, atPoint, actionInputParams.getMagnititudeOfAction()));
            retiacalTiles.Remove(fromPoint);
        }
        else if (actionInputParams.areaOfEffectType == ReticalShapes.S3x3)
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
    //Shadow Stuff
    public bool DrawShadows = false;
    public List<Vector3Int> reDrawShadows()
    {

        //getting edges of camera
        var topleft = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        var downright = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
        //assigning offsets
        topleftoffset = new Vector3Int(-2, 1, 0);
        downrightoffset = new Vector3Int(1, -2, 0);
        topleft = topleft + topleftoffset;
        downright = downright + downrightoffset;
        //generating ranges
        List<Vector3Int> shadowRange = universalCalculator.generateRangeFrom2Vectors(topleft, downright);
        if (!DrawShadows)
        { return null; }
        //reDrawTiles(shadowRange, shadowTilemap, shadowTilePrefab);
        SetTiles(shadowRange, shadowTilemap, shadowTilePrefab);

        var OrderOfInteractableCharacters = this.gameObject.GetComponent<TurnManager>().OrderOfInteractableCharacters;
        foreach (GameObject thisChar in OrderOfInteractableCharacters)
        {
            CharacterControllerScript thisCDH = thisChar.GetComponent<CharacterControllerScript>();
            if (thisCDH.isPlayerCharacter == true)
                SetVision(thisCDH.getCharV3Int(), thisCDH.rangeOfVision);//Need to change this otherwise animations break shadows
        }
        return shadowRange;
    }
    public void setReticalToFromPoint()
    {
        List<Vector3Int> thisPos = new List<Vector3Int>() { };
        thisPos.Add(fromPoint);
        reDrawReticalTiles(thisPos);
    }
    //Methods
    void SetVision(Vector3Int thisPoint, int rangeOfAction)
    {
        SetTiles(universalCalculator.generateRangeFromPoint(thisPoint, rangeOfAction), shadowTilemap, null);
    }
    public void reDrawTiles(List<Vector3Int> theseTiles, Tilemap onTileMap, TileBase thisTile)//Needs review
    {
        ClearAllTiles(onTileMap);
        if (theseTiles != null)
            SetTiles(theseTiles, onTileMap, thisTile);
    }
}
public enum ReticalShapes
{
    SSingle,
    S3x3,
    SSweep,
    SArrow
}