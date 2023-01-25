using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftTimerScript : MonoBehaviour
{
    public float Timeleft;
    private float InitTime;
    public bool TimerOn = false;

    public TMPro.TMP_Text Timertext;


    private void Start()
    {
        InitTime = Timeleft;
        setActive();
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        DraftEvents.OnDraftMessageTextChange += resetTimer;
    }

    private void setActive()
    {
        Debug.Log("Timer Active");
        TimerOn = true;
    }

    private void resetTimer()
    {
        Timeleft = InitTime;
    }

    private void Update()
    {
        if (TimerOn)
        {
            if (Timeleft > 0)
            {
                Timeleft -= Time.deltaTime;
                updateTimer(Timeleft);
            }
            else
            {
                //TODO: Random Draft + next Player
                Timeleft = InitTime;
            }
        }
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        Timertext.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }
}
