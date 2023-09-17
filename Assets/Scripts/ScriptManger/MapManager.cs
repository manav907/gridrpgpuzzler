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
    public void SetVariables()
    {
        universalCalculator = this.gameObject.GetComponent<UniversalCalculator>();
        moveDictionaryManager = GetComponent<MoveDictionaryManager>();
        LoadCorrectScene();

        LoadMapDataFromSO();
        Character_Placement.ClearAllTiles();
        PosToCharGO = new Dictionary<Vector3Int, GameObject>();
        SetTilesDir();
        SetCellDataDir();
    }
    void LoadCorrectScene()
    {
        Debug.ClearDeveloperConsole();
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
            Dictionary<Vector3Int, TileBase> dict = new();
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
    void SetTilesDir()
    {
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (var ScriptableObjects in GameEvents.current.tileDatas)
            foreach (var tileFound in ScriptableObjects.tiles)
            {
                dataFromTiles.Add(tileFound, ScriptableObjects);
            }
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
    public bool IsCellHoldingCharacer(Vector3Int pos)
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
    void SetCellDataDir()
    {
        allTileMaps = new List<Tilemap>
        {
            Ground_Floor,
            Ground_Floor_Over
        };
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
                        Debug.LogError("Yo This Tile was not in the Dictionary Catorize it in GameEvent");
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
        public void AddNeighbours(Node neighbour)//, float HCost, float GCost)
        {
            NeighbourToTotalCost.Add(neighbour, neighbour.FScore);
        }
    }
    public List<Vector3Int> FilterListWithWalkRequirements(List<Vector3Int> scanPoints, List<GroundFloorType> canWalkOn)
    {
        var newList = new List<Vector3Int>();
        foreach (var point in scanPoints)
        {
            if (checkAtPosIfCharacterCanWalk(point))
            {
                newList.Add(point);
            }
        }
        return newList;
        bool checkAtPosIfCharacterCanWalk(Vector3Int tilePos)
        {
            if (canWalkOn.Contains(GroundFloorType.Invalid))
                return true;
            if (!cellDataDir.ContainsKey(tilePos))
            {
                return false;
            }
            //This get data From SO
            foreach (GroundFloorType groundFloorType in cellDataDir[tilePos].groundFloorTypeWalkRequireMents)//This Gets Cached Data
                if (!canWalkOn.Contains(groundFloorType))
                    return false;
            return true;
        }
    }
    public List<Vector3Int> FilterListWithTileRequirements(List<Vector3Int> scanPoints, CharacterControllerScript castingCharacter, ValidTargets validTargets)
    {
        var newList = new List<Vector3Int>();
        foreach (var point in scanPoints)
        {
            if (CheckIfTargetis(point, validTargets, castingCharacter.Faction))
            {
                newList.Add(point);
            }
        }
        return newList;
    }
    public bool CheckIfTargetis(Vector3Int checkPos, ValidTargets validTargets, string factionOfCaster)
    {
        if (validTargets == ValidTargets.AnyValidOrInValid)
            return true;
        if (!cellDataDir.ContainsKey(checkPos))
            return false;
        if (validTargets == ValidTargets.SolidObstruction)
            return cellDataDir[checkPos].groundFloorTypeWalkRequireMents.Contains(GroundFloorType.StructuresNonWalkable);
        if (validTargets == ValidTargets.Empty)
            return !IsCellHoldingCharacer(checkPos);
        else if (IsCellHoldingCharacer(checkPos))
        {
            string faction = cellDataDir[checkPos].characterAtCell.GetComponent<CharacterControllerScript>().Faction;
            //Debug.Log("Factions " + faction + checkPos + " and " + factionOfCaster + "\n are " + validTargets + "?");
            switch (validTargets)
            {
                case ValidTargets.AnyFaction:
                    {
                        return true;
                    }
                case ValidTargets.Enemies:
                    {
                        if (factionOfCaster != faction)
                            return true;
                        return false;
                    }
                case ValidTargets.Allies:
                    {
                        if (factionOfCaster == faction)
                            return true;
                        return false;
                    }
            }
        }
        return false;
    }
    public List<Vector3Int> CheckDirection(Vector3Int fromPoint, Vector3Int Direction, ExploreParams exploreParams, string faction)
    {
        List<Vector3Int> ValidPoints = new List<Vector3Int>();
        int startPoint = 1;
        if (Direction == Vector3Int.zero)
        {
            startPoint = exploreParams.ExploreRangeMax;
        }
        for (int i = startPoint; i < exploreParams.ExploreRangeMax; i++)
        {
            Debug.Log("Exploreing range " + i);
            Vector3Int CheckingPos = fromPoint + (Direction * i);
            var area = GetArea(CheckingPos, Direction, exploreParams.AffectsArea, faction);
            if (area.Count > 0)
            {
                Debug.Log("The Point is Valid" + CheckingPos + "for realtion of " + exploreParams.AffectsArea.AffectTargets.First() + " to Faction" + faction);
                ValidPoints.Add(CheckingPos);
                if (exploreParams.perices)
                    continue;
                else
                    return ValidPoints;
            }
            if (CheckTargetMatch(CheckingPos, exploreParams.StoppedByTargets, faction))// && !targetPriority)
            {
                return ValidPoints;
            }
        }
        return ValidPoints;
    }
    bool CheckTargetMatch(Vector3Int pos, List<ValidTargets> targets, string faction)
    {
        foreach (var target in targets)
            if (CheckIfTargetis(pos, target, faction))
            {
                //.LogError("faction Condetion Met " + pos + target + " " + faction);
                //Debug.Break();
                return true;
            }
        return false;
    }
    public List<Vector3Int> GetArea(Vector3Int fromPoint, Vector3Int direction, AoeParams aoeParams, string faction)
    {
        List<Vector3Int> ValidPos = new List<Vector3Int>();
        ValidPos = GlobalCal.GenerateArea(aoeParams.TypeOfRange, fromPoint, fromPoint + direction, aoeParams.ExploreRangeMax);
        GameEvents.current.reticalManager.reDrawInValidTiles(ValidPos);
        ValidPos = GlobalCal.FilterWithFunc(ValidPos, simplifiedMatchCondition);
        GameEvents.current.reticalManager.reDrawValidTiles(ValidPos);
        return ValidPos;
        bool simplifiedMatchCondition(Vector3Int pos)
        {
            return CheckTargetMatch(pos, aoeParams.AffectTargets, faction);
        }
    }

    MoveDictionaryManager moveDictionaryManager;
    public List<Vector3Int> FindOptimalPath(Vector3Int startPos, List<Vector3Int> endPos, AbilityData abilityData, Func<Vector3Int, List<Vector3Int>> GenratePossiblitiesForAttack)
    //public List<Vector3Int> FindOptimalPath(Vector3Int startPos, List<Vector3Int> endPos, AbilityData abilityData, bool priortiseFistEndPos = false)
    {
        string AStarDebug = "Starting Astar Navigation from Point ";//Generate Node Data will put
        List<Node> openList = new List<Node>();
        List<Vector3Int> closeList = new List<Vector3Int>();
        List<Node> historyNode = new List<Node>();
        if (endPos.Count == 0)
        {
            Debug.Log("End Pos 0; There are no Targets what are you expecting");
            return new List<Vector3Int>() { startPos };
        }
        openList.Add(generateNodeData(startPos, null));
        closeList.Add(startPos);
        int maxLoops = 0;
        while (maxLoops != 20)
        {
            openList = universalCalculator.ConvertSortedListToNormalList(universalCalculator.SortListWithVar(openList, getFCost));
            Node currentNode = openList.First();
            openList.Remove(currentNode);
            AStarDebug += "\n  Step " + maxLoops + ": Evaluating Node " + currentNode.nodeID + " as its F Cost was " + getFCost(currentNode) + " Neighbours: ";
            if (GenratePossiblitiesForAttack(currentNode.nodeID).Count > 0)
            {
                return reconstructPath(currentNode, "Attack Possible From Current Node: Path Found!");
            }
            if (endPos.Contains(currentNode.nodeID))
            {
                return reconstructPath(currentNode, "End Pos contains NodeID: Path Found!");
            }
            foreach (Vector3Int neighbourPoint in moveDictionaryManager.GenerateAbiltyPointMap(abilityData, currentNode.nodeID).Keys.ToList())
            {
                if (closeList.Contains(neighbourPoint))
                    continue;
                else
                    closeList.Add(neighbourPoint);
                Node neighbourNode = generateNodeData(neighbourPoint, currentNode);
                currentNode.AddNeighbours(neighbourNode);
                openList.Add(neighbourNode);
                historyNode.Add(neighbourNode);
            }
            if (openList.Count == 0)
            {
                historyNode = universalCalculator.ConvertSortedListToNormalList(universalCalculator.SortListWithVar(historyNode, getFCost));
                return reconstructPath(historyNode[0], "OpenList Null Using History");
            }
            maxLoops++;
        }
        historyNode = universalCalculator.ConvertSortedListToNormalList(universalCalculator.SortListWithVar(historyNode, getFCost));
        return reconstructPath(historyNode[0], "Creating Path From Histrory");
        Node generateNodeData(Vector3Int pos, Node previousNode = null)
        {
            Vector3Int closestEndPos;
            closestEndPos = universalCalculator.SortListAccordingtoDistanceFromPoint(endPos, pos).First();
            float Hcost = Vector3Int.Distance(pos, closestEndPos);//distance to endPoint
            float Gcost = Vector3Int.Distance(pos, startPos);//distance to startPoint 
                                                             //AStarDebug += "\n" + "         " + pos + " had H and G cost of " + Hcost + " " + Gcost;
            AStarDebug += pos + ", ";
            Node currentNode = new Node(pos, Hcost, Gcost, previousNode);
            return currentNode;
        }
        float getFCost(Node node)
        {
            return node.FScore;
        }
        List<Vector3Int> reconstructPath(Node node, string PrefixDebug)
        {
            List<Vector3Int> path = new List<Vector3Int>();
            string Text = "PathData";
            while (node.previousNode != null)
            {
                path.Add(node.nodeID);
                Text += " > " + node.nodeID;
                node = node.previousNode;
            }
            //Debug.Log(PrefixDebug + " " + path.Count + " Jumps needed" + "\n" + Text + "\n" + AStarDebug);
            path.Reverse();
            return path;
        }
    }
}
