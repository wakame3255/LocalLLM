using Python.Runtime;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class LLMResponseModel
{
    //同時実行数制限用変数
    private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    private CancellationToken _cancellationToken = new();

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

    /// <summary>
    /// ゲームの状態を保持するReactiveProperty
    /// </summary>
    private ReactiveProperty<GameState> _gameState = new ReactiveProperty<GameState>();
    public ReadOnlyReactiveProperty<GameState> CurrentGameState => _gameState;

    /// <summary>
    /// LLMの呼び出しを許可するかどうかのReactiveProperty
    /// </summary>
    private ReactiveProperty<bool> _acceptLLMCall = new ReactiveProperty<bool>(false);
    public ReadOnlyReactiveProperty<bool> AcceptLLMCall => _acceptLLMCall;

    /// <summary>
    /// LLMの出題データを保持するリポジトリ
    /// </summary>
    private FaultDataRepository _faultDataRepository = new FaultDataRepository();

    private string _headWord = "--プレイヤーの解決策--";

    private string _waitText = "ローカルLLMの応答を待っています...";

    private string _mainPronpt;

    private string _menuPronpt = "あなたは危機脱出ゲームのAIです。危機的状況を具体的に50文字以内で1つ提示してください。\r\n【出力例】\r\n状況: [具体的な危機的状況]";
    private string _inGamePronpt = "【判定】\r\nプレイヤーの解決策を厳しく評価し、以下の形式で回答：\r\n結果：助かる/助からない\r\n理由：解決策を実行した結果と判定根拠（50文字以内）\r\n\r\n解決策の実行過程と結果を具体的に想像して判定してください。";

    public LLMResponseModel(GameStateJudge stateJuge)
    {
        _stateJuge = stateJuge;

        //ゲーム状態の購読
        stateJuge.CurrentGameState
            .Subscribe(NoticeViewActive);
    }

    public void FirstJuge()
    {
        string faultData = _faultDataRepository.GetRandomFaultData();

        RunLLM("最初に"+ faultData + "の危機的状況を提示してください。");
    }

    /// <summary>
    /// LLMの実行を開始するメソッド
    /// </summary>
    /// <param name="question">プレイヤーからの質問</param>
    public async void RunLLM(string question)
    {
        //会話履歴を取得
        string talkHistory = _talkDataMemory.GetTalkData();

        // 応答待機中のテキストをセット
        _response.Value = _waitText;

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

            //通信許可
            _acceptLLMCall.Value = true;

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

        Debug.Log(modelPath);

        using (Py.GIL())
        {
            //パイソンスクリプトをインポート
            using dynamic sample = Py.Import("llama_chat_handler");

            //Pythonスクリプトの関数を呼び出す
            using dynamic result = sample.llamaCppPython(modelPath, _mainPronpt, question);
            return result;
        }
    }

    private void NoticeViewActive(GameState gameState)
    {
        //ビューのアクティブ状態を更新
        _gameState.Value = gameState;

        if (gameState == GameState.Menu)
        {
            _talkDataMemory.ResetTalkData(); // 会話履歴をリセット

            _acceptLLMCall.Value = false; // メニュー状態ではLLMの呼び出しを無効化

            _mainPronpt = _menuPronpt; // メニューのプロンプトを設定

            FirstJuge(); // 最初の状況を提示
        }
        else if (gameState == GameState.InGame)
        {
            _mainPronpt = _inGamePronpt; // ゲーム中のプロンプト
        }
    }
}
