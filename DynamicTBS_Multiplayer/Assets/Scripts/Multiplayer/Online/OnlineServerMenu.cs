using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineServerMenu : MonoBehaviour
{
    [SerializeField] private OnlineServer server;

    private void Awake()
    {
        server.Init(8007);
        Debug.Log("Server Initialized");
    }
}
