using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TimerScript : MonoBehaviour
{
    #region Timer config

    private const float timePerTurn = 90;
    private const float debuffRate = 0.25f;
    private const int maxDebuffs = 3;

    #endregion

    [SerializeField]
    private Animator lampAnimator;

    public float Timeleft;
    public bool TimerOn = false;
    public Color Player1;
    public Color Player2;
    private int CounterPlayer1 = 0;
    private int CounterPlayer2 = 0;

    public GameObject SurrenderComponent;

    public TMPro.TMP_Text Timertext;

    private readonly Dictionary<Player, int> timerDebuffsPerPlayer = new Dictionary<Player, int>();


    private void Start()
    {
        Timertext.color = Player1;
        InitTime = Timeleft;

        setActive();

        foreach(Player player in PlayerManager.GetAllPlayers())
        {
            timerDebuffsPerPlayer[player] = 0;
        }

        SubscribeEvents();
    }

    private void setActive()
    {
        TimerOn = true;
    }

    private void resetTimer(Player player)
    {
        Player nextPlayer = PlayerManager.GetOtherPlayer(player);
        Timeleft = timePerTurn * Mathf.Pow(1 - debuffRate, timerDebuffsPerPlayer[nextPlayer]);

        // TODO: if
        // timerDebuffsPerPlayer[nextPlayer] = 0 -> set both lamps on
        // timerDebuffsPerPlayer[nextPlayer] = 1 -> set one lamp on, one lamp off
        // timerDebuffsPerPlayer[nextPlayer] = 2 -> set both lamps off
        lampAnimator.SetInteger("Actions", GameplayManager.maxActionsPerRound - timerDebuffsPerPlayer[nextPlayer]);

        changeTextColor();
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
                Player currentPlayer = PlayerManager.GetCurrentPlayer();
                timerDebuffsPerPlayer[currentPlayer] += 1;
                if(timerDebuffsPerPlayer[currentPlayer] == maxDebuffs)
                {
                    GameplayEvents.GameIsOver(PlayerManager.GetOtherPlayer(currentPlayer).GetPlayerType(), GameOverCondition.PLAYER_TIMEOUT);
                }

                GameplayEvents.AbortCurrentPlayerTurn();
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


    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        Timertext.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    #region Events

    private void SubscribeEvents()
    {
        GameplayEvents.OnPlayerTurnEnded += resetTimer;
    }
    private void UnsubscribeEvents()
    {
        GameplayEvents.OnPlayerTurnEnded -= resetTimer;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}