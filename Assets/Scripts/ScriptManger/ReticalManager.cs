using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ReticalManager : MonoBehaviour
{
    [SerializeField] Vector3 reticalPos;
    void FixedUpdate()
    {
        setRetical();
    }
    public GameObject characterRetical;
    void setRetical()
    {
        reticalPos = getMovePoint();
        characterRetical.transform.position = reticalPos;
    }
    public Tilemap Grid;
    Vector3 worldPos;
    Vector3Int tilePos;
    public Vector3Int getMovePoint()
    {
        worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        tilePos = Grid.WorldToCell(worldPos);
        return tilePos;
    }

}
