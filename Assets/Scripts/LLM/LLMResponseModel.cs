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

	private const string HEAD_WORD = "--現在の質問--";

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
            string formattedQuestion = talkHistory + "\n" + HEAD_WORD + "\n" + question;

			DebugUtility.Log(formattedQuestion);

            //LLMの実行
            string resultText = await Task.Run(() => LLMPython(formattedQuestion));

            //会話のログを記録
            _talkDataMemory.AddTalkData(question, resultText);

            //返答をセット
            _response.Value = resultText;

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
		string systemContent = "あなたは「危機脱出ゲーム」のゲームマスターです。以下のルールに従って進行してください：\r\n\r\n【役割】\r\n- 危機的状況の出題者\r\n- プレイヤーの解決策に対する判定者\r\n\r\n【出題ルール】\r\n1. ランダムで現実的な危機的状況を1つ提示する\r\n2. 状況は具体的で分かりやすく、100文字程度で説明する\r\n3. 生死に関わる緊急性のある場面を設定する\r\n\r\n【判定ルール】\r\n1. プレイヤーの解決策を「助かる」「助からない」の2段階で評価\r\n2. 解決策を交えた物語を簡潔に説明する（50文字程度）\r\n3. 現実的な観点から合理的に判断する\r\n4. 創意工夫や機転の利いた解決策は高く評価する\r\n\r\n【出力形式】\r\n危機的状況の提示：\r\n「状況：[具体的な危機状況]」\r\n\r\n解決策への判定：\r\n「結果：[助かる/助からない]」\r\n「理由：[解決策を交えた判定理由]」\r\n\r\nでは、最初の危機的状況を提示してください。";
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
