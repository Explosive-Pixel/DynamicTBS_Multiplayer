using UnityEngine;

public class StunHandler : MonoBehaviour
{
    [SerializeField] private GameObject stunMarkerPink;
    [SerializeField] private GameObject stunMarkerBlue;

    public void VisualizeStun(PlayerType stunningSide)
    {
        gameObject.SetActive(true);
        stunMarkerPink.SetActive(stunningSide == PlayerType.pink);
        stunMarkerBlue.SetActive(stunningSide == PlayerType.blue);
    }

    public void TurnOffStunVisualization()
    {
        gameObject.SetActive(false);
    }
}
