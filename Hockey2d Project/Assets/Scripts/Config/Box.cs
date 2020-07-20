using System;
using System.Collections.Generic;
using UnityEngine;
//Game Level
[Serializable]
public class Box
{
    public enum WallType
    {
        Left = 0,
        Top = 1,
        Right = 2,
        Bottom = 3,
    }

    private List<Line> walls = new List<Line>();

    public Bounds Bounds { get; private set; }
    public IEnumerable<Line> Walls { get { return this.walls; } }


    // Built-in Functions
    public void Initialize(Vector2 size, float inset)
    {
        var totalInset = inset * 2;
        var tableSize = new Vector2(size.x - totalInset, size.y - totalInset);
        this.Bounds = new Bounds(Vector3.zero, tableSize);

        var min = this.Bounds.min;
        var max = this.Bounds.max;

        //Collision Walls
        this.walls.Clear();
        this.walls.Add(new Line(new Vector2(min.x, max.y), new Vector2(min.x, min.y))); // Parede Esquerda Index
        this.walls.Add(new Line(new Vector2(max.x, max.y), new Vector2(min.x, max.y))); // Parede Top Index
        this.walls.Add(new Line(new Vector2(max.x, min.y), new Vector2(max.x, max.y))); // Parede Direita Index
        this.walls.Add(new Line(new Vector2(min.x, min.y), new Vector2(max.x, min.y))); // Parede Bottom Index
    }



    // Custom Functions
    public Line GetWall(WallType type)
    {
        return this.walls[(int)type];
    }

    public Line GetNextWall(Line wall, Direction direction)
    {
        var index = this.walls.IndexOf(wall);
        index += direction == Direction.CW ? 1 : -1;

        if (index < 0)
        {
            index = this.walls.Count - 1;
        }
        else if (index > this.walls.Count - 1)
        {
            index = 0;
        }

        return this.walls[index];
    }
}
