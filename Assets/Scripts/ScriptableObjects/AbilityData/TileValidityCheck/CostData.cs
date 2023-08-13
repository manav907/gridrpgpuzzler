using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CostData", menuName = "CostData")]
public class CostData : ScriptableObject
{
    public SerializableDictionary<CostType, int> primaryCost;
    public string GetCostTip()
    {
        string abilityCost = " Requires ";
        foreach (var cost in primaryCost.returnDict())
        {
            abilityCost += cost.Value + " " + cost.Key + " ";
        }
        return abilityCost;
    }
    public bool CheckAbilityBudget(CharacterControllerScript characterControllerScript, bool consumePoints = false)
    {
        foreach (var keypair in primaryCost.returnKeyPairList())
        {
            if (!canAffotd(keypair.value, keypair.key))
            {
                return false;
            }
        }
        return true;
        bool canAffotd(int cost, CostType costType)
        {
            switch (costType)
            {
                case CostType.Stamina:
                    {
                        if (cost <= characterControllerScript.currentStamina)
                        {
                            if (consumePoints)
                                characterControllerScript.currentStamina = characterControllerScript.currentStamina - cost;
                            return true;
                        }
                        break;
                    }
                case CostType.FocusPoints:
                    {
                        if (cost <= characterControllerScript.currentFocusPoints)
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