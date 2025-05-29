
using R3;

/// <summary>
/// �Q�[����Ԃɉ�����UI�𐧌䂷��v���[���^�[
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