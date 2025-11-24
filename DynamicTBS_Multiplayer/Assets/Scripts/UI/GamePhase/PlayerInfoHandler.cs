using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class PlayerInfoHandler : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshPro playerNamePink;
    [SerializeField] private TMPro.TextMeshPro playerNameBlue;

    [SerializeField] private Text youArePlayer;

    [SerializeField] private LocalizedString youArePlayerLocalized;

    private void Awake()
    {
        playerNamePink.text = PlayerSetup.GetSideName(PlayerType.pink);
        playerNameBlue.text = PlayerSetup.GetSideName(PlayerType.blue);
    }

    private void Start()
    {
        youArePlayer.text = "";

        if (GameManager.IsPlayer() && GameManager.GameType == GameType.ONLINE)
        {
            youArePlayerLocalized.Arguments = new object[]
            {
                PlayerSetup.SideName
            };

            youArePlayerLocalized.StringChanged += (value) =>
            {
                youArePlayer.text = value;
            };
        }
    }

    private void Update()
    {
        if (playerNamePink == null || playerNameBlue == null)
            return;

        if (GameManager.GameType == GameType.ONLINE && Client.InLobby)
        {
            playerNamePink.text = Client.CurrentLobby.GetPlayerName(PlayerType.pink);
            playerNameBlue.text = Client.CurrentLobby.GetPlayerName(PlayerType.blue);
        }
    }
}
