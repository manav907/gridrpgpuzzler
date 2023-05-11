using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEvents : MonoBehaviour
{
    [SerializeField] GameObject scriptManager;
    public static GameEvents current;
    [SerializeField] TMPro.TextMeshProUGUI textBox;
    [SerializeField] string[] listOFDialog;
    int currentDialog = 0;
    int TotalEnemies = 0;
    private void Awake()
    {
        current = this;
        onCharacterDeath += delegate { Debug.Log("Even dEath Called"); };
        TriggerNextDialogAction += TriggerCharacterDialogs;
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
            TriggerNextDialogAction -= TriggerCharacterDialogs;
            Action winGameDialog = delegate
            {
                textBox.text = "Event of Game Win";
            };
            TriggerNextDialogAction += winGameDialog;

            //Debug.Break();
        }
    }
    public void addEnemy()
    {
        TotalEnemies++;
    }
    public event Action TriggerNextDialogAction;
    public void TriggerNextDialog()
    {
        if (TriggerNextDialogAction != null)
            TriggerNextDialogAction();
    }
    void TriggerCharacterDialogs()
    {

        if (currentDialog + 1 > listOFDialog.Length)
        { textBox.text = ""; return; }

        textBox.text = listOFDialog[currentDialog];
        currentDialog++;
    }

}
