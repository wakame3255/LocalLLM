using R3;
using R3.Triggers;
using System.Diagnostics;
using UnityEngine;

public class LLMTextPresenter
{
    private LLMTextView _textView;
    private LLMResponseModel _llmModel;

    public LLMTextPresenter(LLMTextView textView, LLMResponseModel llmModel)
    {
        _textView = textView;
        _llmModel = llmModel;

        DebugUtility.Log("LLMTextPresenter initialized.");

        Bind();
    }

    private void Bind()
    {
        // LLMボタンで質問を実行
        _textView.RunButton.OnClickAsObservable()
             .Subscribe(_ => _llmModel.RunLLM(_textView.InputText.text));

        // LLMの返答を表示
        _llmModel.Response.Subscribe(aw => _textView.SetText(aw));

        // LLMの状態を購読して、ビューの表示/非表示を切り替える
        _llmModel.CurrentGameState.Subscribe(NoticeViewActive);

        //LLM送信許可通知
        _llmModel.AcceptLLMCall.Subscribe(_textView.AcceptLLMButton);
    }

    private void NoticeViewActive(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.InGame:
                _textView.ChangeInGame();
                break;
            case GameState.Clear:
                _textView.ChangeResult();
                break;
            case GameState.GameOver:
                _textView.ChangeResult();
                break;
            case GameState.Menu:
                _textView.ChangeMainMenu();
                break;
        }
    }
}