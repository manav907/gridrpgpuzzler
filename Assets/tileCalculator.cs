using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileCalculator : MonoBehaviour
{

    public List<Vector3Int> generateRange(Vector3 start, Vector3 end)
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
}
