using TMPro;
using UnityEngine;

public class SetupNameHandler : MonoBehaviour, ISetupHandler
{
    [SerializeField] private TMP_InputField clientName;

    public string ClientName { get { return clientName.text.Trim(); } }

    public bool SetupCompleted { get { return ClientName.Length > 0; } }

    private void Awake()
    {
        if (PlayerSetup.Name != null)
        {
            clientName.text = PlayerSetup.Name;
        }

        clientName.onValueChanged.AddListener(delegate { SetName(); });
    }

    private void Update()
    {
        clientName.interactable = !Client.InLobby;
    }

    private void SetName()
    {
        PlayerSetup.SetupName(ClientName);
    }
}
