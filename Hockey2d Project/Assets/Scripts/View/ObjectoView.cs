using UnityEngine;

public class ObjectoView : MonoBehaviour
{
    [SerializeField]
    private Transform rootTrans;

    [SerializeField]
    private GameObject nonTutorialParent;

    [SerializeField]
    private GameObject tutorialParent;

    [SerializeField]
    private TrailRenderer[] trails;

    private void Awake()
    {
        Debug.Assert(this.rootTrans, this);
        Debug.Assert(this.nonTutorialParent, this);
        Debug.Assert(this.tutorialParent, this);
        Debug.Assert(this.trails.Length > 0, this);
    }

    public void Initialize(GameContext context)
    {
        this.nonTutorialParent.SetActive(!GameConfig.InTutorialMode);
        this.tutorialParent.SetActive(GameConfig.InTutorialMode);

        this.UpdateTransform(0f, context);
    }

    public void EnableTrail(bool enable)
    {
        foreach (var t in this.trails)
        {
            if (enable)
            {
                t.Clear();
            }

            t.enabled = enable;
        }
    }

    public void UpdateTransform(float time, GameContext context)
    {
        var state = context.State.Disco;

        var trans = state.CalculateTransformAtTime(time, context);

        this.rootTrans.localPosition = trans.Position;
        this.rootTrans.localEulerAngles = Vector3.forward * trans.Rotation;
    }
}
