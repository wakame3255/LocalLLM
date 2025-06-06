
using UnityEngine;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{

    [SerializeField]
    private Button _reTryButton;
    public Button RetryButton => _reTryButton;

    [SerializeField]
    private Button _mainMenuButton;
    public Button MainMenuButton => _mainMenuButton;

    public void ChangeActiveMenu(bool active)
    {
        gameObject.SetActive(active);
    }
}