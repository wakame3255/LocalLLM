
using R3;

public enum GameState
{
    Menu,
    InGame,
    Paused,
    GameOver
}

public class GameStateGuardian
{
    private ReactiveProperty<GameState> _currentGameState = new();
    public ReadOnlyReactiveProperty<GameState> CurrentGameState => _currentGameState;

    public GameStateGuardian()
    {
        // ‰Šúó‘Ô‚ğİ’è
        _currentGameState.Value = GameState.Menu;
    }

    public void ChangeGameState(GameState newState)
    {
        if (_currentGameState.Value != newState)
        {
            _currentGameState.Value = newState;
        }
    }
}