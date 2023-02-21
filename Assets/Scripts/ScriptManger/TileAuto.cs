using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileAuto : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public Tilemap noCollider;
    public Tilemap Collider;
    public Tile thattile;

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Hello World");
            noCollider.SetTile(new Vector3Int(0,0,0),thattile);
        }
        */
    }
}
