using System;
using UnityEngine;

[Serializable]
public class TransformState
{
    public Vector2 Position;

    [SerializeField]
    private float rotation;

    public float Rotation
    {
        get { return this.rotation; }
        set
        {
            this.rotation = value;

        }
    }

    public TransformState Clone()
    {
        return this.MemberwiseClone() as TransformState;
    }

    public static TransformState Lerp(TransformState a, TransformState b, float t)
    {
        var lerped = new TransformState();
        lerped.Position = Vector2.Lerp(a.Position, b.Position, t);
        lerped.Rotation = Mathf.Lerp(a.Rotation, b.Rotation, t);
        return lerped;
    }

    public void Reset()
    {
        this.Position = Vector2.zero;
        this.Rotation = 0f;
    }
}
