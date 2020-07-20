using UnityEngine;

public class BolaView : MonoBehaviour
{
    [SerializeField]
    private Transform rootTrans;

    [SerializeField]
    private Transform[] rotationTrans;

    [SerializeField]
    private GameObject nonTutorialParent;

    [SerializeField]
    private GameObject tutorialParent;

    [SerializeField]
    private TrailRenderer[] trails;

    [SerializeField]
    private SpriteRenderer[] sprites;

    [SerializeField]
    private float directionVisualUpdateSpeedFast = 1400f;

    [SerializeField]
    private float directionVisualUpdateSpeedSlow = 1000f;

    private int playerId;

    private float currRot;
    private float targetRot;

    private void Awake()
    {
        Debug.Assert(this.rootTrans, this);
        Debug.Assert(this.rotationTrans.Length > 0, this);
        Debug.Assert(this.nonTutorialParent, this);
        Debug.Assert(this.tutorialParent, this);
        Debug.Assert(this.trails.Length > 0, this);
        Debug.Assert(this.sprites.Length > 0, this);
    }

    private void Update()
    {
        if (Mathf.Approximately(this.currRot, this.targetRot))
        {
            return;
        }

        var baseSpeed = GameConfig.InTutorialMode ? this.directionVisualUpdateSpeedSlow : this.directionVisualUpdateSpeedFast;

        this.currRot += baseSpeed * Time.deltaTime * Mathf.Sign(this.targetRot - this.currRot);
        this.currRot = Mathf.Clamp(this.currRot, 0, 90);

        foreach (var t in this.rotationTrans)
        {
            t.localEulerAngles = Vector3.forward * this.currRot;
        }
    }

    public void Initialize(int playerId, GameContext context)
    {
        this.playerId = playerId;

        this.nonTutorialParent.SetActive(!GameConfig.InTutorialMode);
        this.tutorialParent.SetActive(GameConfig.InTutorialMode);

        this.UpdateTransform(0f, context);
    }

    public void UpdateTransform(float time, GameContext context)
    {
        var state = context.State.GetBola(this.playerId);

        var trans = state.CalculateTransformAtTime(time, context);

        this.rootTrans.localPosition = trans.Position;
        this.rootTrans.localEulerAngles = Vector3.forward * trans.Rotation;

        this.UpdateDirection(context);
    }

    public void UpdateColours(GameContext context)
    {
        var set = context.Config.Colours.CurrentSet;

        foreach (var t in this.trails)
        {
            t.material.color = set.ForegroundColour;
        }

        foreach (var s in this.sprites)
        {
            s.material.color = set.ForegroundColour;
        }

        this.UpdateDirection(context);
    }

    private void UpdateDirection(GameContext context)
    {
        var state = context.State.GetBola(this.playerId);

        this.targetRot = state.Direction == Direction.CW ? 0 : 90;
    }
}
