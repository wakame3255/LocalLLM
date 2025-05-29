
using UnityEngine;
using VContainer;
using VContainer.Unity;
public class GameLifeTimeScope : MonoBehaviour
{
    [SerializeField]
    private LLMTextView _llmTextView;

    private void Start()
    {
        LLMResponseModel llmResponseModel = new LLMResponseModel();
        LLMTextPresenter llmTextPresenter = new LLMTextPresenter(_llmTextView, llmResponseModel);
    }
}



