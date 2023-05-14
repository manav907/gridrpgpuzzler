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
    int imageLoop = 0;
    [SerializeField] string[] listOFDialog;
    [Header("Game State")]
    int currentDialog = 0;
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
    public void DeathEvent(CharacterControllerScript characterControllerScript)
    {

        switch (characterControllerScript.CharacterDataSO.NameEnum)
        {
            case CharacterName.PinkHairGuy:
                {
                    // Code to execute when the character's name is PinkHairGuy
                    // Add your logic here
                    break; // It's important to include the break statement to exit the switch statement
                }
            // Add additional cases for other character names if needed
            // case CharacterName.AnotherCharacterName:
            // {
            //     // Code to execute when the character's name is AnotherCharacterName
            //     break;
            // }
            default:
                {
                    // Code to execute when none of the cases match
                    break;
                }
        }
        if (characterControllerScript.CharacterDataSO.NameEnum == CharacterName.PinkHairGuy)
        {
            DialotTreeInt = 1;
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
        string newDialog;
        newDialog = dialogSetManger.DialogTree[DialotTreeInt].getCharacterDialog(currentCharacterName.ToString(), turnLoop);
        Debug.Log(newDialog);

        if (newDialog == null)
        {
            setDialogToNull();

        }
        if (imageLoop == images.Length)
            imageLoop = 0;
        imagePortraitReffrence.sprite = images[imageLoop];
        //textBox.text = listOFDialog[currentDialog];
        textBox.text = newDialog;
        currentDialog++;
        imageLoop++;
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
