using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIHandlers : BehaviourSingleton<UIHandlers>
{
    [SerializeField]
    private GameObject menuParent;

    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private Text controlSchemeText;

    [SerializeField]
    private Text inTutorialModeText;

    private void Awake()
    {
        Debug.Assert(this.menuParent, this);
        Debug.Assert(this.scoreText, this);
        Debug.Assert(this.controlSchemeText, this);
        Debug.Assert(this.inTutorialModeText, this);

        instance = this;
    }

    private void Start()
    {
        this.menuParent.SetActive(false);

        this.UpdateDisplay();
    }

    public void UpdateScore(int team0Score, int team1Score)
    {
        this.scoreText.text = string.Format("{0} - {1}", team0Score, team1Score);
    }

    private void UpdateDisplay()
    {
        this.controlSchemeText.text = string.Format("INPUT={0}", (int)GameConfig.ControlScheme);
        this.inTutorialModeText.text = string.Format("TUTORIAL={0}", GameConfig.InTutorialMode ? "1" : "0");
    }

    private void ResetGame()
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene(0);
    }

    public void UGUI_ToggleMenuButtonPress()
    {
        this.menuParent.SetActive(!this.menuParent.activeInHierarchy);
    }

    public void UGUI_OnResetButtonPress()
    {
        this.ResetGame();
    }

    public void UGUI_ToggleControlSchemeButtonPress()
    {
        var scheme = GameConfig.ControlScheme;

        var nextScheme = (int)scheme + 1;
        if (nextScheme >= (int)BolaPlayerController.ControlScheme.Count)
        {
            nextScheme = 0;
        }

        GameConfig.ControlScheme = (BolaPlayerController.ControlScheme)nextScheme;
        this.UpdateDisplay();
    }


    public void UGUI_ToggleInTutorialModeButtonPress()
    {
        GameConfig.InTutorialMode = !GameConfig.InTutorialMode;
        this.ResetGame();
    }
}
