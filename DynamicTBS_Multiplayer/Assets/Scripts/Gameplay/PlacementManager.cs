using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    private PlayerManager playerManager;

    private void Awake()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        SortCharacters();
    }

    private void SortCharacters()
    {
        // TODO
    }
    
    
}