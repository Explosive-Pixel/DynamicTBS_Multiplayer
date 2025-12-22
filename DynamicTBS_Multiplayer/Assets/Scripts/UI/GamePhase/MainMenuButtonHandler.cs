using UnityEngine;

public class MainMenuButtonHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenuButton;

    void Awake()
    {
        mainMenuButton.SetActive(GameManager.GameType == GameType.LOCAL || GameManager.IsSpectator());
    }
}
