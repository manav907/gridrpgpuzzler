using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCameraController : MonoBehaviour
{

    GameObject thisGameObject;
    MapManager mapManager;
    ReticalManager reticalManager;
    UniversalCalculator universalCalculator;
    MoveDictionaryManager moveDictionaryManager;
    [SerializeField] ExploreMethod example;
    [SerializeField] float moveTime;
    void Start()
    {
        thisGameObject = this.gameObject;
    }
    public void setVariables(GameObject gameObject)
    {
        mapManager = gameObject.GetComponent<MapManager>();
        reticalManager = gameObject.GetComponent<ReticalManager>();
        universalCalculator = gameObject.GetComponent<UniversalCalculator>();
        moveDictionaryManager = gameObject.GetComponent<MoveDictionaryManager>();
    }
    public void setCameraPos(Vector3 pos)
    {
        StartCoroutine(TransformAnimationScript.current.MoveUsingQueueSystem(this.transform, new Vector3(pos.x, pos.y, this.transform.position.z), moveTime));
    }
    int speedreudce = 10;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            GameEvents.current.reloadScene();
        }
        if (Input.GetKey(KeyCode.W))
        {
            thisGameObject.transform.position = thisGameObject.transform.position + Vector3.up / speedreudce;
        }
        if (Input.GetKey(KeyCode.S))
        {
            thisGameObject.transform.position = thisGameObject.transform.position + Vector3.down / speedreudce;
        }
        if (Input.GetKey(KeyCode.A))
        {
            thisGameObject.transform.position = thisGameObject.transform.position + Vector3.left / speedreudce;
        }
        if (Input.GetKey(KeyCode.D))
        {
            thisGameObject.transform.position = thisGameObject.transform.position + Vector3.right / speedreudce;
        }
        if (Input.GetKeyDown(KeyCode.Space) && false)
        {
            //mapManager.getCellData(reticalManager.getMovePoint());
            Vector3Int MousePos = reticalManager.getIntPoint();
            Vector3Int currentCharPos = moveDictionaryManager.characterCS.GetCharV3Int();
            Vector3Int direction = GlobalCal.GetNormalizedDirection(currentCharPos, MousePos);
            string faction = moveDictionaryManager.characterCS.Faction;
            List<Vector3Int> TotalArea = new List<Vector3Int>();
            Debug.Log("Current Faction  " + faction);
            foreach (Vector3Int point in mapManager.CheckDirection(currentCharPos, direction, example.ProjectilesFired, faction))
            {
                TotalArea.AddRange(mapManager.GetArea(point, direction, example.HelpUiArea, faction));
            }
            reticalManager.reDrawInValidTiles(TotalArea);

            //reticalManager.reDrawInValidTiles(mapManager.GetArea(currentCharPos, direction, example.ProjectilesFired.AffectsArea, faction));
            /* var area = mapManager.CheckDirection(currentCharPos, GlobalCal.GetNormalizedDirection(currentCharPos, MousePos), example.ProjectilesFired, faction);
            Debug.Log(area + " " + GlobalCal.GetNormalizedDirection(currentCharPos, MousePos)); */
            //reticalManager.reDrawInValidTiles(area);
        }
    }
}
