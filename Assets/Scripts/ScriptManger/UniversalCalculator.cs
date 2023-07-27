using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using UnityEditor;
using System.IO;

public class UniversalCalculator : MonoBehaviour
{

    ReticalManager reticalManager;
    public void setVariables()
    {
        reticalManager = GetComponent<ReticalManager>();
    }
    ///String handeling
    public string CamelCaseToSpaces(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        StringBuilder output = new StringBuilder();
        output.Append(input[0]);

        for (int i = 1; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                output.Append(' ');
            }
            output.Append(input[i]);
        }

        return output.ToString();
    }

    //Vector handeling
    public Vector3Int castAsV3Int(Vector3 thisVector)
    {
        return new Vector3Int((int)thisVector.x, (int)thisVector.y, (int)thisVector.z);
    }
    public List<Vector3Int> castListAsV3Int(List<Vector3> v3List)
    {
        var List = new List<Vector3Int>();
        foreach (var pos in v3List)
        {
            List.Add(castAsV3Int(pos));
        }
        return List;
    }
    public List<Vector3> castListAsV3(List<Vector3Int> v3IntList)
    {
        var List = new List<Vector3>();
        foreach (var pos in v3IntList)
        {
            List.Add(pos);
        }
        return List;
    }
    public List<Vector3Int> generateRangeFrom2Vectors(Vector3 start, Vector3 end)
    {
        List<Vector3Int> listOfRanges = new List<Vector3Int>();

        int startx = (int)start.x;
        int starty = (int)start.y;
        int endx = (int)end.x;
        int endy = (int)end.y;
        if (startx > endx)
        {
            int x = startx;
            startx = endx;
            endx = x;
        }
        if (starty > endy)
        {
            int x = starty;
            starty = endy;
            endy = x;
        }
        for (int x = startx; x <= endx; x++)
        {
            for (int y = starty; y <= endy; y++)// problem when y > than y
            {
                Vector3Int atXY = new Vector3Int(x, y, 0);
                listOfRanges.Add(atXY);
            }
        }
        return listOfRanges;
    }
    public List<Vector3Int> generateRangeFromPoint(Vector3Int thisPoint, float rangeOfAction)
    {
        Vector3 centerPos = thisPoint;
        Vector3 startRange = centerPos - new Vector3(rangeOfAction, rangeOfAction);
        Vector3 endRange = centerPos + new Vector3(rangeOfAction, rangeOfAction);
        List<Vector3Int> listOfRanges = generateRangeFrom2Vectors(startRange, endRange);
        return listOfRanges;
    }
    public List<Vector3Int> generateTaxiRangeFromPoint(Vector3Int thisPoint, float rangeOfAction)
    {
        List<Vector3Int> listOfRanges = generateRangeFromPoint(thisPoint, rangeOfAction);
        List<Vector3Int> outputList = new List<Vector3Int>();
        foreach (Vector3Int point in listOfRanges)
        {
            float distance = Vector3Int.Distance(thisPoint, point);
            if (distance <= rangeOfAction)
            {
                outputList.Add(point);
            }
        }
        return outputList;
    }
    public List<Vector3Int> generateDirectionalRange(Vector3Int fromPoint, float rangeOfAction, List<Vector3Int> reffrences)
    {
        int range = Mathf.FloorToInt(rangeOfAction);
        List<Vector3Int> output = new List<Vector3Int>();
        while (range != 0)
        {
            foreach (var reffence in reffrences)
            {
                output.Add(new Vector3Int(reffence.x * range, reffence.y * range, 0) + fromPoint);
            }
            range--;
        }
        output.Reverse();
        return output;
    }
    public List<Vector3Int> generate9WayReffence()
    {
        List<Vector3Int> reffrence = new List<Vector3Int>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                reffrence.Add(new Vector3Int(j, i, 0));
            }
        }
        return reffrence;
    }
    public List<Vector3Int> getSimpleArc(Vector3Int fromPoint, Vector3Int atPoint, float rangeOfAction)
    {
        var retiacalTiles = new List<Vector3Int>();
        Vector3Int direction = getNormalizedDirection(atPoint, fromPoint);
        int xDirection = direction.x;
        int yDirection = direction.y;
        foreach (Vector3Int pos in generateTaxiRangeFromPoint(fromPoint, rangeOfAction))
        {
            Vector3Int consideredDirection = getNormalizedDirection(pos, fromPoint);
            if ((consideredDirection.x != -xDirection || consideredDirection.y != -yDirection))
                if ((consideredDirection.x == xDirection && consideredDirection.y == yDirection) || (consideredDirection.x == 0 || consideredDirection.y == 0))
                {
                    retiacalTiles.Add(pos);
                }
        }
        return retiacalTiles;
    }/* 
    public Vector3Int getNearestPoint(Vector3Int fromPoint, List<Vector3Int> toWardsPoint)
    {
        SortListAccordingtoDistanceFromPoint(toWardsPoint,fromPoint);
    } */
    public Vector3Int getNormalizedDirection(Vector3Int fromPoint, Vector3Int atPoint)
    {
        return Vector3Int.RoundToInt(Vector3.Normalize(atPoint - fromPoint));
    }
    public Vector3Int getLeftDirection(Vector3Int forwardDirection)
    {
        return new Vector3Int(-forwardDirection.y, forwardDirection.x, 0);
    }

    public Vector3Int getRightDirection(Vector3Int forwardDirection)
    {
        return new Vector3Int(forwardDirection.y, -forwardDirection.x, 0);
    }
    public List<Vector3Int> PointsInDirectionFilter(Vector3Int fromPoint, Vector3Int toPoint, List<Vector3Int> checkPoints)
    {
        var dircetion = getNormalizedDirection(fromPoint, toPoint);
        var output = new List<Vector3Int>();
        foreach (var point in checkPoints)
        {
            var pointDirection = getNormalizedDirection(fromPoint, point);
            if (pointDirection == dircetion)
                output.Add(point);
        }
        return output;
    }
    public Dictionary<Vector3Int, List<Vector3Int>> DirectionToCellSnapData(Vector3Int fromPoint, List<Vector3Int> toWardsPoint)
    {
        var dict = new Dictionary<Vector3Int, List<Vector3Int>>();
        foreach (var point in toWardsPoint)
        {
            var dircetion = getNormalizedDirection(fromPoint, point);
            if (!dict.ContainsKey(dircetion))
            {
                dict.Add(dircetion, new List<Vector3Int>());
            }
            dict[dircetion].Add(point);
        }
        return dict;
    }
    public List<Vector3Int> getSmallAxeArc(Vector3Int fromPoint, Vector3Int atPoint)
    {
        var retiacalTiles = new List<Vector3Int>();
        Vector3Int direction = getNormalizedDirection(atPoint, fromPoint);
        Vector3Int first = new Vector3Int();
        Vector3Int second = new Vector3Int();
        if (direction.x != 0 && direction.y != 0)
        {
            first = direction + new Vector3Int(direction.x, 0, 0);
            second = direction + new Vector3Int(0, direction.y, 0);
        }
        else
        {
            first = direction + new Vector3Int(direction.y, direction.x, 0);
            second = direction + new Vector3Int(-direction.y, -direction.x, 0);
        }
        retiacalTiles.Add(first);
        retiacalTiles.Add(second);
        retiacalTiles.Add(direction);
        return retiacalTiles;
    }
    public List<Vector3Int> generateComplexArc(Vector3Int fromPoint, Vector3Int atPoint, float rangeOfAction, bool doAxeCheck = true)
    {
        var arcTiles = new List<Vector3Int>();
        Vector3Int direction = getNormalizedDirection(atPoint, fromPoint);
        int xDirection = direction.x;
        int yDirection = direction.y;
        foreach (Vector3Int pos in generateTaxiRangeFromPoint(fromPoint, rangeOfAction))
        {
            Vector3Int consideredDirection = getNormalizedDirection(pos, fromPoint);
            bool SpearCheck = consideredDirection.x == xDirection || consideredDirection.y == yDirection;
            bool AxeCheck;
            if (doAxeCheck)//This is smaller
            { AxeCheck = (consideredDirection.x != -xDirection && consideredDirection.y != -yDirection) || direction == consideredDirection; }
            else//This is Larger
            { AxeCheck = (consideredDirection.x != -xDirection || consideredDirection.y != -yDirection) || direction == consideredDirection; }
            if (SpearCheck)//Spear Condition
                if (AxeCheck)//Axe Condition
                { arcTiles.Add(pos); }
        }
        return arcTiles;

    }
    public List<Vector3Int> generatePoint(Vector3Int fromPoint, Vector3Int toWardPoint, float rangeOfAction)
    {
        var output = new List<Vector3Int>();
        Vector3Int consideredDirection = getNormalizedDirection(fromPoint, toWardPoint);

        return output;
    }
    ///List Handeling
    public int SelectRandomBetweenZeroAndInt(int ListCount)
    {
        var random = new System.Random();
        return random.Next(ListCount);
    }
    public List<T> convertSortedListToNormalList<T>(SortedList<float, T> inputList)
    {
        return inputList.Values.ToList();
    }
    List<T> getReverseList<T>(List<T> thislist)
    {
        thislist.Reverse();
        return thislist;
    }

    System.Random rand = new System.Random(64);
    public SortedList<float, T> sortListWithVar<T>(List<T> thisList, Func<T, float> thisFloat, Func<T, float> tieBreaker = null)
    // here Func<T, float> thisFloat represent a function that takes Function of the float distance or float speed 
    //with variable T(from point or GameObject) then calculates a float value 
    //which is representrdd by the later part of Func<T, float> thisFloat
    {
        SortedList<float, T> newList = new SortedList<float, T>();
        foreach (var element in thisList)
        {
            float thisDistance = thisFloat(element);
            if (newList.ContainsKey(thisDistance))
            {
                if (tieBreaker != null)
                {
                    thisDistance += tieBreaker(element);
                }
            }
            while (newList.ContainsKey(thisDistance))
            {
                //thisDistance += 0.001f;
                // add a small random value to the distance
                thisDistance += 1 / 1000.0f;
            }
            newList.Add(thisDistance, element);
        }
        return newList;
    }

    public List<Vector3Int> SortListAccordingtoDistanceFromPoint(List<Vector3Int> thisV3IntList, Vector3Int toPoint, bool alignToPoint = true)
    {
        if (!alignToPoint)
        {
            return convertSortedListToNormalList(sortListWithVar(thisV3IntList, distance, isAligned));
        }
        return convertSortedListToNormalList(sortListWithVar(thisV3IntList, distance));
        //declaring necessary function to be used as a delegate
        float isAligned(Vector3Int fromPoint)
        {
            Vector3Int AlignmentToEndPos = getNormalizedDirection(fromPoint, toPoint);
            if (AlignmentToEndPos.x == 0 || AlignmentToEndPos.y == 0)
                return 1 / 10;
            return 0 / 10;
        }
        float distance(Vector3Int fromPoint)
        {
            return Vector3Int.Distance(fromPoint, toPoint);
        }
    }
    public List<T> filterOutList<T>(List<T> GivenList, List<T> ListOfValidObjects)
    {
        var outputList = new List<T>();
        foreach (T GivenObjectFromGivenList in GivenList)
        {
            if (ListOfValidObjects.Contains(GivenObjectFromGivenList))
            {
                outputList.Add(GivenObjectFromGivenList);
                //Debug.Log(GivenObjectFromGivenList + " was included in the list");
            }
        }
        return outputList;
    }

    public void DebugEachItemInList<T>(IEnumerable<T> collection)
    {
        foreach (T item in collection)
        {
            Debug.Log(item);
        }
    }
    public List<T> CompareAndReplace<T>(List<T> originalList, List<T> CompareToList, bool DebugCompare)
    {
        if (DebugCompare == true)
        {
            if (originalList.Equals(CompareToList))
            { return originalList; }

            string debugLine = originalList.Count + " is the number of itmes in original list and in the new list the bumber of itesms are " + CompareToList.Count;
            //Debug.Log(originalList.Count + " is the number of itmes in original list and in the new list the bumber of itesms are " + CompareToList.Count);
            for (int i = 0; i < originalList.Count; i++)
            {
                if (originalList[i].Equals(CompareToList[i]))
                {
                    debugLine += "\n " + " Matched element " + i;
                }
                else
                {
                    debugLine += "\n " + " Replaced element " + i + " " + originalList[i] + " to " + CompareToList[i];
                }
            }
            Debug.Log(debugLine);
        }
        return CompareToList;
    }
    //
}

public static class GlobalCal
{
    public static List<T> createCopyListUsingConstructor<T>(List<T> GivenList)
    {
        var newList = new List<T>();
        foreach (T item in GivenList)
        {
            T newItem = (T)Activator.CreateInstance(typeof(T), item);
            newList.Add(newItem);
        }
        return newList;
    }
    public static bool compareBool(bool compareWith, BoolEnum compareTo)
    {
        switch (compareTo)
        {
            case BoolEnum.TrueOrFalse:
                return true;
            case BoolEnum.True:
                return compareWith == true;
            case BoolEnum.False:
                return compareWith == false;
            default:
                return false;
        }

    }

}


public static class HUDStuff
{
    static GUIStyle style;
    static HUDStuff()
    {
        style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;
    }
}