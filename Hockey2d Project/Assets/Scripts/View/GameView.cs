using System.Collections.Generic;
using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField]
    private Camera gameCamera;

    [SerializeField]
    private Transform rootTrans;

    [SerializeField]
    private ObjectoView discoPrefab;

    [SerializeField]
    private BolaView BolaPrefab;

    private List<BolaView> bolas = new List<BolaView>();

    public ObjectoView Disco { get; private set; }

    private void Awake()
    {
        Debug.Assert(this.gameCamera, this);
        Debug.Assert(this.rootTrans, this);
        Debug.Assert(this.discoPrefab, this);
        Debug.Assert(this.BolaPrefab, this);
    }

    public void Initialize(GameContext context)
    {
        this.Disco = GameObjectUtility.InstantiatePrefab(this.discoPrefab, this.rootTrans);
        this.Disco.Initialize(context);

        for (var i = 0; i < context.Config.Bola.NumBolas; i++)
        {
            var view = GameObjectUtility.InstantiatePrefab(this.BolaPrefab, this.rootTrans);
            view.Initialize(i, context);
            this.bolas.Add(view);
        }

        this.UpdateColours(context);
    }

    public void UpdateTransforms(float time, GameContext context)
    {
        for (var i = 0; i < this.bolas.Count; i++)
        {
            this.bolas[i].UpdateTransform(time, context);
        }

        this.Disco.UpdateTransform(time, context);
    }

    public void UpdateColours(GameContext context)
    {
        var set = context.Config.Colours.CurrentSet;

        this.gameCamera.backgroundColor = set.BackgroundColour;

        foreach (var view in this.bolas)
        {
            view.UpdateColours(context);
        }
    }
}
