using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        getCellData();
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
        if (PostoTileDataList.ContainsKey(tilePos))//Remove Later This is For Null Checks
            foreach (TileData tileData in PostoTileDataList[tilePos])
            {
                if (characterDataHolder.canWalkOn.Contains(tileData.floorType))
                    return true;
            }
        return false;
    }
    Dictionary<Vector3Int, List<TileData>> PostoTileDataList;
    void getCellData()
    {
        PostoTileDataList = new Dictionary<Vector3Int, List<TileData>>();
        foreach (Tilemap tilemap in OrderOfTileMaps)
        {
            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile(pos);
                if (tile != null)
                {
                    addtoDict(pos, dataFromTiles[tile]);
                }
            }
        }
        //Debug.Log(PostoTileDataList.Count);
        void addtoDict(Vector3Int pos, TileData tileData)
        {
            if (!PostoTileDataList.ContainsKey(pos))
            {
                PostoTileDataList.Add(pos, new List<TileData>());
            }
            PostoTileDataList[pos].Add(tileData);
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
