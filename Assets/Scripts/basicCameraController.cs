using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicCameraController : MonoBehaviour
{

    GameObject thisGameObject;
    void Start()
    {
        thisGameObject = this.gameObject;
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
    }
}
