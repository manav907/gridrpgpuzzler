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
    public Vector3Int convertToVector3Int(Vector3 thisVector)
    {
        return new Vector3Int((int)thisVector.x, (int)thisVector.y, (int)thisVector.z);
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
    public List<Vector3Int> getSimpleArc(Vector3Int fromPoint, Vector3Int atPoint, float rangeOfAction)
    {
        var retiacalTiles = new List<Vector3Int>();
        Vector3Int direction = Vector3Int.RoundToInt(Vector3.Normalize(atPoint - fromPoint));
        int xDirection = direction.x;
        int yDirection = direction.y;
        foreach (Vector3Int pos in generateTaxiRangeFromPoint(fromPoint, rangeOfAction))
        {
            Vector3Int consideredDirection = Vector3Int.RoundToInt(Vector3.Normalize(pos - fromPoint));
            if ((consideredDirection.x != -xDirection || consideredDirection.y != -yDirection))
                if ((consideredDirection.x == xDirection && consideredDirection.y == yDirection) || (consideredDirection.x == 0 || consideredDirection.y == 0))
                {
                    retiacalTiles.Add(pos);
                }
        }
        return retiacalTiles;
    }
    public List<Vector3Int> getSmallAxeArc(Vector3Int fromPoint, Vector3Int atPoint)
    {
        var retiacalTiles = new List<Vector3Int>();
        Vector3Int direction = Vector3Int.RoundToInt(Vector3.Normalize(atPoint - fromPoint));
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
        //Vector3Int direction = Vector3Int.RoundToInt(Vector3.Normalize(atPoint - fromPoint));
        Vector3Int direction = Vector3Int.RoundToInt(Vector3.Normalize(atPoint - fromPoint));
        int xDirection = direction.x;
        int yDirection = direction.y;
        foreach (Vector3Int pos in generateTaxiRangeFromPoint(fromPoint, rangeOfAction))
        {
            Vector3Int consideredDirection = Vector3Int.RoundToInt(Vector3.Normalize(pos - fromPoint));
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

    ///List Handeling
    public int SelectRandomBetweenZeroAndInt(int ListCount)
    {
        var random = new System.Random();
        return random.Next(ListCount);
    }
    List<T> convertSortedListToNormalList<T>(SortedList<float, T> inputList)
    {
        return inputList.Values.ToList();
    }
    List<T> getReverseList<T>(List<T> thislist)
    {
        thislist.Reverse();
        return thislist;
    }

    System.Random rand = new System.Random(64);
    public SortedList<float, T> sortListWithVar<T>(List<T> thisList, Func<T, float> thisFloat)
    // here Func<T, float> thisFloat represent a function that takes Function of the float distance or float speed 
    //with variable T(from point or GameObject) then calculates a float value 
    //which is representrdd by the later part of Func<T, float> thisFloat
    {
        SortedList<float, T> newList = new SortedList<float, T>();
        foreach (var element in thisList)
        {
            float thisDistance = thisFloat(element);
            while (newList.ContainsKey(thisDistance))
            {
                //thisDistance += 0.001f;
                // add a small random value to the distance
                thisDistance += rand.Next(1, 100) / 1000.0f;
            }
            newList.Add(thisDistance, element);
        }
        return newList;
    }

    public List<Vector3Int> SortListAccordingtoDistanceFromPoint(List<Vector3Int> thisV3IntList, Vector3Int toPoint)
    {
        SortedList<float, Vector3Int> sortedListOfDistance = sortListWithVar(thisV3IntList, distance);
        return convertSortedListToNormalList(sortedListOfDistance);
        //declaring necessary function to be used as a delegate
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
            if (originalList.GetType() == CompareToList.GetType())
            {
                //Debug.Log("Same Type of List Will Contine");
                Debug.Log(originalList.Count + " is the number of itmes in original list and in the new list the bumber of itesms are " + CompareToList.Count);
                for (int i = 0; i < originalList.Count; i++)
                {
                    Debug.Log("Original Item " + originalList[i] + " was replaced by" + CompareToList[i]);
                }
            }

            else
            {
                Debug.Log("Not the Same will return original List");
                return originalList;
            }
        }
        return CompareToList;
    }
    //
    public void MoveTransFromBetweenPoint(Transform transform, List<Vector3> movePoints, float moveTime)
    {
        StartCoroutine(MoveCoRoutine());
        IEnumerator MoveCoRoutine()
        {
            string debugLine = "";
            yield return new WaitForSeconds(0.2f);
            foreach (Vector3 movePoint in movePoints)
            {
                float elapsedTime = 0;
                Vector3 startingPosition = transform.position;
                debugLine = ("Character Started at " + startingPosition + " and Will Move to " + movePoint + " and it will take time of " + moveTime) + ":\n";
                while (elapsedTime < moveTime)
                {
                    yield return null;
                    //yield return new WaitForFixedUpdate();//This makes stuff very laggy
                    elapsedTime += Time.deltaTime;
                    float percentageMoved = Mathf.Clamp01(elapsedTime / moveTime);
                    transform.position = Vector3.Lerp(startingPosition, movePoint, percentageMoved);
                    debugLine += "ElapsedTime " + elapsedTime + " and Current Position" + transform.position + " At the End\n";

                }
                reticalManager.reDrawShadows();
            }
            //Debug.Log(debugLine);
            transform.position = movePoints.Last();
        }
    }
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
public static class BackupManager
{
    public static void BackupSOString(ScriptableObject scriptableObject, string json)
    {
        string filePath = AssetDatabase.GetAssetPath(scriptableObject);
        string fileName = scriptableObject.name;
        string folderPath = System.IO.Path.GetDirectoryName(filePath);
        string backupFolderPath = Path.Combine(folderPath, fileName + "_BackupsFolder");

        if (Directory.Exists(backupFolderPath))
        {
            string currentTime = DateTime.Now.ToString("dd.MM.yyyy - hh.mm.ss tt");
            string backupFilePath = Path.Combine(backupFolderPath, fileName + "_" + currentTime + ".jsonStringBackup");
            string[] backupFiles = Directory.GetFiles(backupFolderPath, "*.jsonStringBackup");
            if (backupFiles.Length > 0)
            {
                Array.Sort(backupFiles, (a, b) => File.GetCreationTime(b).CompareTo(File.GetCreationTime(a)));
                string lastSavedFileLocation = backupFiles[0];
                string lastSavedJson = System.IO.File.ReadAllText(lastSavedFileLocation);
                if (lastSavedJson == json)
                {
                    Debug.LogWarning("No new Changes in Files will not create dupplicate File");
                    return;
                }
                // Use the lastSavedJson as needed
            }
            else
            {
                // No backup files found in the directory
            }
            System.IO.File.WriteAllText(backupFilePath, json);
            Debug.Log("File Backup Created" + backupFilePath + " \n" + currentTime);
        }
        else
        {
            Debug.LogError("Backup Folder Does not Exist" + backupFolderPath);
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