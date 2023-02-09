using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGameButtonHandler : MonoBehaviour
{
    public void StartLocalGame()
    {
        GameEvents.StartGame();
    }
}
