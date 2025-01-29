using UnityEngine;

public class SideHandler : MonoBehaviour
{
    [SerializeField] private GameObject gameObjectPink;
    [SerializeField] private GameObject gameObjectBlue;

    public void SetSide(PlayerType side)
    {
        gameObjectPink.SetActive(side == PlayerType.pink);
        gameObjectBlue.SetActive(side == PlayerType.blue);
    }
}
