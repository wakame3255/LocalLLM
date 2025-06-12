
using R3;

/// <summary>
/// ゲーム状態に応じてUIを制御するプレゼンター
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
        //状態購読
        _gameStateModel.CurrentGameState.Subscribe(JugeGameState);

        //メインメニュー
        _mainMenuView.StartButton.OnClickAsObservable()
            .Subscribe(_ => TransitionInGame());
        _mainMenuView.ExitButton.OnClickAsObservable()
            .Subscribe(_ => TransitionExit());

        //ゲームオーバー
        _gameOverView.MainMenuButton.OnClickAsObservable()
            .Subscribe(_ => TransitionMenu());              

        //クリア
        _clearView.MainMenuButton.OnClickAsObservable()
            .Subscribe(_ => TransitionMenu());
    }

    /// <summary>
    /// ゲーム状態に応じてのUI処理
    /// </summary>
    /// <param name="gameState">ゲーム状態</param>
    private void JugeGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Menu:
                _mainMenuView.ChangeActiveMenu(true);
                _gameOverView.ChangeActiveMenu(false);
                _clearView.ChangeActiveMenu(false);
                break;
            case GameState.InGame:
                _mainMenuView.ChangeActiveMenu(false);
                break;
            case GameState.Clear:
                _clearView.ChangeActiveMenu(true);
                break;
            case GameState.GameOver:
                _gameOverView.ChangeActiveMenu(true);
                break;
        }
    }
    public void TransitionMenu()
    {
        _gameStateModel.ChangeGameState(GameState.Menu);
    }

    public void TransitionInGame()
    {
        _gameStateModel.ChangeGameState(GameState.InGame);
    }

    public void TransitionExit()
    {
        _gameStateModel.DoGameExit();
    }
}