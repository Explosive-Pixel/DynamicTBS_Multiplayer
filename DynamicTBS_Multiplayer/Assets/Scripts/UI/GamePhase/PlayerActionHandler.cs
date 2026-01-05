using UnityEngine;

public class PlayerActionHandler : MonoBehaviour
{
    [SerializeField] private PlayerActionType playerActionType;

    [SerializeField] private GameObject trigger_interactable;
    [SerializeField] private GameObject trigger_noninteractive;

    private void Awake()
    {
        if (SceneChangeManager.Instance.CurrentScene == Scene.TUTORIAL)
        {
            return;
        }

        SubscribeEvents();
        SetActive(false, true);
    }

    public void OnMouseDown()
    {
        if (!PlayerManager.ClientIsCurrentPlayer() || GameplayManager.gameIsPaused)
            return;

        if (trigger_interactable.activeSelf)
        {
            AudioEvents.PressingButton();
            PlayerActionUtils.ExecuteAction(PlayerActionRegistry.GetAction(playerActionType), PlayerManager.CurrentPlayer);
        }
    }

    private void SetActive(bool active, bool interactable)
    {
        bool visible = GameManager.IsPlayer() && active;
        trigger_interactable.SetActive(visible && interactable);
        trigger_noninteractive.SetActive(visible && !interactable);
        this.gameObject.SetActive(visible);
    }

    private void ChangeButtonInteractability()
    {
        ChangeButtonInteractability(PlayerManager.CurrentPlayer);
    }

    private void ChangeButtonInteractability(PlayerType player)
    {
        IPlayerAction playerAction = PlayerActionRegistry.GetAction(playerActionType);

        if (playerAction == null)
            return;

        SetActive(!PlayerSetup.IsNotSide(player), playerAction.IsActionAvailable(PlayerManager.CurrentPlayer));
    }

    private void SetActive(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.GAMEPLAY)
            return;

        SetActive(GameManager.IsPlayer() && !PlayerSetup.IsNotSide(PlayerManager.StartPlayer[GamePhase.GAMEPLAY]), false);
        GameplayEvents.OnChangeRemainingActions += ChangeButtonInteractability;
        GameplayEvents.OnCurrentPlayerChanged += ChangeButtonInteractability;
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
        GameplayEvents.OnCurrentPlayerChanged -= ChangeButtonInteractability;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
