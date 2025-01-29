using UnityEngine;

public class TilePreview : MonoBehaviour
{
    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject oven;
    [SerializeField] private GameObject blueStart;
    [SerializeField] private GameObject pinkStart;
    [SerializeField] private GameObject blueCaptainStart;
    [SerializeField] private GameObject pinkCaptainStart;

    private GameObject UnitStart(PlayerType side) { return side == PlayerType.blue ? blueStart : pinkStart; }
    private GameObject CaptainStart(PlayerType side) { return side == PlayerType.blue ? blueCaptainStart : pinkCaptainStart; }

    public void Init(TileType tileType, PlayerType side)
    {
        floor.SetActive(tileType != TileType.EmptyTile);
        oven.SetActive(tileType == TileType.GoalTile);
        UnitStart(side).SetActive(tileType == TileType.StartTile);
        UnitStart(PlayerManager.GetOtherSide(side)).SetActive(false);
        CaptainStart(side).SetActive(tileType == TileType.CaptainStartTile);
        CaptainStart(PlayerManager.GetOtherSide(side)).SetActive(false);
    }
}
