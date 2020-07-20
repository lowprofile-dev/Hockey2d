public class GameContext
{
    public readonly GameConfig Config;
    public readonly GameState State;

    public GameContext(GameConfig config, GameState state)
    {
        this.Config = config;
        this.State = state;
    }
}
