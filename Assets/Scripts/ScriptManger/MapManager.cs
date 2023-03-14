using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{

    [SerializeField] List<Tilemap> OrderOfTileMaps;
    [SerializeField] List<TileData> listOfTileDataScriptableObjects;
    [SerializeField] Dictionary<TileBase, TileData> dataFromTiles;
    TileCalculator tileCalculator;
    public void setTileDictionary()
    {
        tileCalculator = this.gameObject.GetComponent<TileCalculator>();
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
            Vector3Int thisPos = tileCalculator.convertToVector3Int(character.transform.position);
            PositionToGameObject.Add(thisPos, character);
        }
    }

    [SerializeField] private List<Vector3Int> PositionToGameObjectVector3;
    [SerializeField] private List<GameObject> PositionToGameObjectGameObjects;
    public void UpdateCharacterPosition(Vector3Int previousPosition, Vector3Int newPosition, GameObject thisCharacter)
    {
        PositionToGameObject.Remove(previousPosition);
        PositionToGameObject.Add(newPosition, thisCharacter);
        PositionToGameObjectGameObjects.Clear();
        PositionToGameObjectVector3.Clear();
        foreach (Vector3Int position in PositionToGameObject.Keys)
        {
            PositionToGameObjectGameObjects.Add(PositionToGameObject[position]);
            PositionToGameObjectVector3.Add(position);
        }
    }

    //Getters
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









}
