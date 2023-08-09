using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameUI : MonoBehaviour
{
    [Header("EditorSettings")]
    public bool enableEditorQuickMode = true;
    [Header("Other")]
    public Button restartButton;
    public Button exitButton;
    public Button ControlScheme;
    public Button SkipAnimation;
    public Button NextButton;
    public Button AblityButtonExample;
    public VisualElement TipBox;
    public Label Tip;
    public VisualElement AbilityButtonSideBar;
    public VisualElement DialogBoxBar;


    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        restartButton = root.Q<Button>("restartButton");
        exitButton = root.Q<Button>("exitButton");
        NextButton = root.Q<Button>("NextButton");
        AblityButtonExample = root.Q<Button>("ExampleButton");
        AbilityButtonSideBar = root.Q<VisualElement>("SideButtons");
        DialogBoxBar = root.Q<VisualElement>("DialogBox");
        Tip = root.Q<Label>("Tip");
        TipBox = root.Q<VisualElement>("TipBox");
        ControlScheme = root.Q<Button>("ControlScheme");
        SkipAnimation = root.Q<Button>("SkipAnimation");
        SetUpUI();


        restartButton.clicked += GameEvents.current.reloadScene;
        exitButton.clicked += GameEvents.current.returnToLevelSelect;
        ControlScheme.clicked += SwithControlScheme;
        SkipAnimation.clicked += SwithSkipAnimation;

        UserDataManager.SetSetting();

        SwithControlScheme();
        SwithSkipAnimation();
        SwithControlScheme();
        SwithSkipAnimation();


        //Debug.Log("Start was called");
    }
    void SwithSkipAnimation()
    {
        if (UserDataManager.skipAnimations == false)
        {
            UserDataManager.skipAnimations = true;
            SkipAnimation.text = "Fast Animations";
        }
        else if (UserDataManager.skipAnimations == true)
        {
            UserDataManager.skipAnimations = false;
            SkipAnimation.text = "Normal Animations";
        }
    }
    void SwithControlScheme()
    {
        if (UserDataManager.Snap == false)
        {
            UserDataManager.Snap = true;
            ControlScheme.text = "Snap Mode";
        }
        else if (UserDataManager.Snap == true)
        {
            UserDataManager.Snap = false;
            ControlScheme.text = "Pick Mode";
        }
    }
    void SetUpUI()
    {
        ControlScheme.text = "Pick Mode";
#if UNITY_EDITOR
        if (GameEvents.current.inGameUI.enableEditorQuickMode)
        {
            ControlScheme.text = "Editor Snap Mode";
        }
#endif

        var root = GetComponent<UIDocument>().rootVisualElement;
        Tip = root.Q<Label>("Tip");
        TipBox = root.Q<VisualElement>("TipBox");
        Tip.text = "Kill All Enemies";
    }
    public void SetTip(string tip)
    {
        Tip.text = tip;
    }
    public void MakeButtonsFromAbilityies(List<AbilityData> list)
    {
        ClearButtons();
        for (int i = 0; i < list.Count; i++)
        {
            AbilityData abilityData = list[i];
            void newAction() { GameEvents.current.moveDictionaryManager.DoAction(abilityData); }

            if (i >= actionsAssigned.Count)
            { actionsAssigned.Add(newAction); }
            else
                actionsAssigned[i] = newAction;
            string buttonName = abilityData.userFriendlyName;
            if (buttonName == "")
                buttonName = abilityData.name;
            AddButton(buttonName, actionsAssigned[i]);
        }
        if (endTurn == null)
        {
            endTurn = delegate { GameEvents.current.turnManager.EndTurn(); };
        }
        AbilityButtonSideBar.Add(InstansiateButton("End Turn", endTurn));
    }
    Action endTurn;
    List<Action> actionsAssigned;
    public void ClearButtons()
    {
        AbilityButtonSideBar.Clear();
        if (actionsAssigned == null)
        {
            actionsAssigned = new List<Action>();
        }
    }
    public void AddButton(string nameOFButton, Action action)
    {
        AbilityButtonSideBar.Add(InstansiateButton(nameOFButton, action));
    }
    public Button InstansiateButton(string nameOFButton, Action action)
    {
        var newButton = new Button
        {
            name = nameOFButton,
            text = nameOFButton
        };
        newButton.style.height = AblityButtonExample.style.height;

        newButton.AddToClassList("ButtonsThatShrinkToFit");
        //newButton.style.fontSize = AblityButtonExample.style.fontSize;
        newButton.clicked += action;

        return newButton;
        //newButton.style
    }
}
