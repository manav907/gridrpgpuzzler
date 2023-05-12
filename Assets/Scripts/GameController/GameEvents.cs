using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameEvents : MonoBehaviour
{
    [SerializeField] GameObject scriptManager;
    public static GameEvents current;
    [SerializeField] TMPro.TextMeshProUGUI textBox;
    [SerializeField] string[] listOFDialog;
    int currentDialog = 0;
    int TotalEnemies = 0;
    int TotalPlayers;
    int Survivors;
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
    public void oneCharacterDied(bool isPlayerCharacter)
    {
        if (!isPlayerCharacter)
        {
            TotalEnemies--;
            Debug.Log("Remaining Enemies are" + TotalEnemies);
        }
        else
        {
            Survivors--;
            Debug.Log("Remaining Survivors are" + Survivors);
        }
        CheckWinCondidion();
    }
    void CheckWinCondidion()
    {
        if (TotalEnemies == 0)
        {
            TriggerNextDialogAction -= TriggerCharacterDialogs;
            string gameWinDialog = "Event of Game Win";
            if (TotalPlayers == Survivors)
            {
                gameWinDialog = "We took significant risks today, but by severing our challenges one by one, we emerged victorious and lived to tell the tale.";
            }
            else if (Survivors == 1)
            {
                gameWinDialog = "I stand here alone, the sole survivor. This fate is a haunting reminder of the lives lost, and the weight of their absence lingers.";
            }
            else
            {
                gameWinDialog = "We survived, but at what cost? The scars we carry will forever remind us of the sacrifices made and the battles fought.";

            }
            Action winGameDialog = delegate
            {
                textBox.text = gameWinDialog;
            };
            TriggerNextDialogAction += winGameDialog;

            //Debug.Break();
        }
    }
    public void addCharacter(bool isPlayerCharacter)
    {
        if (!isPlayerCharacter)
            TotalEnemies++;
        else
        {
            TotalPlayers++;
            Survivors++;
        }
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
    public void reloadScene()
    {
        // Get the index of the current scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // Reload the current scene by loading its index
        SceneManager.LoadScene(currentSceneIndex);
    }

}
