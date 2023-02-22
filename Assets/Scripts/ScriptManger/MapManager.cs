using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{

    public List<Tilemap> OrderOfTileMaps;
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

    public Dictionary<Vector3, GameObject> PositionToGameObject;
    public void AddCharactersToDictionaryAfterInstantiating(List<GameObject> allInteractableCharacters)
    {
        PositionToGameObject = new Dictionary<Vector3, GameObject>();
        foreach (GameObject character in allInteractableCharacters)
        {
            PositionToGameObject.Add(character.transform.position, character);
        }
    }

    [SerializeField] private List<Vector3> PositionToGameObjectVector3;
    [SerializeField] private List<GameObject> PositionToGameObjectGameObjects;
    public void UpdateCharacterPosition(Vector3 previousPosition, Vector3 newPosition, GameObject thisCharacter)
    {
        PositionToGameObject.Remove(previousPosition);
        PositionToGameObject.Add(newPosition, thisCharacter);
        PositionToGameObjectGameObjects.Clear();
        PositionToGameObjectVector3.Clear();
        foreach (Vector3 position in PositionToGameObject.Keys)
        {
            PositionToGameObjectGameObjects.Add(PositionToGameObject[position]);
            PositionToGameObjectVector3.Add(position);
        }
    }
}
