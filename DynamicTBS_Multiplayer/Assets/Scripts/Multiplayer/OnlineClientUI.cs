using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineClientUI : MonoBehaviour
{
    [SerializeField] private Text clientInfoText;

    private void Update()
    {
        if(Client.Instance.IsConnected)
        {
            clientInfoText.text = "You are connected!\n\nWaiting for host to start the game ...";
        } else if(Client.Instance.IsActive)
        {
            clientInfoText.text = "Trying to connect to host ...";
        } else
        {
            clientInfoText.text = "Could not connect to host. Please try again.";
        }
    }
}
