using UnityEngine;
using UnityEngine.UI;
using R3.Triggers;
using R3;

public class LLMTextView : MonoBehaviour
{
    /// <summary>
    /// LLMの出力テキストを表示するためのコンポーネント
    /// </summary>
    [SerializeField]
    private GameObject _llmOutView;
    [SerializeField]
    private Text _outPutText;

    /// <summary>
    /// LLMの入力テキストを表示するためのコンポーネント
    /// </summary>
    [SerializeField]
    private GameObject _inputField;
    [SerializeField]
    private Text _inputText;
    public Text InputText => _inputText;

    [SerializeField]
    private Button _runButton;
    public Button RunButton => _runButton;

    private bool _runButtonActive = false;

    private void Start()
    {
        // LLMボタンで質問を実行
        _runButton.OnClickAsObservable()
             .Subscribe(_ => AcceptLLMButton(false));
    }

    public void SetText(string text)
    {
        if (_outPutText != null)
        {
            _outPutText.text = text;
        }
        else
        {
            Debug.LogWarning("Text component is not assigned.");
        }
    }

    /// <summary>
    /// LLMのビューをアクティブにするメソッド
    /// </summary>
    public void ChangeInGame()
    {
        gameObject.SetActive(true);

        _inputField!.gameObject.SetActive(true);
        _llmOutView!.gameObject.SetActive(true);
        _runButton!.gameObject.SetActive(false);

        _runButton!.gameObject.SetActive(_runButtonActive);

        _inputText.text = "ここに解決策を入力してね";

        DebugUtility.Log("LLMのビューをアクティブにします。入力フィールドと出力フィールドを表示します。");
    }

    /// <summary>
    /// LLMのボタンを有効にするメソッド
    /// </summary>
    public void AcceptLLMButton(bool isAccept)
    {
        _runButtonActive = isAccept;
        _runButton!.gameObject.SetActive(isAccept);

        DebugUtility.Log($"LLMの実行を許可: {isAccept}。ボタンの表示を切り替えます。");
    }

    /// <summary>
    /// 結果表示用のメソッド
    /// </summary>
    public void ChangeResult()
    {
        _runButton!.gameObject.SetActive(false);
        _inputField!.gameObject.SetActive(false);

        DebugUtility.Log("結果を表示します。");
    }

    /// <summary>
    /// メインメニューメソッド
    /// </summary>
    public void ChangeMainMenu()
    {
        gameObject.SetActive(false);

        _inputField!.gameObject.SetActive(false);
        _llmOutView!.gameObject.SetActive(false);

        DebugUtility.Log("メインメニューに戻ります。");
    }
}
