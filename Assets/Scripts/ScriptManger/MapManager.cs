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
        PositionToGameObject = new Dictionary<Vector3Int, GameObject>();
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
    public bool checkAtPosIfCharacterCanWalk(Vector3Int tilePos, characterDataHolder characterDataHolder)
    {
        foreach (GroundFloorType groundFloorType in cellDataDir[tilePos].tileDatas.Select(tileData => tileData.groundFloorType).ToList())
            //This get data From SO
            //foreach (GroundFloorType groundFloorType in cellDataDir[tilePos].groundFloorTypeWalkRequireMents)
            //This Gets Cached Data
            //If This Loop Completes without returning False then that means that all tiles at this tilePos are Walkable
            if (!characterDataHolder.canWalkOn.Contains(groundFloorType))
                return false;
        return true;
    }
    void setCellData()
    {
        cellDataDir = new Dictionary<Vector3Int, CellData>();
        foreach (Tilemap tilemap in OrderOfTileMaps)
        {
            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile(pos);
                if (tile != null)
                {
                    GroundFloorType walkRequirements = dataFromTiles[tile].groundFloorType;
                    TileBase tilesOnCell = tile;
                    TileData tileData = dataFromTiles[tile];
                    addCellDatatoDir(pos, walkRequirements, tilesOnCell, tileData);
                }
            }
        }
        void addCellDatatoDir(Vector3Int pos, GroundFloorType walkRequirements, TileBase tilesOnCell, TileData tileData)
        {
            if (!cellDataDir.ContainsKey(pos))
            {
                cellDataDir.Add(pos, new CellData(this.gameObject));
            }
            cellDataDir[pos].addToCellData(pos, walkRequirements, tilesOnCell, tileData);
        }
    }
    public Dictionary<Vector3Int, CellData> cellDataDir;
    public class CellData
    {
        public List<GroundFloorType> groundFloorTypeWalkRequireMents;
        public List<TileBase> tilesOnCell;
        public List<TileData> tileDatas;
        public Vector3Int cellPos;
        private GameObject gameController;
        public CellData(GameObject gameObject)
        {
            groundFloorTypeWalkRequireMents = new List<GroundFloorType>();
            tilesOnCell = new List<TileBase>();
            tileDatas = new List<TileData>();
            gameController = gameObject;
        }
        public void addToCellData(
            Vector3Int cellPos,
            GroundFloorType walkRequirements,
            TileBase tilesOnCell,
            TileData tileData)
        {
            this.groundFloorTypeWalkRequireMents.Add(walkRequirements);
            this.tilesOnCell.Add(tilesOnCell);
            this.tileDatas.Add(tileData);

        }
        List<Tilemap> OrderOfTileMaps;
        Dictionary<TileBase, TileData> dataFromTiles;
        void getTileMapData()
        {
            MapManager mapManager = gameController.GetComponent<MapManager>();
            OrderOfTileMaps = mapManager.OrderOfTileMaps;
            dataFromTiles = mapManager.dataFromTiles;
        }
        void refreshCellData()
        {
            getTileMapData();
            List<GroundFloorType> NEWgroundFloorTypeWalkRequireMents = new List<GroundFloorType>();
            List<TileBase> NEWtilesOnCell = new List<TileBase>();
            List<TileData> NEWtileDatas = new List<TileData>();
            foreach (Tilemap tilemap in OrderOfTileMaps)
            {
                TileBase tile = tilemap.GetTile(cellPos);
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
                    Debug.Log("Tile Was Null Somehow!");
                }
            }
        }
        public void ReadInfo()
        {
            foreach (GroundFloorType groundFloorType in groundFloorTypeWalkRequireMents)
            {
                Debug.Log(groundFloorType);
            }
            foreach (TileBase tileBase in tilesOnCell)
            {
                Debug.Log(tileBase);
            }
            foreach (TileData tileData in tileDatas)
            {
                Debug.Log(tileData);
            }
        }

    }

    public Dictionary<Vector3Int, GameObject> PositionToGameObject;
    public void AddCharactersToDictionaryAfterInstantiating(GameObject character)
    {
        Vector3Int thisPos = universalCalculator.convertToVector3Int(character.transform.position);
        PositionToGameObject.Add(thisPos, character);
    }
    public void UpdateCharacterPosition(Vector3Int previousPosition, Vector3Int newPosition, GameObject thisCharacter)
    {
        PositionToGameObject.Remove(previousPosition);
        PositionToGameObject.Add(newPosition, thisCharacter);
    }

    //Getter
    public GameObject GetGameObjectAtPos(Vector3Int thisPlace)
    {
        if (PositionToGameObject.ContainsKey(thisPlace))
        {
            return PositionToGameObject[thisPlace];
        }
        else
        {
            Debug.Log("Nothing Here");
            return null;
        }
    }
    public void fixErrors()
    {
        foreach (var thisPair in PositionToGameObject)
        {
            Vector3Int key = thisPair.Key;
            GameObject Value = thisPair.Value.gameObject;
            Vector3Int trueKey = universalCalculator.convertToVector3Int(Value.transform.position);
            if (key != trueKey)
            {
                Debug.Log("Problem the Key Value of " + key + " Does not match with current Caracter Positon of " + trueKey + " Attemtpting Fix");
                if (PositionToGameObject.ContainsKey(key))
                    PositionToGameObject.Remove(key);
                if (PositionToGameObject.ContainsKey(trueKey))
                    PositionToGameObject.Remove(trueKey);
                PositionToGameObject.Add(trueKey, Value);
                fixErrors();
                break;
            }
            else
            {
                //Debug.Log("No Errors");
            }
        }
    }










}
