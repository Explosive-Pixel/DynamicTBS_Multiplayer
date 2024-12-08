using UnityEngine;
using UnityEngine.UI;

public class NavigationButtonHandler : MonoBehaviour
{
    [SerializeField] private Scene targetScene;

    private void Awake()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(NavigateToTargetScene);
    }

    private void NavigateToTargetScene()
    {
        AudioEvents.PressingButton();
        SceneChangeManager.Instance.LoadScene(targetScene);
    }
}
