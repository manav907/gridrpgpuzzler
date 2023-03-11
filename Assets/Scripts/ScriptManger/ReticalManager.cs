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
        moveDictionaryManager = this.GetComponent<MoveDictionaryManager>();//should not be here
        var shadowRange = generateRange(topleft, topright);
        //shadowTilemap.ClearAllTiles();
        foreach (Vector3Int pos in shadowRange)
        {
            Debug.Log(pos);
            shadowTilemap.SetTile(pos, shadowTilePrefab);
        }

    }


    List<Vector3Int> generateRange(Vector3 start, Vector3 end)
    {

        Debug.Log(end.y);

        List<Vector3Int> listOfRanges = new List<Vector3Int>();

        int startx = (int)start.x;
        int starty = (int)start.y;
        int endx = (int)end.x;
        int endy = (int)end.y;
        if (startx > endx)
        {
            int x = startx;
            startx = endx;
            endx = x;
        }
        if (starty > endy)
        {
            int x = starty;
            starty = endy;
            endy = x;
        }
        for (int x = startx; x <= endx; x++)
        {

            for (int y = starty; y <= endy; y++)// problem when y > than y
            {

                Debug.Log(y);

                Vector3Int atXY = new Vector3Int(x, y, 0);
                listOfRanges.Add(atXY);
            }
        }
        return listOfRanges;
    }





    MoveDictionaryManager moveDictionaryManager;
    public void setReticalMangerVariables()
    {
        MoveDictionaryManager moveDictionaryManager = this.GetComponent<MoveDictionaryManager>();
    }

}
