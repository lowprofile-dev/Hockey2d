using System;
using UnityEngine;

[Serializable]
public class DiscoState
{
    private TransformState prevTransform;
    public TransformState Transform;

    public float Speed;
    public Vector2 Heading;

    public bool IsHeadingTowardGoal;
    public bool IsInGoal;

    public DiscoState()
    {
        this.Transform = new TransformState();
        this.Reset();
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

    public void Reset()
    {
        this.Transform.Reset();
        this.StartNextFrame();

        this.Speed = 0f;
        this.Heading = Vector2.up;

        this.IsHeadingTowardGoal = false;
        this.IsInGoal = false;
    }
}
