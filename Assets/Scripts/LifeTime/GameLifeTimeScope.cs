
using UnityEngine;
using VContainer;
using VContainer.Unity;
public class GameLifeTimeScope : MonoBehaviour
{
    [SerializeField]
    private LLMTextView _llmTextView;

    [SerializeField]
    private MainMenuView _mainMenuView;

    [SerializeField]
    private ClearView _clearView;

    [SerializeField]
    private GameOverView _gameOverView;

    private void Start()
    {
        //�Q�[����Ԃ̊Ǘ�
        GameStateGuardian gameStateGuardian = new GameStateGuardian();
        GameViewData gameViewData = new GameViewData(_mainMenuView, _clearView, _gameOverView);
        GameStatePresenter gameStatePresenter = new GameStatePresenter(gameStateGuardian, gameViewData);

        //�Q�[���R��
        GameStateJudge gameStateJuge = new GameStateJudge(gameStateGuardian);

        //LLM����
        LLMResponseModel llmResponseModel = new LLMResponseModel(gameStateJuge);
        LLMTextPresenter llmTextPresenter = new LLMTextPresenter(_llmTextView, llmResponseModel);
    }
}

public class GameViewData
{
    private MainMenuView _mainMenuView;
    private ClearView _clearView;
    private GameOverView _gameOverView;

    public MainMenuView MainMenuView => _mainMenuView;
    public ClearView ClearView => _clearView;
    public GameOverView GameOverView => _gameOverView;

    public GameViewData(MainMenuView mainMenuView, ClearView clearView, GameOverView gameOverView)
    {
        _mainMenuView = mainMenuView;
        _clearView = clearView;
        _gameOverView = gameOverView;
    }
}



