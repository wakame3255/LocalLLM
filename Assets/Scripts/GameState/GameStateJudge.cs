
using R3;

public class GameStateJudge
{
    private IGameStateGuardian _gameStateGuardian;

    /// <summary>
    /// �Q�[���̏�Ԃ�ێ�����ReactiveProperty
    /// </summary>
    private ReactiveProperty<GameState> _gameState = new ReactiveProperty<GameState>();
    public ReadOnlyReactiveProperty<GameState> CurrentGameState => _gameState;

    public GameStateJudge(IGameStateGuardian gameStateGuardian)
    {
        _gameStateGuardian = gameStateGuardian;

        //�Q�[���̏�Ԃ̍w��
        gameStateGuardian.CurrentGameState
            .Subscribe(JudgeGameOutcome);
    }

    public void JudgeGameOutcome(GameState gameState)
    {
        _gameState.Value = gameState;
    }

    public void NoticeResult(string message)
    {
        if (message.Contains("������"))
        {
            DebugUtility.Log("�N���A");
            _gameStateGuardian.ChangeGameState(GameState.Clear);
        }
        else if (message.Contains("������Ȃ�"))
        {
            DebugUtility.Log("�Q�[���I�[�o�[");
            _gameStateGuardian.ChangeGameState(GameState.GameOver);
        }
    }
}