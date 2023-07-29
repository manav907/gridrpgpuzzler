using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TileValidityParms", menuName = "TileValidityParms")]
public class TileValidityParms : ScriptableObject
{
    public List<GroundFloorType> validFloors;
    public ValidTargets ShowCastOn;
    public TargetType targetType;
}