using UnityEngine;

public class ReadyPlayerCountHandler : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text readyPlayerCount;

    private void Awake()
    {
        MenuEvents.OnUpdateCurrentLobby += UpdateReadyPlayerCount;
    }

    private void UpdateReadyPlayerCount()
    {
        if (!Client.InLobby)
            return;

        readyPlayerCount.text = Client.CurrentLobby.ReadyPlayerCount + "/2";
    }

    private void OnDestroy()
    {
        MenuEvents.OnUpdateCurrentLobby -= UpdateReadyPlayerCount;
    }
}
