
public class GameStateJudge
{
    private IGameStateGuardian _gameStateGuardian;
    // LLM‚Ì‰“šƒ‚ƒfƒ‹

    public GameStateJudge(IGameStateGuardian gameStateGuardian)
    {
        _gameStateGuardian = gameStateGuardian;
    }

    public void JudgeGameOutcome()
    {

    }

    public void NoticeResult(string message)
    {
        if (message.Contains("•‚©‚é"))
        {
            DebugUtility.Log("ƒNƒŠƒA");
            _gameStateGuardian.ChangeGameState(GameState.Clear);
        }       
    }
}