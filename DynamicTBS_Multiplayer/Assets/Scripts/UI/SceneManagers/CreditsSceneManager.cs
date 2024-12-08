using UnityEngine;

public class CreditsSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject creditsCanvas;

    private void Awake()
    {
        creditsCanvas.SetActive(true);
    }
}