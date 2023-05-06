using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class cardhandleScript : MonoBehaviour
{
    //notwendig um in UI zu deaktivieren
    private void Start(){}

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += ActivateCardHandling;
    }

    /// <summary>method <c>setActive</c> aktiviert Kind nach character in cardClass.</summary>
    public void SetActive(Character character)
    {
        foreach (Transform card in transform)
        {
            card.gameObject.SetActive((character != null ? character.GetCharacterType() : null) == card.gameObject.GetComponent<cardClass>().character);
        }
    }

    private void Deactivate(PlayerType? winner, GameOverCondition gameOverCondition)
    {
        SetActive(null);
    }

    private void ActivateCardHandling(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.GAMEPLAY)
            return;

        GameplayEvents.OnCharacterSelectionChange += SetActive;
        GameplayEvents.OnGameOver += Deactivate;
    }

    private void OnDestroy()
    {
        GameplayEvents.OnCharacterSelectionChange -= SetActive;
        GameEvents.OnGamePhaseStart -= ActivateCardHandling;
        GameplayEvents.OnGameOver -= Deactivate;
    }
}
