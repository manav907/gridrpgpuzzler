using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class MapManager : MonoBehaviour
{
    [SerializeField] List<Tilemap> OrderOfTileMaps;
    [SerializeField] List<TileData> listOfTileDataScriptableObjects;
    [SerializeField] Dictionary<TileBase, TileData> dataFromTiles;
    UniversalCalculator universalCalculator;
    public void setVariables()
    {
        universalCalculator = this.gameObject.GetComponent<UniversalCalculator>();
        setTilesDir();
        setCellData();
    }
    void setTilesDir()
    {
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (var ScriptableObjects in listOfTileDataScriptableObjects)
            foreach (var tileFound in ScriptableObjects.tiles)
            {
                dataFromTiles.Add(tileFound, ScriptableObjects);
            }
    }
    public bool checkAtPosIfCharacterCanWalk(Vector3Int tilePos, CharacterControllerScript characterDataHolder)
    {
        //foreach (GroundFloorType groundFloorType in cellDataDir[tilePos].tileDatas.Select(tileData => tileData.groundFloorType).ToList())
        //This get data From SO
        foreach (GroundFloorType groundFloorType in cellDataDir[tilePos].groundFloorTypeWalkRequireMents)
            //This Gets Cached Data
            //If This Loop Completes without returning False then that means that all tiles at this tilePos are Walkable
            if (!characterDataHolder.canWalkOn.Contains(groundFloorType))
                return false;
        return true;
    }
    public bool checkAtPosIfAnotherCharacterIsOccupyingSpace(Vector3Int pos)
    {
        if (cellDataDir.ContainsKey(pos))
        {
            if (cellDataDir[pos].characterAtCell != null)
                return true;
        }
        else
        {
            Debug.Log("Cell Data Dir at " + pos + " was not in Dictionary");
        }
        return false;
    }
    void setCellData()
    {
        cellDataDir = new Dictionary<Vector3Int, CellData>();
        foreach (Tilemap tilemap in OrderOfTileMaps)
        {
            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile(pos);
                if (tile != null)//Null tile Check to insure performace and stuff
                {
                    //Null Key Check to make sure CellData is instanciated only once for multiple tiles at the same Cell
                    if (!cellDataDir.ContainsKey(pos))
                    {
                        cellDataDir.Add(pos, new CellData(this.gameObject, pos));
                    }
                }
            }
        }
    }

    public Dictionary<Vector3Int, CellData> cellDataDir;
    public class CellData
    {
        //Declaring Variables for use in Script
        public List<GroundFloorType> groundFloorTypeWalkRequireMents;
        public List<TileBase> tilesOnCell;
        public List<TileData> tileDatas;
        public Vector3Int cellPos;
        //Declaring Scripts From Game Controller
        UniversalCalculator universalCalculator;
        MapManager mapManager;
        //Declaring Objects From Game Controller
        List<Tilemap> OrderOfTileMaps;
        Dictionary<TileBase, TileData> dataFromTiles;
        //Constructer For Initilizing CellData
        public CellData(GameObject gameController, Vector3Int pos)
        {
            //Initilizing Variables
            groundFloorTypeWalkRequireMents = new List<GroundFloorType>();
            tilesOnCell = new List<TileBase>();
            tileDatas = new List<TileData>();
            //Setting pos this is Very Important
            this.cellPos = pos;
            //Getting Scripts From Game Controller
            mapManager = gameController.GetComponent<MapManager>();
            universalCalculator = gameController.GetComponent<UniversalCalculator>();

            //Getting Objects From Game Controller
            OrderOfTileMaps = mapManager.OrderOfTileMaps;
            dataFromTiles = mapManager.dataFromTiles;
            //Populating CellData Here
            refreshCellData();
        }
        void refreshCellData()
        {
            //Creating New Variables to Comapare
            List<GroundFloorType> NEWgroundFloorTypeWalkRequireMents = new List<GroundFloorType>();
            List<TileBase> NEWtilesOnCell = new List<TileBase>();
            List<TileData> NEWtileDatas = new List<TileData>();
            foreach (Tilemap tilemap in OrderOfTileMaps)
            {
                TileBase tile = tilemap.GetTile(cellPos);
                //Debug.Log("Checking Tilemap " + tilemap + " and the tile was " + tile + " :: on th position of" + cellPos);
                if (tile != null)
                {
                    GroundFloorType walkRequirements = dataFromTiles[tile].groundFloorType;
                    TileBase tilesOnCell = tile;
                    TileData tileData = dataFromTiles[tile];

                    NEWgroundFloorTypeWalkRequireMents.Add(walkRequirements);
                    NEWtilesOnCell.Add(tilesOnCell);
                    NEWtileDatas.Add(tileData);
                }
                else
                {
                    //Debug.Log("Tile Was Null Somehow! On TileMap " + tilemap);
                }
            }
            groundFloorTypeWalkRequireMents = universalCalculator.CompareAndReplace(groundFloorTypeWalkRequireMents, NEWgroundFloorTypeWalkRequireMents, false);
            tilesOnCell = universalCalculator.CompareAndReplace(tilesOnCell, NEWtilesOnCell, false);
            tileDatas = universalCalculator.CompareAndReplace(tileDatas, NEWtileDatas, false);
        }
        public void ReadInfo()
        {

            refreshCellData();
            //universalCalculator.DebugEachItemInList(groundFloorTypeWalkRequireMents);
            //universalCalculator.DebugEachItemInList(tilesOnCell);
            //universalCalculator.DebugEachItemInList(tileDatas);
        }
        public GameObject characterAtCell;
        public bool isCellHoldingCharacer()
        {
            if (characterAtCell == null)
                return false;
            return true;
        }
    }
}
//Defining Global NameSapce
public enum GroundFloorType
{
    Normal,
    Water,
    Fire
};