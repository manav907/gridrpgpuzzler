using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New TileEffectPair", menuName = "TileEffectPair")]
public class TileToEffectPair : ScriptableObject
{
    public int minpoints = 1;
    public int MaxPoints = 1;
    public bool sortFromPoint = true;
    public ValidTargets TileRequirements;
    public RangeOfActionEnum areaOfEffectRange;
    public AoeStyle areaOfEffectStyle;
}
