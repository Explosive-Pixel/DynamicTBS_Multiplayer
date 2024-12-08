using UnityEngine;

public class CurrentPlayerColorHandler : MonoBehaviour
{
    [SerializeField] private GamePhase gamePhase;

    [SerializeField] private GameObject pinkGameObject;
    [SerializeField] private GameObject blueGameObject;

    private void Awake()
    {
        GameplayEvents.OnCurrentPlayerChanged += ChangeColorDuringGamePhase;
    }

    private void Start()
    {
        if (gamePhase == GamePhase.NONE)
            return;

        ChangeColor(PlayerManager.StartPlayer[gamePhase]);
    }

    private void ChangeColor(PlayerType currentPlayer)
    {
        pinkGameObject.SetActive(currentPlayer == PlayerType.pink);
        blueGameObject.SetActive(currentPlayer == PlayerType.blue);
    }

    private void ChangeColorDuringGamePhase(PlayerType currentPlayer)
    {
        if (GameManager.CurrentGamePhase != gamePhase)
            return;

        ChangeColor(currentPlayer);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnCurrentPlayerChanged -= ChangeColorDuringGamePhase;
    }
}
