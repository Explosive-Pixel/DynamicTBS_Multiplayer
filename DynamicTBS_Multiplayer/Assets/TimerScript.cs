using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TimerScript : MonoBehaviour
{
    //TODO: Animation erstellen und ausführen
    public static int timerInit = 90;
    private int timer = timerInit;

    // Start is called before the first frame update
    void Start()
    {
        CountDownTimer();
        GameplayEvents.OnPlayerTurnEnded += resetTimer;
    }

    IEnumerable CountDownTimer()
    {
        yield return new WaitForSecondsRealtime(1);
        timer--;
        this.GetComponent<Text>().text = timer.ToString();
    }

    void resetTimer(Player player)
    {
        timer = timerInit;
    }
}