using UnityEngine;

public class OnlineMenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject infoScreen;
    [SerializeField] private GameObject onlineMenuScreen;

    private void Update()
    {
        infoScreen.SetActive(!Client.IsConnectedToServer);
        onlineMenuScreen.SetActive(Client.IsConnectedToServer);
    }
}
