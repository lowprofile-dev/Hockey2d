using UnityEngine;

public static class BolaPhysics
{
    private const float NearZero = 0.001f;

    public static void ResolveTarget(int playerId, GameContext context, TransformTarget target)
    {
        var state = context.State.GetBola(playerId);

        while (target.Distance > NearZero)
        {
            MoveToTarget(playerId, context, target);
            CheckDiscCollision(state, target, context);
        }
    }

    private static void MoveToTarget(int playerId, GameContext context, TransformTarget target)
    {
        var state = context.State.GetBola(playerId);
        var trans = state.Transform;

        var nextPos = trans.Position + target.Heading * target.Distance;
        var nextRot = trans.Rotation + target.AngleOffset;

        Line collisionWall;
        Vector2 collisionPoint;

        var collided = BoxPhysics.CheckPointCollision(context.Config.BolaBox, trans.Position, nextPos, out collisionWall, out collisionPoint);
        if (collided)
        {
            // Calculate partial position offset
            var collisionPosAmount = Mathf.Max(0f, Vector2.Distance(trans.Position, collisionPoint) - NearZero);
            var collisionPosTarget = trans.Position + target.Heading * collisionPosAmount;

            // Apply partial position offset
            target.Distance -= collisionPosAmount;
            trans.Position = collisionPosTarget;

            // Calculate partial rotation offset
            var bounceDistPercentage = collisionPosAmount / target.Distance;
            var bounceRotAmount = bounceDistPercentage * target.AngleOffset;

            // Reflect rotation against wall + apply partial rotation offset
            target.Heading = Vector2.Reflect(nextPos - trans.Position, collisionWall.Normal).Rotate(bounceRotAmount).normalized;
            target.AngleOffset -= bounceRotAmount;
            trans.Rotation = Vector2.up.SignedAngle(target.Heading);
        }
        else
        {
            target.Distance = 0f;
            target.AngleOffset = 0f;
            trans.Position = nextPos;
            trans.Rotation = nextRot;
        }
    }

    // TODO: Criar fisica do Disco
    /// <remarks>Source: http://gamedevelopment.tutsplus.com/tutorials/when-worlds-collide-simulating-circle-circle-collisions--gamedev-769</remarks>
    private static void CheckDiscCollision(BolaState state, TransformTarget target, GameContext context)
    {
        // Circle-circle collision
        var discRadius = context.Config.Disco.Size.Radius;
        var paddleRadius = context.Config.Bola.Size.Radius;
        var discPos = context.State.Disco.Transform.Position;
        var bolaPos = state.Transform.Position;

        var discVelocity = context.State.Disco.Heading * context.State.Disco.Speed;
        var paddleVelocity = target.Heading * context.Config.Bola.Size.ForwardSpeed;
        var discMass = context.Config.Disco.Size.Mass;
        var bolaMass = context.Config.Bola.Size.Mass;

        var dist = Vector2.Distance(discPos, bolaPos);
        var minDist = discRadius + paddleRadius;

        if (dist < minDist)
        {
            var collisionPoint = Vector2.zero;
            collisionPoint.x = ((discPos.x * paddleRadius) + (bolaPos.x * discRadius)) / (discRadius + paddleRadius);
            collisionPoint.y = ((discPos.y * paddleRadius) + (bolaPos.y * discRadius)) / (discRadius + paddleRadius);

            var discNormal = (collisionPoint - discPos).normalized;
            var bounceHeading = -Vector2.Reflect(paddleVelocity, discNormal).normalized;

            var newDiscVelocity = Vector2.zero;
            newDiscVelocity.x = (discVelocity.x * (discMass - bolaMass) + (2 * bolaMass * paddleVelocity.x)) / (discMass + bolaMass);
            newDiscVelocity.y = (discVelocity.y * (discMass - bolaMass) + (2 * bolaMass * paddleVelocity.y)) / (discMass + bolaMass);

            context.State.Disco.Heading = (newDiscVelocity + bounceHeading).normalized;
            context.State.Disco.Speed = Mathf.Max(context.State.Disco.Speed, newDiscVelocity.magnitude * context.Config.Bola.DiscBounceFactor);
        }
    }
}
