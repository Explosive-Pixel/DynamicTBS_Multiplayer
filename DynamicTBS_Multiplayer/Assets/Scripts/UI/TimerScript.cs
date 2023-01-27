using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TimerScript : MonoBehaviour
{
    public float Timeleft;
    private float InitTime;
    public bool TimerOn = false;
    public Color Player1;
    public Color Player2;
    private int CounterPlayer1 = 0;
    private int CounterPlayer2 = 0;

    public GameObject SurrenderComponent;

    public TMPro.TMP_Text  Timertext;


    private void Start()
    {
        Timertext.color = Player1;
        InitTime = Timeleft;
        setActive();
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        GameplayEvents.OnPlayerTurnEnded += resetTimer;
    }

    private void setActive()
    {
        TimerOn = true;
    }

    private void resetTimer(Player player)
    {
        changeTextColor();
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
                increaseCounter();
                checkCounterAmount();
                SkipAction.Execute();
                Timeleft = InitTime;
            }
        }
    }

    private void changeTextColor()
    {
        if (Timertext.color == Player1)
        {
            Timertext.color = Player2;
        }
        else
        {
            Timertext.color = Player1;
        }
    }

    private void increaseCounter()
    {
        if (Timertext.color == Player1)
        {
            CounterPlayer1 += 1;
        }
        else
        {
            CounterPlayer2 += 1;
        }
    }

    private void checkCounterAmount()
    {
        if (Timertext.color == Player1)
        {
            if (CounterPlayer1 > 2)
            {
                surrender();
            }
        }
        else
        {
            if (CounterPlayer2 > 2)
            {
                surrender();
            }
        }
    }

    private void surrender()
    {
        GameplayEvents.GameIsOver(PlayerManager.GetOtherPlayer(PlayerManager.GetCurrentPlayer()).GetPlayerType(), GameOverCondition.PLAYER_SURRENDERED);
    }


    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        Timertext.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }
}