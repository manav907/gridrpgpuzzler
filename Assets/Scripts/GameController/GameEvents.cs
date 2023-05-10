using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;
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
}
