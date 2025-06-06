
public class GameStateJudge
{
    private IGameStateGuardian _gameStateGuardian;
    // LLM�̉������f��

    public GameStateJudge(IGameStateGuardian gameStateGuardian)
    {
        _gameStateGuardian = gameStateGuardian;
    }

    public void JudgeGameOutcome()
    {

    }

    public void NoticeResult(string message)
    {
        if (message.Contains("������"))
        {
            DebugUtility.Log("�N���A");
            _gameStateGuardian.ChangeGameState(GameState.Clear);
        }       
    }
}