using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    [SerializeField] GameObject scriptManager;
    public static GameEvents current;
    int TotalEnemies = 0;
    private void Awake()
    {
        current = this;
        onCharacterDeath += delegate { Debug.Log("Even dEath Called"); };
    }
    public event Action onCharacterDeath;
    public void CheckDath()
    {
        if (onCharacterDeath != null)
        {
            onCharacterDeath();
        }
    }
    public void oneEnemyDied()
    {
        TotalEnemies--;
        Debug.Log("Remaining Enemies are" + TotalEnemies);
        CheckWinCondidion();
    }
    void CheckWinCondidion()
    {
        if (TotalEnemies == 0)
        {
            Debug.Log("Event of Game Win");
            Debug.Break();
        }
    }
    public void addEnemy()
    {
        TotalEnemies++;
    }
}
