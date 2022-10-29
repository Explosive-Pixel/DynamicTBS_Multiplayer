using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIEvents
{
    public delegate void PositionsList(List<Vector3> positions, UIActionType type);
    public static event PositionsList OnPassActionPositionsList;

    public delegate void SinglePosition(Vector3 position, UIActionType type);
    public static event SinglePosition OnPassActionDestination;

    public delegate void NoPosition();
    public static event NoPosition OnInformNoActionDestinationsAvailable;

    public delegate void CharacterAction(Character character, UIActionType type);
    public static event CharacterAction OnCharacterSelectionForAction;

    public delegate void AfterMove(Vector3 oldPosition, Character character);
    public static event AfterMove OnMoveOver;

    public static void PassActionPositionsList(List<Vector3> positions, UIActionType type)
    {
        if (OnPassActionPositionsList != null)
            OnPassActionPositionsList(positions, type);
    }

    public static void PassActionDestination(Vector3 position, UIActionType type)
    {
        if (OnPassActionDestination != null)
            OnPassActionDestination(position, type);
    }

    public static void InformNoActionDestinationAvailable()
    {
        if (OnInformNoActionDestinationsAvailable != null)
            OnInformNoActionDestinationsAvailable();
    }

    public static void SelectCharacterForAction(Character character, UIActionType type)
    {
        if (OnCharacterSelectionForAction != null)
            OnCharacterSelectionForAction(character, type);
    }

    public static void MoveOver(Vector3 oldPosition, Character character)
    {
        if (OnMoveOver != null)
            OnMoveOver(oldPosition, character);
    }
}
