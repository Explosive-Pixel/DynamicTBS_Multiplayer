using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIEvents
{
    public delegate void PositionsList(List<Vector3> positions);

    public static event PositionsList OnPassMovePositionsList;

    public static void PassMovePositionsList(List<Vector3> positions)
    {
        if (OnPassMovePositionsList != null)
            OnPassMovePositionsList(positions);
    }
}
