using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameUI : MonoBehaviour
{
    [Header("Other")]
    public Button restartButton;
    public Button exitButton;
    public Button ControlScheme;
    public Button SkipAnimation;
    public Button NextButton;
    public Button AblityButtonExample;
    public VisualElement TipBox;
    public Label Tip;
    public Label EnergyInfo;
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
        EnergyInfo = root.Q<Label>("EnergyInfo");
        TipBox = root.Q<VisualElement>("TipBox");
        ControlScheme = root.Q<Button>("ControlScheme");
        SkipAnimation = root.Q<Button>("SkipAnimation");
        SetUpUI();

        restartButton.clicked += GameEvents.current.reloadScene;
        exitButton.clicked += GameEvents.current.returnToLevelSelect;

        ControlScheme.clicked += delegate { UserDataManager.Snap = !UserDataManager.Snap; };
        ControlScheme.clicked += RefreshContolScheme;
        SkipAnimation.clicked += delegate { UserDataManager.AnimationSkipStateNum = (AnimationSkipState)GetNextEnum(UserDataManager.AnimationSkipStateNum); };
        SkipAnimation.clicked += RefreshSkipAnimationButton;
#if UNITY_EDITOR
        UserDataManager.AnimationSkipStateNum = AnimationSkipState.Debug;
        UserDataManager.Snap = true;
#endif
        RefreshSkipAnimationButton();
        RefreshContolScheme();
    }

    Enum GetNextEnum(Enum currentEnum)
    {
        var arrayOfEnum = Enum.GetValues(currentEnum.GetType());
        int index = Array.IndexOf(arrayOfEnum, currentEnum);
        index++;
        if (index >= arrayOfEnum.Length)
            index = 0;
        return (Enum)arrayOfEnum.GetValue(index);
    }
    void RefreshSkipAnimationButton()
    {
        switch (UserDataManager.AnimationSkipStateNum)
        {
            case AnimationSkipState.Normal:
                {
                    UserDataManager.skipWaitTime = false;
                    UserDataManager.skipAnimations = false;
                    break;
                }
            case AnimationSkipState.Fast:
                {
                    UserDataManager.skipWaitTime = true;
                    UserDataManager.skipAnimations = false;
                    break;
                }
            case AnimationSkipState.Debug:
                {
                    UserDataManager.skipWaitTime = true;
                    UserDataManager.skipAnimations = true;
                    break;
                }
        }
        SkipAnimation.text = UserDataManager.AnimationSkipStateNum.ToString() + " Speed";
    }
    void RefreshContolScheme()
    {
        if (UserDataManager.Snap == false)
            ControlScheme.text = "Pick Mode";
        else if (UserDataManager.Snap == true)
            ControlScheme.text = "Snap Mode";
    }
    void SetUpUI()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        Tip = root.Q<Label>("Tip");
        TipBox = root.Q<VisualElement>("TipBox");
        Tip.text = "Kill All Enemies";
    }
    public void SetEnergyInfo(string info)
    {
        EnergyInfo.text = info;
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
        newButton.tooltip = " we got it";
        newButton.clicked += action;
        return newButton;
        //newButton.style
    }
}
