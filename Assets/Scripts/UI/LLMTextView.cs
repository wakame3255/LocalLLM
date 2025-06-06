using UnityEngine;
using UnityEngine.UI;
using R3;

public class LLMTextView : MonoBehaviour
{
    [SerializeField]
    private Text _outPutText;

    [SerializeField]
    private Text _inputText;
    public Text InputText => _inputText;

    [SerializeField]
    private Button _runButton;
    public Button RunButton => _runButton;

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

    public void ChangeActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
