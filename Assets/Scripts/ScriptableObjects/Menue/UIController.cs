using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public Button storyModeButon;
    public Button arcadeModeButton;
    public Button settingButtons;
    public Button backButtons;
    public Button difficulty;
    public VisualElement Modes;
    public VisualElement ButtonGrid;
    public List<LevelDataSO> arcadeModeLevels;

    public int GridXLimit;
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        storyModeButon = root.Q<Button>("StoryMode");
        arcadeModeButton = root.Q<Button>("ArcadeMode");
        difficulty = root.Q<Button>("Difficulty");
        settingButtons = root.Q<Button>("Settings");
        backButtons = root.Q<Button>("Back");
        Modes = root.Q<VisualElement>("Modes");
        ButtonGrid = root.Q<VisualElement>("ButtonGird");



        difficulty.clicked += DifficultyToggled;
        storyModeButon.clicked += storyModeButonPressed;
        arcadeModeButton.clicked += arcadeModeButtonPressed;
        backButtons.clicked += backButtonPressed;
        UpdateDiffiucltyText();

        initilizeArcadeModeGrid();
    }
    void DifficultyToggled()
    {
        UserDataManager.SmartPosistioning = !UserDataManager.SmartPosistioning;
        UpdateDiffiucltyText();
    }
    void UpdateDiffiucltyText()
    {
        if (UserDataManager.SmartPosistioning)
        {
            difficulty.text = "Smart AI";
        }
        else if (!UserDataManager.SmartPosistioning)
        {
            difficulty.text = "Basic AI";
        }
    }
    void storyModeButonPressed()
    {
        Debug.Log("stotf");
    }
    void arcadeModeButtonPressed()
    {
        Modes.style.display = DisplayStyle.None;
        ButtonGrid.style.display = DisplayStyle.Flex;
    }
    void disableVestagialButtons()
    {
        Debug.Log("disableVestagialButtons disabled");
        backButtons.style.display = DisplayStyle.None;
        settingButtons.style.display = DisplayStyle.None;
    }
    void initilizeArcadeModeGrid()
    {
        disableVestagialButtons();

        ButtonGrid.style.flexDirection = FlexDirection.Column;

        int numberOfButtons = arcadeModeLevels.Count;
        int rowFillLimit = GridXLimit;
        int currentButtonID = 0;

        while (currentButtonID < numberOfButtons)
        {
            var rowContainer = new VisualElement();
            rowContainer.style.flexDirection = FlexDirection.Row;
            ButtonGrid.Add(rowContainer);

            for (int CellFilled = 0; CellFilled < GridXLimit; CellFilled++)
            {
                //Debug.Log(currentButtonID);
                if (currentButtonID >= numberOfButtons)
                    break;

                var button = new Button();
                //Getting Data
                LevelDataSO currentData = arcadeModeLevels[currentButtonID];

                //Assigning Names
                string nameOFButton = currentData.name;
                button.text = nameOFButton;
                button.AddToClassList("ButtonsThatShrinkToFit");
                //assigning Function
                button.clicked += LoadThisScene;

                // Add the button to the row container
                rowContainer.Add(button);
                currentButtonID++;


                void LoadThisScene()
                {
                    UserDataManager.currentLevel = currentData;//setting current Level in UDM
                    SceneManager.LoadScene("Level");
                }
            }
        }
    }
    void backButtonPressed()
    {

        Modes.style.display = DisplayStyle.Flex;
        ButtonGrid.style.display = DisplayStyle.None;
    }
}
;