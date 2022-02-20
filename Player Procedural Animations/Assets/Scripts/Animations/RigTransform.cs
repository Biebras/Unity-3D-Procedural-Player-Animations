using UnityEngine;

[System.Serializable]
public class RigTransform
{
    public Vector3 Position;
    public Vector3 Rotation;

    public RigTransform(Vector3 Position, Vector3 Rotation)
    {
        this.Position = Position;
        this.Rotation = Rotation;
    }
}
