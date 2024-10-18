using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyPlayerCountHandler : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text readyPlayerCount;

    private void Update()
    {
        if (!Client.InLobby)
            return;

        readyPlayerCount.text = Client.CurrentLobby.ReadyPlayerCount + "/2";
    }
}
