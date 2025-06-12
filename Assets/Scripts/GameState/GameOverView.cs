
using UnityEngine;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{

    [SerializeField]
    private Button _mainMenuButton;
    public Button MainMenuButton => _mainMenuButton;

    public void ChangeActiveMenu(bool active)
    {
        gameObject.SetActive(active);
    }
}