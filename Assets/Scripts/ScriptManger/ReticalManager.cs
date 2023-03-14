using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ReticalManager : MonoBehaviour
{
    [SerializeField] Vector3 reticalPos;
    void FixedUpdate()
    {
        setRetical();
    }
    [SerializeField] GameObject characterRetical;
    void setRetical()
    {
        reticalPos = getMovePoint();
        characterRetical.transform.position = reticalPos;
    }
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
    [SerializeField] private Tilemap validTilesTileMap;
    [SerializeField] private TileBase reticalTilePrefab;
    public void reDrawValidTiles(List<Vector3Int> validTilesList)
    {
        if (validTilesList != null)
        {
            SetTiles(validTilesList, validTilesTileMap, reticalTilePrefab);
            //addtiles
        }
        else
        {
            ClearAllTiles(validTilesTileMap);
            //deletetiles
        }
    }
    [SerializeField] private Tilemap shadowTilemap;
    [SerializeField] private TileBase shadowTilePrefab;
    //[SerializeField] 
    Vector3Int topleftoffset;
    //[SerializeField] 
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
        List<Vector3Int> shadowRange = tileCalculator.generateRangeFrom2Vectors(topleft, downright);
        //clearing tiles
        ClearAllTiles(shadowTilemap);
        //creating tiles
        SetTiles(shadowRange, shadowTilemap, shadowTilePrefab);
        //clearing Tiles for vision
        var PositionToGameObjectCopy = this.gameObject.GetComponent<MapManager>().PositionToGameObject;
        foreach (GameObject thisChar in PositionToGameObjectCopy.Values)
        {
            characterDataHolder thisCDH = thisChar.GetComponent<characterDataHolder>();
            setVision(thisCDH.getCharV3Int(), thisCDH.rangeOfVision);
        }
        return shadowRange;

    }
    void setVision(Vector3Int thisPoint, int rangeOfAction)
    {
        SetTiles(tileCalculator.generateRangeFromPoint(thisPoint, rangeOfAction), shadowTilemap, null);
    }
    void SetTiles(List<Vector3Int> range, Tilemap thistilemap, TileBase thistile)
    {
        foreach (Vector3Int pos in range)
        {
            thistilemap.SetTile(pos, thistile);
        }
    }
    void ClearAllTiles(Tilemap thistilemap)
    {
        thistilemap.ClearAllTiles();
    }
    TileCalculator tileCalculator;
    public void setReticalMangerVariables()
    {
        tileCalculator = this.GetComponent<TileCalculator>();
    }

}
