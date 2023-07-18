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
    [SerializeField] Tilemap Ground_Floor_Over;
    [SerializeField] Tilemap Ground_Floor;
    [SerializeField] Tilemap Character_Placement;
    //[SerializeField] List<TileData> listOfTileDataScriptableObjects;
    [SerializeField] Dictionary<TileBase, TileData> dataFromTiles;
    Dictionary<Vector3Int, GameObject> PosToCharGO;
    UniversalCalculator universalCalculator;
    TurnManager turnManager;


    public void setVariables()
    {
        universalCalculator = this.gameObject.GetComponent<UniversalCalculator>();
        turnManager = GetComponent<TurnManager>();
        moveDictionaryManager = GetComponent<MoveDictionaryManager>();
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
            UserDataManager.currentLevel = LoadThisLevel;
            return;
        }
        LoadThisLevel = UserDataManager.currentLevel;

    }
    public void OverWriteMapDataToSO()
    {
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

    }
    public void LoadMapDataFromSO()
    {
        pushToTileMap(Ground_Floor_Over, LoadThisLevel.Ground_Floor_Over);
        pushToTileMap(Ground_Floor, LoadThisLevel.Ground_Floor);
        pushToTileMap(Character_Placement, LoadThisLevel.Character_Placeholder);
        void pushToTileMap(Tilemap tilemap, SerializableDictionary<Vector3Int, TileBase> tileMapStore, bool ClearAllTiles = true)
        {
            var dict = tileMapStore.returnDict();
            if (ClearAllTiles)//This is used when you want to combine tilemaps be super carefull when using this
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
        foreach (var ScriptableObjects in GameEvents.current.tileDatas)
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
        //This get data From SO
        foreach (GroundFloorType groundFloorType in cellDataDir[tilePos].groundFloorTypeWalkRequireMents)//This Gets Cached Data
            if (!characterDataHolder.canWalkOn.Contains(groundFloorType))
                return false;
        return true;
    }
    public void PlaceCharacterAtPos(Vector3Int newPos, GameObject character)
    {
        PosToCharGO.Add(newPos, character);
        cellDataDir[newPos].characterAtCell = character;
        character.GetComponent<CharacterControllerScript>().currentCellPosOfCharcter = newPos;
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
                        cellDataDir.Add(pos, new CellData(pos));
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
        public Vector3Int cellPos;
        //Declaring Scripts From Game Controller
        //UniversalCalculator universalCalculator;
        //MapManager mapManager;
        //Declaring Objects From Game Controller
        //List<Tilemap> OrderOfTileMaps;
        //Dictionary<TileBase, TileData> dataFromTiles;
        //Characters
        public GameObject characterAtCell;
        //Constructer For Initilizing CellData
        public CellData(Vector3Int pos)
        {
            //Initilizing Variables
            groundFloorTypeWalkRequireMents = new List<GroundFloorType>();
            //Setting pos this is Very Important
            this.cellPos = pos;
            //Populating CellData Here
            refreshCellData();
        }
        void refreshCellData()
        {
            //Creating New Variables to Comapare
            List<GroundFloorType> NEWgroundFloorTypeWalkRequireMents = new List<GroundFloorType>();
            foreach (Tilemap tilemap in GameEvents.current.mapManager.allTileMaps)
            {
                TileBase tile = tilemap.GetTile(cellPos);
                //Debug.Log("Checking Tilemap " + tilemap + " and the tile was " + tile + " :: on th position of" + cellPos);
                if (tile != null)
                {
                    GroundFloorType walkRequirements = GroundFloorType.NotSet;

                    if (GameEvents.current.mapManager.dataFromTiles.ContainsKey(tile))
                    {
                        walkRequirements = GameEvents.current.mapManager.dataFromTiles[tile].groundFloorType;
                    }
                    else
                    {
                        var TileLayerConflict = GameEvents.current.TileLayerConflict;
                        Debug.LogError("Yo This Tile was not in the Dictionary Catorize it in GameEvent");
                        //KeyPair<TileBase, GroundFloorType> keyPair = new KeyPair<TileBase, GroundFloorType>(tile, GroundFloorType.NotSet);
                        GameEvents.current.TileLayerConflict.Add(tile);
                        return;
                    }
                    if (walkRequirements != GroundFloorType.NotSet)
                        NEWgroundFloorTypeWalkRequireMents.Add(walkRequirements);
                }
            }
            groundFloorTypeWalkRequireMents = GameEvents.current.universalCalculator.CompareAndReplace(groundFloorTypeWalkRequireMents, NEWgroundFloorTypeWalkRequireMents, false);

        }
        public void ReadInfo()
        {

            refreshCellData();
            //universalCalculator.DebugEachItemInList(groundFloorTypeWalkRequireMents);
            //universalCalculator.DebugEachItemInList(tilesOnCell);
            //universalCalculator.DebugEachItemInList(tileDatas);
        }

    }
    class Node
    {
        public Vector3Int nodeID;
        Dictionary<Node, float> NeighbourToTotalCost;
        float HeuristicCost;//distance to endPoint
        float GScore;//distance to startPoint
        public float FScore => GScore + HeuristicCost;
        public Node previousNode;
        public Node(Vector3Int Id, float HeuristicCost, float GScore, Node previousNode)
        {
            nodeID = Id;
            NeighbourToTotalCost = new Dictionary<Node, float>();
            this.HeuristicCost = HeuristicCost;
            this.GScore = GScore;
            this.previousNode = previousNode;
        }
        public void addNeighbours(Node neighbour)//, float HCost, float GCost)
        {
            NeighbourToTotalCost.Add(neighbour, neighbour.FScore);
        }
    }
    MoveDictionaryManager moveDictionaryManager;
    public List<Vector3Int> findOptimalPath(Vector3Int startPos, Vector3Int endPos, ActionInputParams actionInputParams)
    {
        string AStarDebug = "Starting Astar Navigation from Point " + startPos + " to End pos " + endPos + "\n";
        List<Node> openList = new List<Node>();
        List<Vector3Int> closeList = new List<Vector3Int>();
        List<Node> historyNode = new List<Node>();
        openList.Add(generateNodeData(startPos, null));
        closeList.Add(startPos);

        int maxLoops = 50;
        while (maxLoops != 0)
        {
            openList = universalCalculator.convertSortedListToNormalList(universalCalculator.sortListWithVar(openList, getFCost));
            Node currentNode = openList.First();
            openList.Remove(currentNode);
            AStarDebug += "\nStep " + maxLoops + ": Evaluating Node " + currentNode.nodeID + " its F Cost was " + getFCost(currentNode) + " Available Nodes Here: ";
            if (currentNode.nodeID == endPos)
            {
                PrintDebug("Path Found");
                return reconstructPath(currentNode);
            }
            foreach (Vector3Int neighbourPoint in moveDictionaryManager.getValidTargetList(actionInputParams, currentNode.nodeID))
            {
                if (closeList.Contains(neighbourPoint))
                    continue;
                else
                    closeList.Add(neighbourPoint);
                Node neighbourNode = generateNodeData(neighbourPoint, currentNode);

                currentNode.addNeighbours(neighbourNode);
                openList.Add(neighbourNode);
                historyNode.Add(neighbourNode);
            }

            maxLoops--;

        }
        PrintDebug("Failed as max loops were" + maxLoops);
        //return new List<Vector3Int>();
        historyNode = universalCalculator.convertSortedListToNormalList(universalCalculator.sortListWithVar(historyNode, getFCost));
        return reconstructPath(historyNode[0]);
        void PrintDebug(string prefix)
        {
            Debug.Log(prefix + "\n " + AStarDebug);
        }
        Node generateNodeData(Vector3Int pos, Node previousNode = null)
        {
            float Hcost = Vector3Int.Distance(pos, endPos);//distance to endPoint
            float Gcost = Vector3Int.Distance(pos, startPos);//distance to startPoint
            AStarDebug += " " + pos + " had H and G cost of " + Hcost + " " + Gcost;
            Node currentNode = new Node(pos, Hcost, Gcost, previousNode);
            return currentNode;
        }
        float getFCost(Node node)
        {
            return node.FScore;
        }
        List<Vector3Int> reconstructPath(Node node)
        {
            List<Vector3Int> path = new List<Vector3Int>();
            string Text = "PathData";
            while (node.previousNode != null)
            {
                path.Add(node.nodeID);
                Text += "\n" + node.nodeID;
                node = node.previousNode;
            }
            Debug.Log(Text);
            path.Reverse();
            return path;
        }
    }
}
//Defining Global NameSapce
public enum GroundFloorType
{
    NotSet = 0,
    Normal = 1,
    Water = 2,
    Fire = 3,
    StructuresNonWalkable = 4
};