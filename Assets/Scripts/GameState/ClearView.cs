
using UnityEngine;
using UnityEngine.UI;

public class ClearView : MonoBehaviour
{

    [SerializeField]
    private Button _mainMenuButton;
    public Button MainMenuButton => _mainMenuButton;

    public void ChangeActiveMenu(bool active)
    {
        gameObject.SetActive(active);
    }
}