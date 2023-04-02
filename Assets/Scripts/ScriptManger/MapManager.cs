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
        setTilesDir();
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
    public bool getIsWalkable(Vector3Int tilePos)
    {
        for (int i = 0; i < OrderOfTileMaps.Count; i++)
        {
            if (OrderOfTileMaps[i].GetTile(tilePos))
            {
                TileBase clickedTile = OrderOfTileMaps[i].GetTile(tilePos);
                bool isWalkable = dataFromTiles[clickedTile].isWalkable;
                return isWalkable;
            }
        }
        return false;
    }

    public Dictionary<Vector3Int, GameObject> PositionToGameObject;
    public void AddCharactersToDictionaryAfterInstantiating(List<GameObject> allInteractableCharacters)
    {
        PositionToGameObject = new Dictionary<Vector3Int, GameObject>();
        foreach (GameObject character in allInteractableCharacters)
        {
            Vector3Int thisPos = universalCalculator.convertToVector3Int(character.transform.position);
            PositionToGameObject.Add(thisPos, character);
        }
        getMapData();
    }
    void getMapData()
    {
        foreach (Tilemap tilemap in OrderOfTileMaps)
        {

            Debug.Log(tilemap.gameObject.name);
            //Debug.Log(tilemap.cellBounds);
            tilemap.CompressBounds();
            //Debug.Log("CompressingBounds");
            Debug.Log(tilemap.cellBounds);
        }
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
