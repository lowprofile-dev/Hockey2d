using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameState
{
    [SerializeField]
    private DiscoState disco;

    [SerializeField]
    private BolaState[] bolas;

    public float PrevTime;
    public float Time;

    public DiscoState Disco { get { return this.disco; } }
    public IEnumerable<BolaState> Bolas { get { return this.bolas; } }

    private GameState() { }

    public GameState(GameConfig config)
    {
        this.disco = new DiscoState();

        this.bolas = new BolaState[config.Bola.NumBolas];
        for (var i = 0; i < this.bolas.Length; i++)
        {
            this.bolas[i] = new BolaState(i);
        }
    }

    public BolaState GetBola(int playerId)
    {
        Debug.Assert(playerId >= 0 && playerId < this.bolas.Length);

        return this.bolas[playerId];
    }
}
