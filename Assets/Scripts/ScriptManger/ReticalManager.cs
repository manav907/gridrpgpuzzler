using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class ReticalManager : MonoBehaviour
{
    UniversalCalculator universalCalculator;
    TurnManager turnManager;

    [Header("Retical Shape")]
    public ReticalShapes reticalShapes = ReticalShapes.SSingle;
    public Vector3Int fromPoint = Vector3Int.zero;
    public float rangeOfAction;

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
        lastMovePoint = getMovePoint();
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
            reDrawReticalTiles(generateShape(currentMovePoint));
        }
    }
    //Retical Stuff
    public List<Vector3Int> generateShape(Vector3Int atPoint)
    {
        var retiacalTiles = new List<Vector3Int>();
        if (reticalShapes == ReticalShapes.SSingle)
        {
            //retiacalTiles.Add(atPoint);
            //Debug.Log(fromPoint + " " + TurnManager.thisCharacter.GetComponent<CharacterControllerScript>().getCharV3Int());
            retiacalTiles.AddRange(universalCalculator.generateSingleSnapPoints(fromPoint, atPoint));
        }
        else if (reticalShapes == ReticalShapes.SSweep)
        {

            //retiacalTiles.AddRange(universalCalculator.getSimpleArc(fromPoint, atPoint, rangeOfAction));
            retiacalTiles.AddRange(universalCalculator.generateComplexArc(fromPoint, atPoint, rangeOfAction));
            retiacalTiles.Remove(fromPoint);
        }
        else if (reticalShapes == ReticalShapes.S3x3)
        {
            retiacalTiles.AddRange(
                universalCalculator.generateRangeFrom2Vectors(
                atPoint + Vector3Int.up + Vector3Int.left, atPoint + Vector3Int.right + Vector3Int.down));
        }
        else if (reticalShapes == ReticalShapes.SEndTurn)
        {
            retiacalTiles.Add(fromPoint);
        }
        return retiacalTiles;
    }
    public Vector3Int getMovePoint()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        //return Grid.WorldToCell(worldPos);
        return universalCalculator.generateSingleSnapPoints(fromPoint, universalCalculator.castAsV3Int(Grid.WorldToCell(worldPos)))[0];
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
    SArrow,
    SEndTurn

}