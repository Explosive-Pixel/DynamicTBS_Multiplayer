using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButtonHandler : MonoBehaviour, IClickableObject
{
    [SerializeField] private GameObject turnEndedButton;
    [SerializeField] private GameObject turnEndedButton_grayed;
    [SerializeField] private ClickPermission clickPermission;

    public ClickPermission ClickPermission { get { return clickPermission; } }

    private void Awake()
    {
        SubscribeEvents();
        SetActive(false, true);
    }

    public void OnClick()
    {
        if (turnEndedButton.activeSelf)
            SkipAction.Execute();
    }

    private void SetActive(bool active, bool interactable)
    {
        turnEndedButton.SetActive(active && interactable);
        turnEndedButton_grayed.SetActive(active && !interactable);
    }

    private void ChangeButtonInteractability()
    {
        SetActive(!(GameManager.gameType == GameType.ONLINE && PlayerManager.CurrentPlayer != OnlineClient.Instance.Side), GameplayManager.GetRemainingActions() == 1);
    }

    private void SetActive(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.GAMEPLAY)
            return;

        SetActive(GameManager.IsPlayer() && !(GameManager.gameType == GameType.ONLINE && PlayerManager.StartPlayer[GamePhase.GAMEPLAY] != OnlineClient.Instance.Side), false);
        GameplayEvents.OnChangeRemainingActions += ChangeButtonInteractability;
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseStart += SetActive;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseStart -= SetActive;
        GameplayEvents.OnChangeRemainingActions -= ChangeButtonInteractability;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
