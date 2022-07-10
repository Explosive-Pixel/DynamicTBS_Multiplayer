using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIEvents
{
    public delegate void PositionsList(List<Vector3> positions);
    public static event PositionsList OnPassMovePositionsList;

    public delegate void SinglePosition(Vector3 position);
    public static event SinglePosition OnPassMoveDestination;

    public delegate void NoPosition();
    public static event NoPosition OnInformNoMoveDestinationsAvailable;

    public delegate void CharacterMove(Character character);
    public static event CharacterMove OnCharacterSelectionForMove;

    public delegate void AfterMove(Vector3 oldPosition, Character character);
    public static event AfterMove OnMoveOver;

    public static void PassMovePositionsList(List<Vector3> positions)
    {
        if (OnPassMovePositionsList != null)
            OnPassMovePositionsList(positions);
    }

    public static void PassMoveDestination(Vector3 position)
    {
        if (OnPassMoveDestination != null)
            OnPassMoveDestination(position);
    }

    public static void InformNoMoveDestinationAvailable()
    {
        if (OnInformNoMoveDestinationsAvailable != null)
            OnInformNoMoveDestinationsAvailable();
    }

    public static void SelectCharacterForMove(Character character)
    {
        if (OnCharacterSelectionForMove != null)
            OnCharacterSelectionForMove(character);
    }

    public static void MoveOver(Vector3 oldPosition, Character character)
    {
        if (OnMoveOver != null)
            OnMoveOver(oldPosition, character);
    }
}
