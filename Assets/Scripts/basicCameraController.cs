using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicCameraController : MonoBehaviour
{

    GameObject thisGameObject;
    MapManager mapManager;
    ReticalManager reticalManager;
    void Start()
    {
        thisGameObject = this.gameObject;
    }
    public void setVariables(GameObject gameObject)
    {
        mapManager = gameObject.GetComponent<MapManager>();
        reticalManager = gameObject.GetComponent<ReticalManager>();
    }
    int speedreudce = 10;
    // Update is called once per frame
    void Update()
    {
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //mapManager.getCellData(reticalManager.getMovePoint());
            Vector3Int thisPos = reticalManager.getMovePoint();
            Debug.Log("At Pos " + thisPos + " AnyCharacter here was = " + mapManager.cellDataDir[thisPos].isCellHoldingCharacer());
            //mapManager.cellDataDir[thisPos].ReadInfo();
        }
    }
}
