
using R3;

public class GameStateJudge
{
    private IGameStateGuardian _gameStateGuardian;

    /// <summary>
    /// ゲームの状態を保持するReactiveProperty
    /// </summary>
    private ReactiveProperty<GameState> _gameState = new ReactiveProperty<GameState>();
    public ReadOnlyReactiveProperty<GameState> CurrentGameState => _gameState;

    public GameStateJudge(IGameStateGuardian gameStateGuardian)
    {
        _gameStateGuardian = gameStateGuardian;

        //ゲームの状態の購読
        gameStateGuardian.CurrentGameState
            .Subscribe(JudgeGameOutcome);
    }

    public void JudgeGameOutcome(GameState gameState)
    {
        _gameState.Value = gameState;
    }

    public void NoticeResult(string message)
    {
        if (message.Contains("助かる"))
        {
            DebugUtility.Log("クリア");
            _gameStateGuardian.ChangeGameState(GameState.Clear);
        }
        else if (message.Contains("助からない"))
        {
            DebugUtility.Log("ゲームオーバー");
            _gameStateGuardian.ChangeGameState(GameState.GameOver);
        }
    }
}