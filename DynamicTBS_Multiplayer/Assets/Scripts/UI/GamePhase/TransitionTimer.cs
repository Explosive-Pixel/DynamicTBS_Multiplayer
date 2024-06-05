using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionTimer : MonoBehaviour
{
    [SerializeField] private GamePhase gamePhase;

    private float timeLeft;
    private bool active = false;

    private void Awake()
    {
        GameEvents.OnGamePhaseEnd += SetActive;
    }

    private void Start()
    {
        timeLeft = GameManager.DelayAfterGamePhase[gamePhase];
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!active)
            return;

        timeLeft -= Time.deltaTime;
        UpdateTime();

        if (timeLeft < 0)
        {
            SetInactive();
        }
    }

    private void UpdateTime()
    {
        float currentTime = timeLeft < 0 ? 0 : timeLeft;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        gameObject.GetComponent<TMPro.TextMeshPro>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void SetActive(GamePhase gamePhase)
    {
        if (this.gamePhase != gamePhase)
            return;

        active = true;
        gameObject.SetActive(true);
    }

    private void SetInactive()
    {
        active = false;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseEnd -= SetActive;
    }
}
