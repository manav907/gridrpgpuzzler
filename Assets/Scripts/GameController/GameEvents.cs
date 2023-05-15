using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameEvents : MonoBehaviour
{
    [Header("Reffrences")]
    [SerializeField] GameObject scriptManager;
    TurnManager turnManager;
    DialogSetManger dialogSetManger;
    public static GameEvents current;
    [Header("Dialog Stuff")]
    [SerializeField] TMPro.TextMeshProUGUI textBox;
    [SerializeField] Image imagePortraitReffrence;
    [SerializeField] Sprite[] images;
    [SerializeField] string[] listOFDialog;
    [Header("Game State")]
    int TotalEnemies = 0;
    int TotalPlayers;
    int Survivors;
    int turnLoop;
    CharacterName currentCharacterName;

    [Header("SFX")]
    public AudioClip[] sfx;
    public void PlaySound(int i)
    {
        //GameObject soundGameObject = new GameObject("Sound");
        //AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(sfx[i]);
    }
    private void Awake()
    {
        current = this;
        onCharacterDeath += delegate { Debug.Log("Even dEath Called"); };
        TriggerNextDialogAction += TriggerCharacterDialogs;
        turnManager = scriptManager.GetComponent<TurnManager>();
        dialogSetManger = GetComponent<DialogSetManger>();

    }
    public event Action onCharacterDeath;
    public void CheckDath()
    {
        if (onCharacterDeath != null)
        {
            onCharacterDeath();
        }
    }
    bool char1 = true;
    bool char2 = true;
    bool char3 = true;
    string generateTreeIndex()
    {

        string output = "";

        if (char1)
            output += "1";
        else
            output += "0";

        if (char2)
            output += "1";
        else
            output += "0";

        if (char3)
            output += "1";
        else
            output += "0";
        return output;
    }
    public void DeathEvent(CharacterControllerScript characterControllerScript)
    {

        switch (characterControllerScript.CharacterDataSO.NameEnum)
        {

            case CharacterName.GreenRedDude:
                {
                    char1 = false;
                    DialotTreeInt = 1;
                    break; // It's important to include the break statement to exit the switch statement
                }
            case CharacterName.PinkHairGuy:
                {
                    char2 = false;
                    DialotTreeInt = 1;
                    break; // It's important to include the break statement to exit the switch statement
                }
            case CharacterName.RedPriestessGirl:
                {
                    char3 = false;
                    DialotTreeInt = 1;
                    break; // It's important to include the break statement to exit the switch statement
                }
            default:
                {
                    DialotTreeInt = 0;
                    break;
                }
        }
        if (!characterControllerScript.isPlayerCharacter)
        {
            TotalEnemies--;
            //Debug.Log("Remaining Enemies are" + TotalEnemies);
        }
        else
        {
            Survivors--;
            //Debug.Log("Remaining Survivors are" + Survivors);
        }
        CheckWinCondidion();
    }
    void CheckWinCondidion()
    {
        string gameWinDialog = "Event of Game Win";
        void winGameDialog()
        {
            textBox.text = gameWinDialog;

            //Setting Portrait transparency to 0
            Color spriteColor = imagePortraitReffrence.color;
            spriteColor.a = 0f; // Set alpha channel to 0 (fully transparent)
            imagePortraitReffrence.color = spriteColor;
        }
        if (TotalEnemies == 0)
        {
            TriggerNextDialogAction -= TriggerCharacterDialogs;

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

            TriggerNextDialogAction += winGameDialog;
        }
        else if (Survivors == 0)
        {
            TriggerNextDialogAction -= TriggerCharacterDialogs;
            gameWinDialog = "Game Over";
            TriggerNextDialogAction += winGameDialog;
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
        turnLoop = turnManager.TurnLoop;
        currentCharacterName = turnManager.thisCharacter.GetComponent<CharacterControllerScript>().CharacterDataSO.NameEnum;
        if (TriggerNextDialogAction != null)
            TriggerNextDialogAction();
    }
    public int DialotTreeInt = 0;
    void TriggerCharacterDialogs()
    {
        Debug.Log("Current Tree" + DialotTreeInt + " Chanracter Name" + currentCharacterName.ToString() + " Turn Loop " + turnLoop);
        dialogSetManger.setBranch(generateTreeIndex());
        string newDialog;
        newDialog = dialogSetManger.DialogTree[DialotTreeInt].getCharacterDialog(currentCharacterName.ToString(), turnLoop);
        Sprite newSprite;
        newSprite = dialogSetManger.DialogTree[DialotTreeInt].getCharacterSprite(currentCharacterName.ToString(), turnLoop);
        Debug.Log(newDialog);

        if (newDialog == null)
        {
            setDialogToNull();

        }
        imagePortraitReffrence.sprite = newSprite;
        textBox.text = newDialog;
        void setDialogToNull()
        {
            Color spriteColor = imagePortraitReffrence.color;
            spriteColor.a = 0f; // Set alpha channel to 0 (fully transparent)
            imagePortraitReffrence.color = spriteColor;



            textBox.text = "";
            return;
        }
    }

    public void reloadScene()
    {
        // Get the index of the current scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Reload the current scene by loading its index
        SceneManager.LoadScene(currentSceneIndex);
    }

}
