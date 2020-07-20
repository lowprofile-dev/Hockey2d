using System;
using UnityEngine;

[Serializable]
public class ColourConfig
{
    [Serializable]
    public struct ColourSet
    {
        public Color BackgroundColour;
        public Color ForegroundColour;
    }

    [SerializeField]
    private ColourSet[] sets;

    private static int nextIndex;

    public ColourSet CurrentSet { get; private set; }

    public void GenerateColourSet()
    {
        this.CurrentSet = this.sets[nextIndex];

        nextIndex++;
        if (nextIndex >= this.sets.Length)
        {
            nextIndex = 0;
        }
    }
}
