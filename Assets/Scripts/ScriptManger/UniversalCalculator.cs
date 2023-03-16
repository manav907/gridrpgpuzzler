using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UniversalCalculator : MonoBehaviour
{

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
    public List<Vector3Int> generateRangeFromPoint(Vector3Int thisPoint, int rangeOfAction)
    {
        Vector3 centerPos = thisPoint;
        Vector3 startRange = centerPos - new Vector3(rangeOfAction, rangeOfAction);
        Vector3 endRange = centerPos + new Vector3(rangeOfAction, rangeOfAction);
        List<Vector3Int> listOfRanges = generateRangeFrom2Vectors(startRange, endRange);
        return listOfRanges;
    }
    public Vector3Int convertToVector3Int(Vector3 thisVector)
    {
        return new Vector3Int((int)thisVector.x, (int)thisVector.y, (int)thisVector.z);
    }
    public int SelectRandomBetweenZeroAndInt(int ListCount)
    {
        var random = new System.Random();
        return random.Next(ListCount);
    }
    public void doThis()
    {
        Debug.Log("do this invoked or FireBall");
    }
    public List<Vector3Int> SortListAccordingtoDistanceFromPoint(List<Vector3Int> thisV3IntList, Vector3Int thisPoint)
    {
        SortedList<float, Vector3Int> sortedListOfDistance = new SortedList<float, Vector3Int>();
        foreach (var element in thisV3IntList)
        {
            float thisDistance = Vector3Int.Distance(element, thisPoint);
            while (sortedListOfDistance.ContainsKey(thisDistance))
            {
                thisDistance += 0.001f;
            }
            sortedListOfDistance.Add(thisDistance, element);
        }


        inputDynamicValue = sortedListOfDistance;
        outputDynamicValue = new List<Vector3Int>();
        convertSortedListToNormalList();
        return outputDynamicValue;
    }

    dynamic inputDynamicValue;
    dynamic outputDynamicValue;
    void convertSortedListToNormalList()
    {
        foreach (var eachValue in inputDynamicValue.Values)
        {
            outputDynamicValue.Add(eachValue);
        }
    }
    public List<GameObject> SortBySpeed(List<GameObject> thisList)
    {

        SortedList<float, GameObject> sortedList = new SortedList<float, GameObject>();
        foreach (GameObject thisChar in thisList)
        {
            float speedofChar = thisChar.GetComponent<characterDataHolder>().speedValue;
            while (sortedList.ContainsKey(speedofChar))
            {
                speedofChar += 0.001f;
            }
            sortedList.Add(speedofChar, thisChar);
        }

        inputDynamicValue = sortedList;
        outputDynamicValue = new List<GameObject>();
        convertSortedListToNormalList();
        outputDynamicValue.Reverse();//Only Because the list Operates in Reverse
        return outputDynamicValue;
    }
}
