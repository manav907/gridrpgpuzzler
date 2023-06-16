using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformAnimationScript : MonoBehaviour
{
    public static TransformAnimationScript current;
    void Awake()
    {
        current = this;
    }
    private Dictionary<Transform, MoveQueueData> moveQueueSystem;
    public IEnumerator MoveUsingQueueSystem(Transform transform, Vector3 movePoint, float moveTime)
    {
        if (moveQueueSystem == null)//Checks it the Dict Exits
        {
            moveQueueSystem = new Dictionary<Transform, MoveQueueData>();
        }
        if (!moveQueueSystem.ContainsKey(transform))//Initilizes List of Keypair and add transform
        {
            var MoveQueueData = new MoveQueueData();
            moveQueueSystem.Add(transform, MoveQueueData);
            Debug.Log("Created New Entry");
        }
        moveQueueSystem[transform].add(movePoint, moveTime);
        Debug.Log(transform.name + " added" + movePoint + moveTime);
        if (moveQueueSystem[transform].hasRun)
        {

        }
        else
        {
            Debug.Log("Starting MoveBw");
            yield return StartCoroutine(MoveTransBetweenpoints(transform));
        }

    }
    IEnumerator MoveTransBetweenpoints(Transform transform)
    {
        moveQueueSystem[transform].hasRun = true;
        MoveQueueData data = moveQueueSystem[transform];

        int currentIndex = 0;
        int TotalItems = data.NumberOfItems;


        while (currentIndex < TotalItems)//Iterates through Each Item
        {
            float moveTime = data.timeToTake[currentIndex];
            Vector3 movePoint = data.points[currentIndex];
            float elapsedTime = 0;

            Vector3 startingPosition = transform.position;

            yield return null;
            //yield return new WaitForFixedUpdate();//This makes stuff very laggy
            Debug.Log("moving" + transform.name + " to " + movePoint + " in " + moveTime + " seconds");
            while (elapsedTime < moveTime)
            {
                yield return null;
                //yield return new WaitForFixedUpdate();//This makes stuff very laggy
                elapsedTime += Time.deltaTime;
                float percentageMoved = Mathf.Clamp01(elapsedTime / moveTime);
                transform.position = Vector3.Lerp(startingPosition, movePoint, percentageMoved);
                //debugLine += "ElapsedTime " + elapsedTime + " and Current Position" + transform.position + " At the End\n";

            }

            data.RemoveAt0();//removes the entry we just worked on
            //currentIndex++;//Moves To Next Index
            TotalItems = data.NumberOfItems;//Updates if new items were added
        }
        moveQueueSystem.Remove(transform);
        Debug.Log("completed removoeing transform");
        yield return null;
    }
    class MoveQueueData
    {
        public List<Vector3> points;
        public List<float> timeToTake;
        int numberOfItems;
        public int NumberOfItems { get { return numberOfItems; } }

        public bool hasRun;
        public void add(Vector3 point, float time)
        {
            points.Add(point);
            timeToTake.Add(time);
            numberOfItems++;
        }
        public void RemoveAt0()
        {
            points.RemoveAt(0);
            timeToTake.RemoveAt(0);
            numberOfItems--;
        }
        public MoveQueueData()
        {
            points = new List<Vector3>();
            timeToTake = new List<float>();
            hasRun = false;
        }
    }
}
