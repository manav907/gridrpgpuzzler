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
    public static GameEvents current;
    [Header("Dialog Stuff")]
    [SerializeField] TMPro.TextMeshProUGUI textBox;
    [SerializeField] Image imagePortraitReffrence;
    [SerializeField] Sprite[] images;
    [SerializeField] ChoiceBasedDialogManager DialogTree;
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
        MapManager mapManager = scriptManager.GetComponent<MapManager>();
        //
        DialogTree = new ChoiceBasedDialogManager(mapManager.LoadThisLevel.ChoiceToBranchMap);
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

        //Debug.Log("Death of " + characterControllerScript.CharacterDataSO.NameEnum);
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
    public bool EventInMotion;
    public void sendChoice(GameObject subjectCharacter, AbilityName abilityPerfomed, GameObject objectCharacter)
    {
        EventInMotion = true;
        Choice newChoice = new Choice(subjectCharacter.GetComponent<CharacterControllerScript>().CharacterDataSO.NameEnum, Choice.Actions.Observed, objectCharacter.GetComponent<CharacterControllerScript>().CharacterDataSO.NameEnum);
        if (abilityPerfomed != AbilityName.Move)
            newChoice.performedAction = Choice.Actions.Attacked;
        DeathCheckOnChoice();
        if (DialogTree.ChangeBranch(newChoice))
        {
            TriggerDialogEvent(DialogTree.currentBranch);
        }
        else
        {
            EventInMotion = false;
        }
        void DeathCheckOnChoice()
        {
            if (true == true)
            {
                newChoice.performedAction = Choice.Actions.Killed;
            }
        }
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
        //Debug.Log("Replacing This Stuff");

    }
    void TriggerDialogEvent(DialogEvent currentDialogEvent)
    {
        Dictionary<CharacterName, Dialog> DialogMap = currentDialogEvent.OrderOfDialogs.returnDict();
        foreach (var keyPair in DialogMap)
        {
            Debug.Log("Character" + keyPair.Key + " Said That " + keyPair.Value.dialogText);
        }

        EventInMotion = false;
        setDialog(null, "");
        void setDialog(Sprite newSprite, string newDialog)
        {
            if (newDialog == null)
            {
                setDialogToNull();
                return;
            }
            imagePortraitReffrence.sprite = newSprite;
            textBox.text = newDialog;
        }
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
