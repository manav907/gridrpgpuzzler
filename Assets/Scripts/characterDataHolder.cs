using System;
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
    public bool isCharacterTurn;
    [SerializeField] private TextMesh Heatlh;
    void Start()
    {
        UpdateCharacterData();
    }
    public List<string> GetCharacterMoveList()
    {
        List<string> defaultMovesAvaliable;
        defaultMovesAvaliable = new List<string>();
        defaultMovesAvaliable.AddRange(new List<string>
        {
            "Move",
            "Attack",
            "FireBall",
            "End Turn"
        });
        return defaultMovesAvaliable;
    }
    public void UpdateCharacterData()
    {
        if (!isCharacterTurn)
            thisAnimation.SetFloat("BlendSpeed", 0f);
        else
            thisAnimation.SetFloat("BlendSpeed", 1f);
        Heatlh.text = health + "";
        if (health <= 0)
        {
            Debug.Log("I am dying");
            //Destroy(this.gameObject);
        }
    }
    [SerializeField] Animator thisAnimation;
}
