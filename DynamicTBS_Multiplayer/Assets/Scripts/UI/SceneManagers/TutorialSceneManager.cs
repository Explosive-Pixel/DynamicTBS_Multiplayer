using UnityEngine;

public class TutorialSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject rulesPageCanvas;
    [SerializeField] private GameObject characterPageCanvas;

    private void Awake()
    {
        GoToRulesPage();
    }

    public void GoToCharacterPage()
    {
        rulesPageCanvas.SetActive(false);
        characterPageCanvas.SetActive(true);
    }

    public void GoToRulesPage()
    {
        rulesPageCanvas.SetActive(true);
        characterPageCanvas.SetActive(false);
    }
}