using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameUI : MonoBehaviour
{
    public Button restartButton;
    public Button exitButton;
    public Toggle QuickSnap;
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
        QuickSnap = root.Q<Toggle>("QuickSnap");
        setUpUI();


        restartButton.clicked += GameEvents.current.reloadScene;
        exitButton.clicked += GameEvents.current.returnToLevelSelect;

        UserDataManager.Snap = false;
        QuickSnap.RegisterValueChangedCallback(toogleQuickSnap);
        //backButtons.clicked += backButtonPressed;

        //initilizeArcadeModeGrid();
    }
    void toogleQuickSnap(ChangeEvent<bool> s)
    {
        UserDataManager.Snap = s.newValue;
    }
    void setUpUI()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        Tip = root.Q<Label>("Tip");
        TipBox = root.Q<VisualElement>("TipBox");
        //Debug.Log(TipBox.style.backgroundSize + " " + TipBox.style.backgroundRepeat);
        TipBox.style.backgroundRepeat = new BackgroundRepeat(Repeat.Repeat, Repeat.Repeat);
        TipBox.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
        Tip.text = "Kill All Enemies";
    }
    public Sprite sprite;
    public void setTip(string tip)
    {

        //setUpUI();
        //Debug.Log(TipBox.style.backgroundSize + " " + TipBox.style.backgroundRepeat);
        Tip.text = tip;
    }
    public void MakeButtonsFromLadderCollapseFunction(List<LadderCollapseFunction> list)
    {
        ClearButtons();
        for (int i = 0; i < list.Count; i++)
        {
            LadderCollapseFunction ladderCollapseFunction = list[i];
            Action newAction = delegate { GameEvents.current.moveDictionaryManager.doAction(ladderCollapseFunction); };

            if (i >= actionsAssigned.Count)
            { actionsAssigned.Add(newAction); }
            else
                actionsAssigned[i] = newAction;
            addButton(ladderCollapseFunction.Name, actionsAssigned[i]);
        }
        if (endTurn == null)
        {
            endTurn = delegate { GameEvents.current.turnManager.endTurn(); };
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
    public void addButton(string nameOFButton, Action action)
    {
        AbilityButtonSideBar.Add(InstansiateButton(nameOFButton, action));
    }
    public Button InstansiateButton(string nameOFButton, Action action)
    {
        var newButton = new Button();
        newButton.name = nameOFButton;
        newButton.text = nameOFButton;
        newButton.style.height = AblityButtonExample.style.height;
        //newButton.style.fontSize = AblityButtonExample.style.fontSize;
        newButton.clicked += action;

        return newButton;
        //newButton.style
    }
}
