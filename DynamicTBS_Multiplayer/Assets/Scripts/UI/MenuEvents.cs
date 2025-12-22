public static class MenuEvents
{
    public delegate void SelectedLobby(Lobby lobby);
    public static event SelectedLobby OnChangeLobbySelection;

    public delegate void CurrentLobby();
    public static event CurrentLobby OnUpdateCurrentLobby;
    public static event CurrentLobby OnClosedCurrentLobby;

    public static void ChangeLobbySelection(Lobby lobby)
    {
        if (OnChangeLobbySelection != null)
            OnChangeLobbySelection(lobby);
    }

    public static void UpdateCurrentLobby()
    {
        if (OnUpdateCurrentLobby != null)
            OnUpdateCurrentLobby();
    }

    public static void CurrentLobbyClosed()
    {
        if (OnClosedCurrentLobby != null)
            OnClosedCurrentLobby();
    }
}
