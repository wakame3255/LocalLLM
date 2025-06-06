
using R3;

/// <summary>
/// ƒQ[ƒ€ó‘Ô‚É‰‚¶‚ÄUI‚ğ§Œä‚·‚éƒvƒŒƒ[ƒ“ƒ^[
/// </summary>
public class GameStatePresenter
{
    private GameStateGuardian _gameStateModel;

    private MainMenuView _mainMenuView;
    private GameOverView _gameOverView;
    private ClearView _clearView;

    public GameStatePresenter(GameStateGuardian gameStateModel, GameViewData gameViewData)
    {
        _gameStateModel = gameStateModel;
        _mainMenuView = gameViewData.MainMenuView;
        _gameOverView = gameViewData.GameOverView;
        _clearView = gameViewData.ClearView;
        Initialize();
    }

    private void Initialize()
    {
        //ó‘Ôw“Ç
        _gameStateModel.CurrentGameState.Subscribe(JugeGameState);
    }

    /// <summary>
    /// ƒQ[ƒ€ó‘Ô‚É‰‚¶‚Ä‚ÌUIˆ—
    /// </summary>
    /// <param name="gameState">ƒQ[ƒ€ó‘Ô</param>
    private void JugeGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Menu:
                _mainMenuView.ChangeActiveMenu(true);
                break;
            case GameState.InGame:
                _mainMenuView.ChangeActiveMenu(false);
                break;
        }
    }
}