using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ExploreMethod : ScriptableObject
{
    public string userFriendlyName;
    public TypeOfAction Primaryuse;
    public ExploreParams primaryExploreParams;
}

public class ExploreParams
{
    public TypeOfAction effectOnPos;
    public List<GroundFloorType> CollideOnTerrain;
    public ValidTargets CollideWithTarrget;
    public SerializableDictionary<CollisionState, ExploreParams> ssss;
}
public enum CollisionState
{
    Collided,
    Phased,
    Expired
}