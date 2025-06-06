
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
        // ������Ԃ�ݒ�
        _currentGameState.Value = GameState.Menu;
    }

    /// <summary>
    /// �Q�[����Ԃ�ύX���郁�\�b�h
    /// </summary>
    /// <param name="newState">�ύX�������X�e�[�g</param>
    public void ChangeGameState(GameState newState)
    {
        if (_currentGameState.Value != newState)
        {
            _currentGameState.Value = newState;
        }
    }
}