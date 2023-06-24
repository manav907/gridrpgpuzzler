using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu(fileName = "New LevelGeberat", menuName = "LevelGeberat")]
public class LevelGenerator : ScriptableObject
{
    [SerializeField] int GridIntXCutOff = 0;
    [SerializeField] string promptData;
    [SerializeField] List<int> listOFTiles;
    [SerializeField] List<Vector3Int> posList;
    [SerializeField] TileBase normalFloor;
    [SerializeField] TileBase obstcles;
    [SerializeField] TileBase enemies;
    [SerializeField] TileBase playerCharacters;
    public SerializableDictionary<Vector3Int, TileBase> Ground_Floor_Over;
    public SerializableDictionary<Vector3Int, TileBase> Ground_Floor;
    public SerializableDictionary<Vector3Int, TileBase> Character_Placeholder;
    public void GenerateLevelFromList()
    {
        Ground_Floor_Over = new SerializableDictionary<Vector3Int, TileBase>();
        Ground_Floor = new SerializableDictionary<Vector3Int, TileBase>();
        Character_Placeholder = new SerializableDictionary<Vector3Int, TileBase>();

        GenerateIntListFromPrompt();
        posList = generateGridFromListAndCutoff(listOFTiles.Count, GridIntXCutOff);
        generateTileDataFromPosList();
        List<Vector3Int> generateGridFromListAndCutoff(int numberOfItems, int Columns)
        {
            var newList = new List<Vector3Int>();
            int rows = (int)numberOfItems / Columns + 1;
            rows--;
            int currentRow = 0;
            int currentColumn = 0;
            for (int i = 0; i < numberOfItems; i++)
            {
                Vector3Int pos = new Vector3Int(currentColumn, currentRow, 0);
                newList.Add(pos);
                currentColumn++;
                if (i != 0 && currentColumn % Columns == 0)
                {
                    currentRow++;
                    currentColumn = 0;
                }
            }
            return newList;

        }

        void generateTileDataFromPosList()
        {
            if (posList.Count == listOFTiles.Count)
                for (int i = 0; i < listOFTiles.Count; i++)
                {
                    Vector3Int pos = posList[i];
                    switch (listOFTiles[i])
                    {
                        case (1):
                            {
                                Ground_Floor_Over.Add(pos, obstcles);
                                break;
                            }
                        case (2):
                            {
                                Character_Placeholder.Add(pos, playerCharacters);
                                break;
                            }
                        case (3):
                            {
                                Character_Placeholder.Add(pos, enemies);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    Ground_Floor.Add(pos, normalFloor);
                }
        }
        void GenerateIntListFromPrompt()
        {
            listOFTiles.Clear();
            char[] letters = promptData.ToCharArray();
            foreach (var letter in letters)
            {
                if (char.IsDigit(letter))
                {
                    int tile = int.Parse(letter.ToString());
                    listOFTiles.Add(tile);
                }
            }
        }
    }

}
