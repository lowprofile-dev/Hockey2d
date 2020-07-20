using UnityEngine;

public struct Line
{
    public readonly Vector2 Start;
    public readonly Vector2 End;
    public readonly Vector2 Mid;
    public readonly Vector2 Normal;

    public Line(Vector2 start, Vector2 end)
    {
        this.Start = start;
        this.End = end;

        var v = (end - start).normalized;
        this.Mid = start + v * ((end - start).magnitude / 2);
        this.Normal = new Vector2(-v.y, v.x);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode() * this.Start.GetHashCode() * this.End.GetHashCode() * this.Normal.GetHashCode();
    }

    public override bool Equals(object obj) 
    {
        if (!(obj is Line))
            return false;

        var l = (Line)obj;

        return l.Start == this.Start && l.End == this.End && l.Normal == this.Normal;
    }
}