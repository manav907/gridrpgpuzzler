using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[CreateAssetMenu(fileName = "New AbilityData", menuName = "AbilityData")]
public class AbilityData : ScriptableObject
{
    public string userFriendlyName;
    public TypeOfAction Primaryuse;
    public SerializableDictionary<CostType, int> primaryCost;
    public AreaGenerationParams rangeOfAbility;
    public List<AreaGenerationParams> ValidTileData;
    public List<ActionEffectParams> ApplyEffects;
    public bool CheckAbilityBudget(CharacterControllerScript characterControllerScript, bool consumePoints = false)
    {
        Debug.Log(characterControllerScript.characterName + " can use " + this.name);
        foreach (var keypair in primaryCost.returnKeyPairList())
        {
            if (!canAffotd(keypair.value, keypair.key))
                return false;
        }
        return true;
        bool canAffotd(int cost, CostType costType)
        {
            Debug.Log(costType + cost);
            switch (costType)
            {
                case CostType.Stamina:
                    {
                        if (characterControllerScript.currentStamina <= cost)
                        {
                            if (consumePoints)
                                characterControllerScript.currentStamina = characterControllerScript.currentStamina - cost;
                            return true;
                        }
                        break;
                    }
                case CostType.FocusPoints:
                    {
                        if (characterControllerScript.currentFocusPoints <= cost)
                        {
                            if (consumePoints)
                                characterControllerScript.currentFocusPoints = characterControllerScript.currentFocusPoints - cost;
                            return true;
                        }
                        break;
                    }
                default:
                    Debug.Log("CaseFailed");
                    break;

            }
            return false;
        }
    }
}
public enum CostType
{
    Stamina,//Refilled Each turn
    FocusPoints,//Earned Each Turn
    /* Flow,//Lost Each turn
    Mana,//Needs Manual Refillling
    Health,//Costs Health
    Consentraion,//Earned Each Kill\
    Zen,//Earned on Specific Actions */

}
[Serializable]
public class AreaGenerationParams
{
    public string name;
    public int minpoints = 1;
    //public int MaxPoints = 1;
    [Range(-3, 3)]
    public int adjustAtPointWithDirection = 1;
    [Range(1, 3)]
    public int rangeOfArea = 1;
    public TileValidityParms tileValidityParms;
    public AoeStyle aoeStyle;
}
public enum TargetType
{
    AnyValid,
    FirstValid,
    LastValid,
}
public enum ValidTargets
{
    AnyValidOrInValid = 0,
    Empty = 1,
    AnyFaction = 2,
    Enemies = 3,
    Allies = 4,
    Neutral,
}
public enum AoeStyle
{

    Taxi = 4,
    Square = 5,
    SSweep = 2,
}