using UnityEngine;
using UnityEngine.UI;
using R3;
using R3.Triggers;

public class MainMenuView : MonoBehaviour
{
    [SerializeField]
    private Button _startButton;
    public Button StartButton => _startButton;

    [SerializeField]
    private Button _exitButton;
    public Button ExitButton => _exitButton;

    public void ChangeActiveMenu(bool active)
    {
        gameObject.SetActive(active);
    }
}