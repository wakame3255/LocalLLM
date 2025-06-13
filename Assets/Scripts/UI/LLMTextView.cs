using UnityEngine;
using UnityEngine.UI;
using R3.Triggers;
using R3;

public class LLMTextView : MonoBehaviour
{
    /// <summary>
    /// LLM�̏o�̓e�L�X�g��\�����邽�߂̃R���|�[�l���g
    /// </summary>
    [SerializeField]
    private GameObject _llmOutView;
    [SerializeField]
    private Text _outPutText;

    /// <summary>
    /// LLM�̓��̓e�L�X�g��\�����邽�߂̃R���|�[�l���g
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
        // LLM�{�^���Ŏ�������s
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
    /// LLM�̃r���[���A�N�e�B�u�ɂ��郁�\�b�h
    /// </summary>
    public void ChangeInGame()
    {
        gameObject.SetActive(true);

        _inputField!.gameObject.SetActive(true);
        _llmOutView!.gameObject.SetActive(true);
        _runButton!.gameObject.SetActive(false);

        _runButton!.gameObject.SetActive(_runButtonActive);

        _inputText.text = "�����ɉ��������͂��Ă�";

        DebugUtility.Log("LLM�̃r���[���A�N�e�B�u�ɂ��܂��B���̓t�B�[���h�Əo�̓t�B�[���h��\�����܂��B");
    }

    /// <summary>
    /// LLM�̃{�^����L���ɂ��郁�\�b�h
    /// </summary>
    public void AcceptLLMButton(bool isAccept)
    {
        _runButtonActive = isAccept;
        _runButton!.gameObject.SetActive(isAccept);

        DebugUtility.Log($"LLM�̎��s������: {isAccept}�B�{�^���̕\����؂�ւ��܂��B");
    }

    /// <summary>
    /// ���ʕ\���p�̃��\�b�h
    /// </summary>
    public void ChangeResult()
    {
        _runButton!.gameObject.SetActive(false);
        _inputField!.gameObject.SetActive(false);

        DebugUtility.Log("���ʂ�\�����܂��B");
    }

    /// <summary>
    /// ���C�����j���[���\�b�h
    /// </summary>
    public void ChangeMainMenu()
    {
        gameObject.SetActive(false);

        _inputField!.gameObject.SetActive(false);
        _llmOutView!.gameObject.SetActive(false);

        DebugUtility.Log("���C�����j���[�ɖ߂�܂��B");
    }
}
