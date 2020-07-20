using UnityEngine;

public static class BoxPhysics
{
    public static bool CheckPointCollision(Box box, Vector2 currPoint, Vector2 targetPoint, out Line collisionWall, out Vector2 collisionPoint)
    {
        // Wall Collider Space
        collisionWall = default(Line);
        collisionPoint = default(Vector2);

        if (box.Bounds.Contains(targetPoint))
        {
            return false;
        }

        // FIXME: Necessita uma melhor maneira para escolher a parede colidida, pode ser incorrecto se o objecto for para fora das paredes.
        // TODO: Isto poderia ser resolvido se mudando os Layouts das paredes.
        if (targetPoint.x < box.Bounds.min.x)
        {
            collisionWall = box.GetWall(Box.WallType.Left);
        }
        else if (targetPoint.x > box.Bounds.max.x)
        {
            collisionWall = box.GetWall(Box.WallType.Right);
        }
        else if (targetPoint.y < box.Bounds.min.y)
        {
            collisionWall = box.GetWall(Box.WallType.Bottom);
        }
        else if (targetPoint.y > box.Bounds.max.y)
        {
            collisionWall = box.GetWall(Box.WallType.Top);
        }
        else
        {
            throw new System.Exception();
        }

        MathUtility.LineSegmentIntersection(currPoint, targetPoint, collisionWall.Start, collisionWall.End, out collisionPoint);
        return true;
    }

    public static bool IsInside(Box config, Vector2 point)
    {
        return config.Bounds.SqrDistance(point) <= Mathf.Epsilon;
    }
}
