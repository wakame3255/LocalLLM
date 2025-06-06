
using R3;

public enum GameState
{
    Menu,
    InGame,
    Paused,
    GameOver,
    Clear
}

public interface IGameStateGuardian
{
    public ReadOnlyReactiveProperty<GameState> CurrentGameState { get; }
    public void ChangeGameState(GameState newState);
}

public class GameStateGuardian : IGameStateGuardian
{
    private ReactiveProperty<GameState> _currentGameState = new();
    public ReadOnlyReactiveProperty<GameState> CurrentGameState => _currentGameState;

    public GameStateGuardian()
    {
        // 初期状態を設定
        _currentGameState.Value = GameState.Menu;
    }

    /// <summary>
    /// ゲーム状態を変更するメソッド
    /// </summary>
    /// <param name="newState">変更したいステート</param>
    public void ChangeGameState(GameState newState)
    {
        if (_currentGameState.Value != newState)
        {
            _currentGameState.Value = newState;
        }
    }
}