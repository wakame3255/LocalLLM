
using R3;

/// <summary>
/// ƒQ[ƒ€ó‘Ô‚É‰‚¶‚ÄUI‚ğ§Œä‚·‚éƒvƒŒƒ[ƒ“ƒ^[
/// </summary>
public class GameStatePresenter
{
    private GameStateGuardian _gameStateModel;

    private MainMenuView _mainMenuView;
    private PauseView _pauseView;

    public GameStatePresenter(GameStateGuardian gameStateModel, MainMenuView mainMenuView, PauseView pauseView)
    {
        _gameStateModel = gameStateModel;
        _mainMenuView = mainMenuView;
        _pauseView = pauseView;
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