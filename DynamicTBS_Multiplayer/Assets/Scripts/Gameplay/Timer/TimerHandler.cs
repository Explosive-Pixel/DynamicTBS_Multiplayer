using UnityEngine;

public class TimerHandler : MonoBehaviour
{
    [SerializeField] private GameObject pink;
    [SerializeField] private GameObject blue;
    [SerializeField] private GamePhase gamePhase;

    private void Awake()
    {
        TimerFactory.Create(pink, gamePhase, PlayerType.pink);
        TimerFactory.Create(blue, gamePhase, PlayerType.blue);
    }
}
