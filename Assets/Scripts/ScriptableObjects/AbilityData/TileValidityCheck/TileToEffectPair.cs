using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New TileEffectPair", menuName = "TileEffectPair")]
public class TileToEffectPair : ScriptableObject
{
    public ValidTargets validTargersForThesePoints;
    public AoeType areaOfEffectType;
    public ActionEffectParams effectAppliedOnTiles;
}
