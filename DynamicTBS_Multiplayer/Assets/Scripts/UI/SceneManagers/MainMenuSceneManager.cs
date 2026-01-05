using UnityEngine;

public class MainMenuSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject playOptionsMenu;
    [SerializeField] private GameObject tutorialButton;
    [SerializeField] private GameObject infoOptions;

    private void Start()
    {
        AudioEvents.EnteringMainMenu();
    }

    public void TogglePlayOptionsMenu()
    {
        playOptionsMenu.SetActive(!playOptionsMenu.activeSelf);
        tutorialButton.SetActive(!tutorialButton.activeSelf);

        AudioEvents.PressingButton();
    }

    public void ToggleInfoOptions()
    {
        if (infoOptions.activeSelf == true)
            infoOptions.SetActive(false);
        else
            infoOptions.SetActive(true);

        AudioEvents.PressingButton();
    }
}