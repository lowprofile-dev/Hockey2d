using UnityEngine;

public static class MathUtility
{
    /// <remarks>Basiado em: http://www.wyrmtale.com/blog/2013/115/2d-line-intersection-in-c</remarks>
    public static bool LineSegmentIntersection(Vector2 s1Start, Vector2 s1End, Vector2 s2Start, Vector2 s2End, out Vector2 intersectionPoint)
    {
        intersectionPoint = Vector2.zero;
        
        // Get A,B,C of first line from points
        var a1 = s1End.y - s1Start.y;
        var b1 = s1Start.x - s1End.x;
        var c1 = a1*s1Start.x + b1*s1Start.y;

        // Get A,B,C of second line from points
        var a2 = s2End.y - s2Start.y;
        var b2 = s2Start.x - s2End.x;
        var c2 = a2*s2Start.x + b2*s2Start.y;

        var delta = a1*b2 - a2*b1;

        // Lines are parallel
        if (Mathf.Approximately(delta, 0f))
        {
            return false;
        }

        intersectionPoint.x = (b2*c1 - b1*c2) / delta;
        intersectionPoint.y = (a1*c2 - a2*c1) / delta;
        return true;
    }

    /// <remarks>Basiado em: http://forum.unity3d.com/threads/moving-an-object-on-a-specific-arc.387757/</remarks>
    public static float CircleArcDistanceToAngleOffset(float distance, float circleRadius)
    {
        return (distance / circleRadius) * Mathf.Rad2Deg;
    }

    /// <remarks>Basiado em: http://forum.unity3d.com/threads/how-do-i-find-the-closest-point-on-a-line.340058/</remarks>
    public static Vector2 NearestPointOnFiniteLine(Vector2 start, Vector2 end, Vector2 point)
    {
        var line = (end - start);
        var len = line.magnitude;
        line.Normalize();

        var v = point - start;
        var d = Vector3.Dot(v, line);
        d = Mathf.Clamp(d, 0f, len);
        return start + line * d;
    }

    /// <remarks>Basiado em: http://answers.unity3d.com/questions/823090/equivalent-of-degree-to-vector2-in-unity.html#answer-823216</remarks>
    public static Vector2 ToHeading(float degrees)
    {
        return Quaternion.AngleAxis(degrees, Vector3.forward) * Vector2.up;
    }
}
