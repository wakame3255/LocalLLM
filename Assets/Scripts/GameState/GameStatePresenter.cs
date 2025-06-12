
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

        //���C�����j���[
        _mainMenuView.StartButton.OnClickAsObservable()
            .Subscribe(_ => TransitionInGame());
        _mainMenuView.ExitButton.OnClickAsObservable()
            .Subscribe(_ => TransitionExit());

        //�Q�[���I�[�o�[
        _gameOverView.MainMenuButton.OnClickAsObservable()
            .Subscribe(_ => TransitionMenu());              

        //�N���A
        _clearView.MainMenuButton.OnClickAsObservable()
            .Subscribe(_ => TransitionMenu());
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