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

        _llmModel.FirstJuge();
    }
}