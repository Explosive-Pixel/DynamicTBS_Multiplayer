using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineServerMenu : MonoBehaviour
{
    [SerializeField] private OnlineServer server;

    private void Awake()
    {
        //server.Init(ConfigManager.Instance.Port);
        Debug.Log("Server Initialized");
    }
}
