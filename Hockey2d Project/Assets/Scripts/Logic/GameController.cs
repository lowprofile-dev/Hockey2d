using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameConfig config;

    [SerializeField]
    private GameView view;

    [SerializeField]
    private RectTransform canvasRect;

    [SerializeField]
    private Transform goalTransform;

    [SerializeField]
    private LineRenderer[] zoneLines;

    [SerializeField]
    private int controllingPlayerId = 0;

    [SerializeField]
    private TransformState initialBolaTransform;

    [SerializeField]
    private float timestep = 1 / 60f;

    private GameState state;
    private GameContext context;

    private int[] scores;

    private void Start()
    {
        Debug.Assert(this.view, this);
        Debug.Assert(this.canvasRect, this);
        Debug.Assert(this.goalTransform, this);
        Debug.Assert(this.zoneLines.Length == 2, this);

        // Config
        var tableSize = this.CalculateTableSize();
        this.config.BolaBox.Initialize(tableSize, config.Bola.Size.Radius);
        this.config.DiscoBox.Initialize(tableSize, config.Disco.Size.Radius);
        this.config.Colours.GenerateColourSet();

        this.goalTransform.localScale = new Vector3(this.config.Size.GoalWidth, 1f, 1f);
        this.scores = new int[2];
        UIHandlers.Instance.UpdateScore(this.scores[0], this.scores[1]);

        const float ZoneLinePosX = 3;
        this.zoneLines[0].SetPosition(0, new Vector3(-ZoneLinePosX, -this.config.Size.ZoneOffset, 1f));
        this.zoneLines[0].SetPosition(1, new Vector3(ZoneLinePosX, -this.config.Size.ZoneOffset, 1f));
        this.zoneLines[1].SetPosition(0, new Vector3(-ZoneLinePosX, this.config.Size.ZoneOffset, 1f));
        this.zoneLines[1].SetPosition(1, new Vector3(ZoneLinePosX, this.config.Size.ZoneOffset, 1f));

        // State
        this.state = new GameState(this.config);
        this.state.GetBola(0).Transform = this.initialBolaTransform.Clone();

        // Context
        this.context = new GameContext(this.config, this.state);

        // View
        this.view.Initialize(this.context);
        this.view.UpdateColours(this.context);

        Time.timeScale = 1f;
    }

    private void Update()
    {
        BolaPlayerController.UpdateInput(this.controllingPlayerId, this.context);

        var time = Time.timeSinceLevelLoad;

        while (this.context.State.Time < time)
        {
            this.context.State.PrevTime = this.context.State.Time;
            this.context.State.Time += this.timestep;

            foreach (var p in this.context.State.Bolas)
            {
                p.StartNextFrame();
                var bolaTarget = BolaPlayerController.StepTarget(this.timestep, p.PlayerId, this.context);
                BolaPhysics.ResolveTarget(p.PlayerId, this.context, bolaTarget);
            }

            this.context.State.Disco.StartNextFrame();
            var discTarget = ObjectoController.StepTarget(this.timestep, this.context);
            ObjectoPhysics.ResolveTarget(this.context, discTarget);

            this.CheckGoalScored();
        }

        this.view.UpdateTransforms(time, context);
    }

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            return;
        }
        #endif

        foreach (var w in this.context.Config.BolaBox.Walls)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(w.Start, w.End);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(w.Mid, w.Mid + w.Normal * 0.5f);
        }

        foreach (var p in this.context.State.Bolas)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(p.Transform.Position, this.context.Config.Bola.Size.Radius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(p.OrbitOrigin, this.context.Config.Bola.Size.OrbitRadius);
        }

        foreach (var w in this.context.Config.BolaBox.Walls)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(w.Start, w.End);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(w.Mid, w.Mid + w.Normal * 0.5f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.context.State.Disco.Transform.Position, this.context.Config.Disco.Size.Radius);
    }

    private Vector2 CalculateTableSize()
    {
        var screenSize = this.canvasRect.sizeDelta;
        screenSize.x *= this.canvasRect.localScale.x;
        screenSize.y *= this.canvasRect.localScale.y;

        return screenSize;
    }

    private void CheckGoalScored()
    {
        var discoState = this.context.State.Disco;
        
        if (!discoState.IsInGoal)
        {
            return;
        }

        var teamScored = discoState.Transform.Position.y > 0 ? 0 : 1;
        this.scores[teamScored]++;

        Debug.Log("Team " + teamScored + " marcou !!!!, RESULTADO :  " + this.scores[0] + " - " + this.scores[1]);

        UIHandlers.Instance.UpdateScore(this.scores[0], this.scores[1]);

        this.view.Disco.EnableTrail(false);
        this.context.State.Disco.Reset();
        this.context.State.Disco.Transform.Position = Vector2.left * Random.Range(-2.2f, 2.2f);
        this.context.State.Disco.StartNextFrame();
        this.view.Disco.EnableTrail(true);
    }
}
