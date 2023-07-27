using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New AbilityData", menuName = "AbilityData")]
public class AbilityData : ScriptableObject
{
    public TypeOfAction Primaryuse;
    public List<TileToEffectPair> ValidTileData;
    public List<ActionEffectParams> ApplyEffects;
}
