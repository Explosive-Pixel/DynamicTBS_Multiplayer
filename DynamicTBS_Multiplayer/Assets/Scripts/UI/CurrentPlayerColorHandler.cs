using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentPlayerColorHandler : MonoBehaviour
{
    [SerializeField] private GameObject pinkGameObject;
    [SerializeField] private GameObject blueGameObject;

    private void Awake()
    {
        GameplayEvents.OnCurrentPlayerChanged += ChangeColor;
    }

    private void Start()
    {
        ChangeColor(PlayerManager.CurrentPlayer);
    }

    private void ChangeColor(PlayerType currentPlayer)
    {
        pinkGameObject.SetActive(currentPlayer == PlayerType.pink);
        blueGameObject.SetActive(currentPlayer == PlayerType.blue);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnCurrentPlayerChanged -= ChangeColor;
    }
}
