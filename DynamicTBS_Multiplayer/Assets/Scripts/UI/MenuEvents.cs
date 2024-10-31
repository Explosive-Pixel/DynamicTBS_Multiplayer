using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MenuEvents
{
    public delegate void SelectedLobby(Lobby lobby);
    public static event SelectedLobby OnChangeLobbySelection;

    public delegate void CurrentLobby();
    public static event CurrentLobby OnUpdateCurrentLobby;

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
}
