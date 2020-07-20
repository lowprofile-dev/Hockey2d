using System;
using UnityEngine;

public static class BolaPlayerController
{
    public enum ControlScheme
    {
        FollowDisc = 0,
        DualTap,
        Count,
    }

    public static void UpdateInput(int playerId, GameContext context)
    {
        var state = context.State.GetBola(playerId);

        state.IsTapping = Input.anyKey;

        switch (GameConfig.ControlScheme)
        {
            case ControlScheme.FollowDisc:
            {   
                if (state.IsTapping)
                {
                    state.Mode = BolaState.MovementMode.Orbiting;
                }
                
                if (!state.IsTapping && state.Mode == BolaState.MovementMode.Orbiting)
                {
                    state.Mode = BolaState.MovementMode.Normal;
                }
            }
            break;

            case ControlScheme.DualTap:
            {
                if (state.IsTapping)
                {
                    #if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
                    var halfScreen = Screen.width / 2f;
                    var isLeftTapping = (Input.touchCount > 0 && Input.GetTouch(0).position.x < halfScreen) || (Input.touchCount > 1 && Input.GetTouch(1).position.x < halfScreen);
                    var isRightTapping = (Input.touchCount > 0 && Input.GetTouch(0).position.x >= halfScreen) || (Input.touchCount > 1 && Input.GetTouch(1).position.x >= halfScreen);
                    #else
                    var isLeftTapping = Input.GetMouseButton(0) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
                    var isRightTapping = Input.GetMouseButton(1) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
                    #endif

                    if (isLeftTapping)
                    {
                        state.Direction = Direction.CCW;
                        state.Mode = BolaState.MovementMode.Orbiting;
                    }
                    else if (isRightTapping)
                    {
                        state.Direction = Direction.CW;
                        state.Mode = BolaState.MovementMode.Orbiting;
                    }
                }

                if (!state.IsTapping && state.Mode == BolaState.MovementMode.Orbiting)
                {
                    state.Mode = BolaState.MovementMode.Normal;
                }
            }
            break;
        }
    }

    public static TransformTarget StepTarget(float deltaTime, int playerId, GameContext context)
    {
        var state = context.State.GetBola(playerId);
        var target = new TransformTarget();

        StepPercentOrbit(deltaTime, playerId, context);

        if (state.Mode == BolaState.MovementMode.Normal)
        {
            UpdateOrbitDirection(playerId, context);
        }

        UpdateOrbitOrigin(playerId, context);

        StepTransform(deltaTime, playerId, context, target);

        return target;
    }

    private static void StepPercentOrbit(float deltaTime, int playerId, GameContext context)
    {
        var state = context.State.GetBola(playerId);

        var mag = state.Mode == BolaState.MovementMode.Orbiting ? context.Config.Bola.ToOrbitDeltaSpeed : context.Config.Bola.FromOrbitDeltaSpeed;
        var delta = mag * deltaTime;
        state.PercentOrbit = Mathf.Clamp01(state.PercentOrbit + delta);
    }

    private static void UpdateOrbitDirection(int playerId, GameContext context)
    {
        if (GameConfig.ControlScheme == ControlScheme.FollowDisc)
        {
            var state = context.State.GetBola(playerId);
            var trans = state.Transform;

            var right = MathUtility.ToHeading(trans.Rotation - 90);
            var to = context.State.Disco.Transform.Position - trans.Position;
            var dot = Vector3.Dot(right, to);

            state.Direction = dot > 0f ? Direction.CW : Direction.CCW;
        }
    }

    private static void UpdateOrbitOrigin(int playerId, GameContext context)
    {
        var state = context.State.GetBola(playerId);
        var trans = state.Transform;

        var v = MathUtility.ToHeading(trans.Rotation);
        var offset = context.Config.Bola.Size.OrbitRadius;

        if (state.Direction == Direction.CW)
        {
            offset *= -1;
        }

        // Basiado em: http://answers.unity3d.com/questions/564166/how-to-find-perpendicular-line-in-2d.html
        state.OrbitOrigin = trans.Position + new Vector2(-v.y, v.x) / Mathf.Sqrt(Mathf.Pow(v.x, 2) + Mathf.Pow(v.y, 2)) * offset;
    }

    private static void StepTransform(float deltaTime, int playerId, GameContext context, TransformTarget target)
    {
        var state = context.State.GetBola(playerId);
        var trans = state.Transform;

        var posOffset = Vector2.zero;
        var angleOffset = 0f;

        // Adicionar orbita
        {
            var orbitDist = deltaTime * context.Config.Bola.Size.OrbitSpeed * state.PercentOrbit;
            var orbitDegrees = MathUtility.CircleArcDistanceToAngleOffset(orbitDist, context.Config.Bola.Size.OrbitRadius);

            if (state.Direction == Direction.CW)
            {
                orbitDegrees *= -1;
            }

            var orbitDest = trans.Position.RotateAround(state.OrbitOrigin, orbitDegrees);
            var orbitPosOffset = (orbitDest - trans.Position);

            posOffset += orbitPosOffset;
            angleOffset += orbitDegrees;
        }

        // Adicionar aceleraçao
        {
            var forwardDir = MathUtility.ToHeading(trans.Rotation);
            var forwardDist = deltaTime * context.Config.Bola.Size.ForwardSpeed * (1 - state.PercentOrbit);
            var forwardPosOffset = forwardDir * forwardDist;

            posOffset += forwardPosOffset;
        }

        target.Heading = posOffset.normalized;
        target.Distance = posOffset.magnitude;
        target.AngleOffset = angleOffset;
    }
}
