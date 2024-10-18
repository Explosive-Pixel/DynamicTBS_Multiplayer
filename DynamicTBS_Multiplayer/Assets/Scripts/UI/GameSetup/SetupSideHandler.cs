using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class SetupSideHandler : MonoBehaviour, ISetupHandler
{
    [SerializeField] private Button selectPinkButton;
    [SerializeField] private Button selectBlueButton;
    [SerializeField] private Button selectRandomSideButton;

    public PlayerType SelectedSide { get; private set; } = PlayerType.none;

    public bool SetupCompleted { get { return SelectedSide != PlayerType.none; } }

    private void Awake()
    {
        selectPinkButton.onClick.AddListener(() => SelectPinkSide());
        selectBlueButton.onClick.AddListener(() => SelectBlueSide());
        selectRandomSideButton.onClick.AddListener(() => SelectRandomSide());
    }

    private void SelectPinkSide()
    {
        SelectSide(PlayerType.pink, false);
    }

    private void SelectBlueSide()
    {
        SelectSide(PlayerType.blue, false);
    }

    private void SelectRandomSide()
    {
        PlayerType randomSide = (PlayerType)RandomNumberGenerator.GetInt32((int)PlayerType.pink, (int)PlayerType.blue + 1);
        SelectSide(randomSide, true);
    }

    private void SelectSide(PlayerType side, bool random)
    {
        AudioEvents.PressingButton();

        SelectedSide = side;
        PlayerSetup.SetupSide(side);

        selectBlueButton.interactable = random || side != PlayerType.blue;
        selectPinkButton.interactable = random || side != PlayerType.pink;
        selectRandomSideButton.interactable = !random;
    }
}
