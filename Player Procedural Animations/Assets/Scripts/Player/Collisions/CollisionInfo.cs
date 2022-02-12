using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionInfo
{
    [Min(0)]
    public float radius;
    public Vector3 offset;
    public Color gizmosColor;

    public Vector3 GetPos(Vector3 pos)
    {
        return pos + offset;
    }
}
