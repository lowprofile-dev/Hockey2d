using UnityEngine;

public static class ObjectoPhysics
{
    private const float NearZero = 0.001f;

    public static void ResolveTarget(GameContext context, TransformTarget target)
    {
        var state = context.State.Disco;

        while (target.Distance > NearZero)
        {
            MoveToTarget(context, target);
        }

        state.Speed = Mathf.Min(state.Speed, context.Config.Disco.MaxSpeed);
    }

    private static void MoveToTarget(GameContext context, TransformTarget target)
    {
        var state = context.State.Disco;

        if (state.IsInGoal)
        {
            target.Distance = 0f;
            return;
        }

        var trans = state.Transform;

        var nextPos = trans.Position + target.Heading * target.Distance;

        Line collisionWall = default;
        var collisionPoint = Vector2.zero;

        var collided = false;

        if (state.IsHeadingTowardGoal)
        {
            if (context.Config.DiscoBox.Bounds.Contains(nextPos))
            {
                state.IsHeadingTowardGoal = false;
            }
        }

        // TODO:Mudar a colisao para o ObjectoController.
        if (!state.IsHeadingTowardGoal)
        {
            collided = BoxPhysics.CheckPointCollision(context.Config.DiscoBox, trans.Position, nextPos, out collisionWall, out collisionPoint);
        }

        if (collided && !state.IsHeadingTowardGoal)
        {
            var halfGoalSize = context.Config.Size.GoalWidth / 2 - context.Config.Disco.Size.Radius; // HACK
            var inGoalX = collisionPoint.x > -halfGoalSize && collisionPoint.x < halfGoalSize;

            if (inGoalX)
            {
                collided = false;
                state.IsHeadingTowardGoal = true;
            }
        }

        if (collided)
        {
            // Calculate partial position offset
            var collisionPosAmount = Mathf.Max(0f, Vector2.Distance(trans.Position, collisionPoint) - NearZero);
            var collisionPosTarget = trans.Position + target.Heading * collisionPosAmount;

            // Apply partial position offset
            target.Distance -= collisionPosAmount;
            trans.Position = collisionPosTarget;

            // Reflect heading against wall
            target.Heading = Vector2.Reflect(target.Heading, collisionWall.Normal).normalized;
            state.Heading = target.Heading;

            state.Speed *= context.Config.Disco.WallBounceFactor;
        }
        else
        {
            target.Distance = 0f;
            target.AngleOffset = 0f;
            trans.Position = nextPos;
        }

        if (state.IsHeadingTowardGoal)
        {
            var goalLineY = context.Config.DiscoBox.Bounds.max.y + context.Config.Disco.Size.Radius * 2;
            if (Mathf.Abs(trans.Position.y) > goalLineY)
            {
                state.IsInGoal = true;
            }
        }
    }
}
