using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

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
    public void reDrawValidTiles(List<Vector3Int> validTilesList)
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
    public List<Vector3Int> generateShape(Vector3Int atPoint)
    {
        var retiacalTiles = new List<Vector3Int>();
        if (reticalShapes == ReticalShapes.SSingle)
            retiacalTiles.Add(atPoint);
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
        return Grid.WorldToCell(worldPos);
    }
    public void doOnClick()
    {
        Action thiAction = delegate { Debug.Log("This "); };
        StartCoroutine(onClick());
        IEnumerator onClick()
        {
            yield return new WaitUntil(() => CheckContinue());//this waits for MB0 or MB1      
            bool CheckContinue()
            {
                if (Input.GetMouseButtonDown(0))
                {
                    thiAction();
                    return true;
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    return true;
                }
                return false;
            }
        }
    }
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
        reDrawTiles(shadowRange, shadowTilemap, shadowTilePrefab);
        var OrderOfInteractableCharacters = this.gameObject.GetComponent<TurnManager>().OrderOfInteractableCharacters;
        foreach (GameObject thisChar in OrderOfInteractableCharacters)
        {
            CharacterControllerScript thisCDH = thisChar.GetComponent<CharacterControllerScript>();
            if (thisCDH.controlCharacter == true)
                SetVision(thisCDH.getCharV3Int(), thisCDH.rangeOfVision);
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
    public void reDrawTiles(List<Vector3Int> theseTiles, Tilemap onTileMap, TileBase thisTile)
    {
        ClearAllTiles(onTileMap);
        if (theseTiles != null)
        {
            SetTiles(theseTiles, onTileMap, thisTile);
            //addtiles
        }
        else
        {
            ClearAllTiles(onTileMap);
            //deletetiles
        }
    }
    void SetTiles(List<Vector3Int> range, Tilemap thistilemap, TileBase thistile)
    {
        foreach (Vector3Int pos in range)
            thistilemap.SetTile(pos, thistile);
    }
    void ClearAllTiles(Tilemap thistilemap)
    {
        thistilemap.ClearAllTiles();
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