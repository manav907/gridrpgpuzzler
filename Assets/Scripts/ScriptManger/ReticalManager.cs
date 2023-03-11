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
    public GameObject characterRetical;
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
            foreach (Vector3Int pos in validTilesList)
            {
                validTilesTileMap.SetTile(pos, reticalTilePrefab);
            }
            //addtiles
        }
        else
        {
            validTilesTileMap.ClearAllTiles();
            //deletetiles
        }
    }
    [SerializeField] private Tilemap shadowTilemap;
    [SerializeField] private TileBase shadowTilePrefab;
    //[SerializeField] 
    Vector3Int topleftoffset;
    //[SerializeField] 
    Vector3Int downrightoffset;
    public void reDrawShadows()
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
        var shadowRange = tileCalculator.generateRange(topleft, downright);
        //clearing tiles
        shadowTilemap.ClearAllTiles();
        //creating tiles
        foreach (Vector3Int pos in shadowRange)
        {
            //            Debug.Log(pos);
            shadowTilemap.SetTile(pos, shadowTilePrefab);
        }

    }
    TileCalculator tileCalculator;
    public void setReticalMangerVariables()
    {
        tileCalculator = this.GetComponent<TileCalculator>();
    }

}
