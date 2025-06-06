using Python.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using R3;

public class LLMResponseModel 
{
    //同時実行数制限用変数
    private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

	private CancellationToken _cancellationToken = new ();

    /// <summary>
    /// LLMの返答を保持するプロパティ
    /// </summary>
    private ReactiveProperty<string> _response = new();
	public ReadOnlyReactiveProperty<string> Response => _response;

    /// <summary>
    /// LLMの会話履歴を保持するクラス
    /// </summary>
    private TalkDataMemory _talkDataMemory = new TalkDataMemory();

	private GameStateJudge _stateJuge;

    private string _headWord = "";

	public LLMResponseModel(GameStateJudge stateJuge)
	{
        _stateJuge = stateJuge;
    }

	public void FirstJuge()
	{
		RunLLM("最初の危機的状況を提示してください。");
        _headWord = "--プレイヤーの解決策--";
    }

    /// <summary>
    /// LLMの実行を開始するメソッド
    /// </summary>
    /// <param name="question">プレイヤーからの質問</param>
    public async void RunLLM(string question)
	{
		//会話履歴を取得
		string talkHistory = _talkDataMemory.GetTalkData();

        IntPtr? state = null;
		try
		{
			//スレッドの実行を許可する（並行処理制限）
			await _semaphore.WaitAsync(_cancellationToken);

			//Pythonスレッドの開始
			state = PythonEngine.BeginAllowThreads();

            //質問のフォーマット生成
            string formattedQuestion = talkHistory + "\n" + _headWord + "\n" + question;

			DebugUtility.Log(formattedQuestion);

            //LLMの実行
            string resultText = await Task.Run(() => LLMPython(formattedQuestion));

            //会話のログを記録
            _talkDataMemory.AddTalkData(question, resultText);

			//返答をセット
			_response.Value = resultText;

			//ゲーム判定クラスに通知
			_stateJuge.NoticeResult(resultText);
            //デバッグログに出力
            DebugUtility.Log(resultText);
        }
		catch (OperationCanceledException e) when (e.CancellationToken == _cancellationToken)
		{
			Debug.Log("失敗");
            _response.Value = "失敗";

            throw;
		}
		finally
		{
			if (state.HasValue)
			{
				PythonEngine.EndAllowThreads(state.Value);
			}
			_semaphore.Release();
        }
	}

	private string LLMPython(string question)
	{
		//LLMモデルパス
		string modelPath =
			Application.streamingAssetsPath + "/GGUF/" + "/Phi-3-mini-128k-instruct.Q5_K_M.gguf";
		string systemContent = "あなたは危機脱出ゲームのAIです。\r\n\r\n【出題】\r\n危機的状況をランダムで1つ提示してください（100文字以内）\r\n\r\n【判定】\r\nプレイヤーの解決策を評価し、以下の形式で回答：\r\n結果：助かる/助からない\r\n理由：解決策を実行した結果と判定根拠（50文字以内）\r\n\r\n解決策の実行過程と結果を具体的に想像して判定してください。";
		Debug.Log(modelPath);

        using (Py.GIL())
		{
			//パイソンスクリプトをインポート
			using dynamic sample = Py.Import("sample");

            //Pythonスクリプトの関数を呼び出す
            using dynamic result = sample.llamaCppPython(modelPath, systemContent, question);
            return result; 
        }
	}
}
