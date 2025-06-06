
using R3;

/// <summary>
/// �Q�[����Ԃɉ�����UI�𐧌䂷��v���[���^�[
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
        //��ԍw��
        _gameStateModel.CurrentGameState.Subscribe(JugeGameState);
    }

    /// <summary>
    /// �Q�[����Ԃɉ����Ă�UI����
    /// </summary>
    /// <param name="gameState">�Q�[�����</param>
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