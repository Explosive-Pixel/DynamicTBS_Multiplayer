using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerHandler : MonoBehaviour
{
    public GameObject Timer;
    // Start is called before the first frame update
    void Start()
    {
        GameplayEvents.OnGameplayPhaseStart += activateTimer;
    }

    private void activateTimer()
    {
        Timer.SetActive(true);
    }
}
