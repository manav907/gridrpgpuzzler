using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class RayCastReticalManager : MonoBehaviour
{
    void Update()
    {
        setRetical();
        //if (Input.GetMouseButtonDown(0))
        //checkOrder();
    }
    public GameObject characterRetical;
    void setRetical()
    {
        characterRetical.transform.position = getMovePoint();
    }

    public Vector3Int getMovePoint()
    {
        return getMovePointAtCursor();
    }
    public Tilemap Grid;
    Vector3 worldPos;
    Vector3Int tilePos;
    Vector3Int getMovePointAtCursor()
    {
        worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        tilePos = Grid.WorldToCell(worldPos);
        //Debug.Log(tilePos);
        return tilePos;
    }
    public List<Tilemap> OrderOfTileMaps;
    //[SerializeField]
    //public List<TileData> tileDatas;
    /*
    public TileData tileDatas;
    public TileBase tile;
    public Dictionary<TileBase, TileData> dataFromTiles;
    public void SetDictionary()
    {
        dataFromTiles = new Dictionary<TileBase, TileData>();
        dataFromTiles.Add(tile, tileDatas);

    }
    */


    public List<TileData> listOfTileDataScriptableObjects;
    public Dictionary<TileBase, TileData> dataFromTiles;
    public void SetDictionary()
    {
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (var ScriptableObjects in listOfTileDataScriptableObjects)
            foreach (var tileFound in ScriptableObjects.tiles)
            {
                dataFromTiles.Add(tileFound, ScriptableObjects);
            }
        /*
        for (int forClass = 0; forClass < listOfTileDataScriptableObjects.Count; forClass++)
        {
            for (int forTilesArray = 0; forTilesArray < listOfTileDataScriptableObjects[forClass].tiles.Length; forTilesArray++)
            {
                dataFromTiles.Add(listOfTileDataScriptableObjects[forClass].tiles[forTilesArray], listOfTileDataScriptableObjects[forClass]);
            }
        }
        */
    }


    public bool checkOrder(Vector3Int tilePos)
    {
        for (int i = 0; i < OrderOfTileMaps.Count; i++)
        {
            if (OrderOfTileMaps[i].GetTile(tilePos))
            {
                //Debug.Log(OrderOfTileMaps[i].gameObject.name + " at Positon " + tilePos);
                TileBase clickedTile = OrderOfTileMaps[i].GetTile(tilePos);
                bool isWalkable = dataFromTiles[clickedTile].isWalkable;
                //Debug.Log("the tile " + clickedTile + " is Walable: " + isWalkable);
                return isWalkable;
                //break;
            }
        }
        return false;
    }
}
