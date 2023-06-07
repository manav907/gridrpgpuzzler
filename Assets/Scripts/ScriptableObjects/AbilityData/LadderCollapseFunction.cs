using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New CharacterAbility")]
public class LadderCollapseFunction : ScriptableObject
{
    public string Name;
    public SerializableDictionary<LadderCollapseFunctionEnums, int> invokeFunction;
    public SerializableDictionary<ActionInputParams, string> SetDataAtIndex;
    public SerializableDictionary<TypeOfAction, string> DoActionFromDataAtIndex;
    public LadderCollapseFunction()
    {

    }
    public LadderCollapseFunction(LadderCollapseFunction given)
    {
        this.name = given.name;
        //Rework This
        invokeFunction = (given.invokeFunction);
        SetDataAtIndex = (given.SetDataAtIndex);
        DoActionFromDataAtIndex = (given.DoActionFromDataAtIndex);
    }
}
