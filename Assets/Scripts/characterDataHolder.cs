using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterDataHolder : MonoBehaviour
{
    public int health = 3;
    public int rangeOfMove = 5;
    public int attackRange = 2;
    public int AttackDamage = 2;
    public int speedValue = 3;
    public List<string> defaultMovesAvaliable;
    //public List<string> specialMovesAvaliable;
    void Start()
    {
        defaultMovesAvaliable = new List<string>();
        defaultMovesAvaliable.AddRange(new List<string>
        {
            "Move",
            "Attack",
            "End Turn"
        });
    }
    public void UpdateCharacterData()
    {
        if (health <= 0)
        {
            Debug.Log("I am dying");
            //Destroy(this.gameObject);
        }
    }
}
