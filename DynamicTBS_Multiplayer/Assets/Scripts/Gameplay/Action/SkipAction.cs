using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipAction : MonoBehaviour
{
    public static void Execute()
    {
        GameplayEvents.ActionFinished(null, ActionType.Skip, new Vector3(), null);
    }
}
