using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ReticalManager : MonoBehaviour
{
    UniversalCalculator universalCalculator;
    TurnManager turnManager;
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
    [SerializeField] ReticalShapes reticalShapes;
    [SerializeField] float rangeOfAction;
    List<Vector3Int> generateShape(Vector3Int atPoint)
    {
        var retiacalTiles = new List<Vector3Int>();
        if (reticalShapes == ReticalShapes.SSingle)
            retiacalTiles.Add(atPoint);
        else if (reticalShapes == ReticalShapes.SSweep)
        {
            Vector3Int fromPoint = universalCalculator.convertToVector3Int(turnManager.thisCharacter.transform.position);
            //retiacalTiles.AddRange(universalCalculator.getSmallAxeArc(fromPoint, atPoint));
            retiacalTiles.AddRange(universalCalculator.generateComplexArc(fromPoint, atPoint, rangeOfAction));
            retiacalTiles.Remove(fromPoint);
        }
        else if (reticalShapes == ReticalShapes.S3x3)
        {
            retiacalTiles.AddRange(
                universalCalculator.generateRangeFrom2Vectors(
                atPoint + Vector3Int.up + Vector3Int.left, atPoint + Vector3Int.right + Vector3Int.down));
        }
        return retiacalTiles;
    }
    [Header("Grid References")]
    public Tilemap Grid;
    Vector3 worldPos;
    Vector3Int tilePos;
    public Vector3Int getMovePoint()
    {
        worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        tilePos = Grid.WorldToCell(worldPos);
        return tilePos;
    }
    [Header("Retical References")]
    [SerializeField] Vector3 reticalPos;
    [SerializeField] Vector3 lastMovePoint;
    //[SerializeField] GameObject characterRetical;
    [SerializeField] TileBase reticalTilePrefab;
    [SerializeField] Tilemap validReticalTilesTilemap;
    void reDrawReticalTiles(List<Vector3Int> validReticalTiles)
    {
        reDrawTiles(validReticalTiles, validReticalTilesTilemap, reticalTilePrefab);
    }
    [Header("Valid Tilemap References")]
    [SerializeField] private Tilemap validTilesTileMap;
    public void reDrawValidTiles(List<Vector3Int> validTilesList)
    {
        reDrawTiles(validTilesList, validTilesTileMap, reticalTilePrefab);
    }
    [Header("Shadow References")]
    [SerializeField] Tilemap shadowTilemap;
    [SerializeField] TileBase shadowTilePrefab;
    Vector3Int topleftoffset;
    Vector3Int downrightoffset;
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
            if (thisCDH.isPlayerCharacter == true)
                SetVision(thisCDH.getCharV3Int(), thisCDH.rangeOfVision);
        }
        return shadowRange;
    }
    //Methods
    void SetVision(Vector3Int thisPoint, int rangeOfAction)
    {
        SetTiles(universalCalculator.generateRangeFromPoint(thisPoint, rangeOfAction), shadowTilemap, null);
    }
    void reDrawTiles(List<Vector3Int> theseTiles, Tilemap onTileMap, TileBase thisTile)
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
enum ReticalShapes
{
    SSingle,
    S3x3,
    SSweep,
    SArrow

}