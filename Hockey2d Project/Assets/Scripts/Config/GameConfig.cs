using System;
using UnityEngine;

// Main Configuration

[Serializable]
public class GameConfig
{
    [Serializable]
    public class GameSizeConfig
    {
        [SerializeField, Range(0f, 5f)]
        private float goalWidth = 2f;

        [SerializeField, Range(0f, 5f)]
        private float zoneOffset = 2.8f;

        public float GoalWidth { get { return this.goalWidth; } }
        public float ZoneOffset { get { return this.zoneOffset; } }
    }

    [SerializeField]
    private GameSizeConfig nonTutorialSize;

    [SerializeField]
    private GameSizeConfig tutorialSize;

    [SerializeField]
    private Box bolaBox;

    [SerializeField]
    private BolaConfig bola;

    [SerializeField]
    private Box discoBox;

    [SerializeField]
    private ObjectoConfig disco;

    [SerializeField]
    private ColourConfig colours;

    public GameSizeConfig Size { get { return GameConfig.InTutorialMode ? this.tutorialSize : this.nonTutorialSize; } }
    public Box BolaBox { get { return this.bolaBox; } }
    public BolaConfig Bola { get { return this.bola; } }
    public Box DiscoBox { get { return this.discoBox; } }
    public ObjectoConfig Disco { get { return this.disco; } }
    public ColourConfig Colours { get { return this.colours; } }

    public static BolaPlayerController.ControlScheme ControlScheme
    {
        get { return (BolaPlayerController.ControlScheme)PlayerPrefs.GetInt("Options.ControlScheme", (int)BolaPlayerController.ControlScheme.FollowDisc); }
        set
        {
            PlayerPrefs.SetInt("Options.ControlScheme", (int)value);
            PlayerPrefs.Save();

            Debug.Log("Control scheme changed to " + value);
        }
    }

    public static bool InTutorialMode
    {
        get { return PlayerPrefs.GetInt("Options.InTutorialMode", 0) == 1; }
        set
        {
            PlayerPrefs.SetInt("Options.InTutorialMode", value ? 1 : 0);
            PlayerPrefs.Save();

            Debug.Log("Modo Tutorial modificado " + value);
        }
    }
}
