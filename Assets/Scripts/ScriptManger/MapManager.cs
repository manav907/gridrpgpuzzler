using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;
using UnityEditor;

public class MapManager : MonoBehaviour
{
    [SerializeField] public LevelDataSO LoadThisLevel;
    List<Tilemap> allTileMaps;
    [SerializeField] Tilemap Obstacles;
    [SerializeField] Tilemap Ground_Floor_Over;
    [SerializeField] Tilemap Ground_Floor;
    [SerializeField] Tilemap Character_Placement;
    [SerializeField] List<TileData> listOfTileDataScriptableObjects;
    [SerializeField] Dictionary<TileBase, TileData> dataFromTiles;
    Dictionary<Vector3Int, GameObject> PosToCharGO;
    UniversalCalculator universalCalculator;
    TurnManager turnManager;
    public void setVariables()
    {
        universalCalculator = this.gameObject.GetComponent<UniversalCalculator>();
        turnManager = GetComponent<TurnManager>();
        LoadCorrectScene();

        LoadMapDataFromSO();
        Character_Placement.ClearAllTiles();
        PosToCharGO = new Dictionary<Vector3Int, GameObject>();
        setTilesDir();
        setCellDataDir();
    }
    void LoadCorrectScene()
    {
        if (UserDataManager.currentLevel == null)
        {
            //Debug.LogError("Incorrect Scene Loaded Loading last Known Scene");
            return;
        }
        LoadThisLevel = UserDataManager.currentLevel;

    }
    public void OverWriteMapDataToSO()
    {
        pullToTileMapStore(Obstacles, LoadThisLevel.Obstacles);
        pullToTileMapStore(Ground_Floor_Over, LoadThisLevel.Ground_Floor_Over);
        pullToTileMapStore(Ground_Floor, LoadThisLevel.Ground_Floor);
        pullToTileMapStore(Character_Placement, LoadThisLevel.Character_Placeholder);
        void pullToTileMapStore(Tilemap tilemap, SerializableDictionary<Vector3Int, TileBase> tileMapStore)
        {
            Dictionary<Vector3Int, TileBase> dict = new Dictionary<Vector3Int, TileBase>();
            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile(pos);
                if (tile != null)//Null tile Check to insure performace and stuff
                {
                    //Null Key Check to make sure CellData is instanciated only once for multiple tiles at the same Cell
                    dict.Add(pos, tile);
                }
            }
            tileMapStore.CopyDict(dict);
        }
        EditorUtility.SetDirty(LoadThisLevel);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }
    public void LoadMapDataFromSO()
    {
        UnityEditor.AssetDatabase.Refresh();
        pushToTileMap(Obstacles, LoadThisLevel.Obstacles);
        pushToTileMap(Ground_Floor_Over, LoadThisLevel.Ground_Floor_Over);
        pushToTileMap(Ground_Floor, LoadThisLevel.Ground_Floor);
        pushToTileMap(Character_Placement, LoadThisLevel.Character_Placeholder);
        void pushToTileMap(Tilemap tilemap, SerializableDictionary<Vector3Int, TileBase> tileMapStore)
        {
            var dict = tileMapStore.returnDict();
            tilemap.ClearAllTiles();
            foreach (var pair in dict)
            {
                Vector3Int pos = pair.Key;
                TileBase tile = pair.Value;
                if (tile != null)//Null tile Check to insure performace and stuff
                {
                    tilemap.SetTile(pos, tile);
                }
            }
        }
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
        if (!cellDataDir.ContainsKey(tilePos))
        {
            return false;
        }
        //foreach (GroundFloorType groundFloorType in cellDataDir[tilePos].tileDatas.Select(tileData => tileData.groundFloorType).ToList())
        //This get data From SO
        foreach (GroundFloorType groundFloorType in cellDataDir[tilePos].groundFloorTypeWalkRequireMents)
            //This Gets Cached Data
            //If This Loop Completes without returning False then that means that all tiles at this tilePos are Walkable
            if (!characterDataHolder.canWalkOn.Contains(groundFloorType))
                return false;
        return true;
    }
    public void PlaceCharacterAtPos(Vector3Int newPos, GameObject character)
    {
        PosToCharGO.Add(newPos, character);
        cellDataDir[newPos].characterAtCell = character;
        character.GetComponent<CharacterControllerScript>().CellPosOfCharcter = newPos;
    }
    public void RemoveCharacterFromPos(Vector3Int Pos)
    {
        if (PosToCharGO.ContainsKey(Pos))
        {
            cellDataDir[Pos].characterAtCell = null;
            PosToCharGO.Remove(Pos);
        }
        else
            Debug.LogError("Fatal Pos Erro");
    }
    public void UpdateCharacterPosistion(Vector3Int oldPos, Vector3Int newPos, GameObject character)
    {
        RemoveCharacterFromPos(oldPos);
        PlaceCharacterAtPos(newPos, character);
    }
    public void KillCharacter(Vector3Int newPos)
    {
        RemoveCharacterFromPos(newPos);
        cellDataDir[newPos].characterAtCell = null;
    }
    public bool isCellHoldingCharacer(Vector3Int pos)
    {
        if (cellDataDir.ContainsKey(pos))
        {
            if (cellDataDir[pos].characterAtCell != null)
                return true;
        }
        else
        {
            //Debug.Log("Cell Data Dir at " + pos + " was not in Dictionary");
        }
        return false;
    }
    void setCellDataDir()
    {
        allTileMaps = new List<Tilemap>();
        allTileMaps.Add(Obstacles);
        allTileMaps.Add(Ground_Floor);
        allTileMaps.Add(Ground_Floor_Over);
        cellDataDir = new Dictionary<Vector3Int, CellData>();
        foreach (Tilemap tilemap in allTileMaps)
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
        //Characters
        public GameObject characterAtCell;
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
            OrderOfTileMaps = mapManager.allTileMaps;
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

    }
}
//Defining Global NameSapce
public enum GroundFloorType
{
    Normal,
    Water,
    Fire,
    StructuresNonWalkable
};