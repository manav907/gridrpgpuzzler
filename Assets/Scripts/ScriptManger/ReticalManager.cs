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
    public void reDrawShadows()
    {
        var topleft = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        var topright = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
        Debug.Log(topleft);
        Debug.Log(topright);
    }

}
