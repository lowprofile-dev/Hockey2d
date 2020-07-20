using System;
using UnityEngine;

[Serializable]
public class BolaState
{
    public enum MovementMode
    {
        Normal = 0,
        Orbiting,
    }

    public readonly int PlayerId;

    private TransformState prevTransform;
    public TransformState Transform;

    public float Speed;
    public bool IsTapping;
    public MovementMode Mode;
    public Direction Direction;

    public float PercentOrbit;
    public Vector2 OrbitOrigin;

    public BolaState(int playerId)
    {
        this.PlayerId = playerId;
        this.Transform = new TransformState();
        this.StartNextFrame();
    }

    public void StartNextFrame()
    {
        this.prevTransform = this.Transform.Clone();
    }

    public TransformState CalculateTransformAtTime(float time, GameContext context)
    {
        var timeDiff = context.State.Time - context.State.PrevTime;

        if (Mathf.Approximately(timeDiff, 0f)) 
        {
            return this.Transform.Clone();
        }
        
        var t = (time - context.State.PrevTime) / timeDiff;
        return TransformState.Lerp(this.prevTransform, this.Transform, t);
    }
}
